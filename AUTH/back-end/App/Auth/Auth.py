import os
from dotenv import load_dotenv
from jose import jwt, JWTError
from App.Models import UserModel, PermissionModel
from passlib.context import CryptContext
from datetime import datetime, timedelta
from App.ABAC import Abac
from fastapi.responses import JSONResponse
from App.CacheService import Cache
from App.LogService import LoggingService
import calendar

crypto = CryptContext(schemes=["sha256_crypt"])
load_dotenv()

secret_key = os.getenv("SECRET_KEY")
algorithm = os.getenv("ALGORITHM")

class Auth:
    def __init__(self):
        self.secret_key = secret_key
        self.algorithm = algorithm
        self.abac = Abac()
        self.cache = Cache()
        self.log = LoggingService()

    def user_login(self, data):
        if not self.abac.can_acess(data.location):
            self.log.log("[LOGIN FAILED] - Location not allowed")
            return JSONResponse(status_code=403, content={"message": "Location not allowed"})
        
        user = UserModel.get_user_by_username(data.username)

        if user:
            if not crypto.verify(data.password, user.hashed_password):
                self.log.log("[LOGIN FAILED] - Invalid credentials")
                return JSONResponse(status_code=403, content={"message": "Invalid credentials"})
        else:
            if data.username == "admin" and data.password == "admin":
                user = UserModel(
                username="admin", 
                hashed_password=crypto.hash("admin"), 
                role="admin")
                UserModel.create_user(user)

                permission = PermissionModel(
                    name="admin",
                    features=["all"]
                )
                PermissionModel.create_permission(permission)
                
            else:
                self.log.log("[LOGIN FAILED] - Invalid credentials")
                return JSONResponse(status_code=403, content={"message": "Invalid credentials"})

        # Verificar se já existe um token válido no cache
        cached_token = self.cache.get(user.username)
        if cached_token:
            exp_timestamp = cached_token['exp']
            if datetime.utcnow() < datetime.utcfromtimestamp(exp_timestamp):
                self.log.log(f"[LOGIN SUCCESS] - Token retrieved from cache | User: {user.username}")
                return cached_token

        # Configurando o tempo de expiração para 2 minutos no futuro
        exp = datetime.utcnow() + timedelta(minutes=2)
        exp_timestamp = calendar.timegm(exp.utctimetuple())

        # Criando o payload do JWT
        payload = {
            "username": user.username,
            "role": user.role,
            "exp": exp_timestamp,
            "iss": "auth"
        }

        token = jwt.encode(payload, self.secret_key, algorithm=self.algorithm)

        # Armazenar token no cache com tempo de expiração
        cached_token = {
            "access_token": token,
            "token_type": "bearer",
            "exp": exp_timestamp,
            "role": user.role,
            "username": user.username,
            "id": str(user.id)
        }
        self.cache.set(user.username, cached_token, ex=120)  # Expira em 120 segundos

        self.log.log(f"[LOGIN SUCCESS] - Token created | User: {user.username}")
        return cached_token

    def verify_token(self, token):
        try:
            payload = jwt.decode(token, self.secret_key, algorithms=[self.algorithm])
            return payload
        except JWTError:
            return JSONResponse(status_code=403, content={"message": "Invalid token"})