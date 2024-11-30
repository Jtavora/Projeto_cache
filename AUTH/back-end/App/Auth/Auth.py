import os
from dotenv import load_dotenv
from jose import jwt, JWTError
from App.Models import UserModel, PermissionModel
from passlib.context import CryptContext
from datetime import datetime, timedelta
from App.ABAC import Abac
from fastapi.responses import JSONResponse

crypto = CryptContext(schemes=["sha256_crypt"])
load_dotenv()

secret_key = os.getenv("SECRET_KEY")
algorithm = os.getenv("ALGORITHM")

class Auth:
    def __init__(self):
        self.secret_key = secret_key
        self.algorithm = algorithm
        self.abac = Abac()

    def user_login(self, data):
        if not self.abac.can_acess(data.location):
            return JSONResponse(status_code=403, content={"message": "Location not allowed"})
        
        user = UserModel.get_user_by_username(data.username)

        if user:
            if not crypto.verify(data.password, user.hashed_password):
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
                return JSONResponse(status_code=403, content={"message": "Invalid credentials"})

        exp = datetime.utcnow() + timedelta(minutes=30)
        payload = {
            "username": user.username,
            "role": user.role,
            "exp": exp
        }

        token = jwt.encode(payload, self.secret_key, algorithm=self.algorithm)

        return {
            "access_token": token,
            "token_type": "bearer",
            "exp": exp.isoformat(),
            "role": user.role,
            "username": user.username,
            "id": str(user.id)
        }

    def verify_token(self, token):
        try:
            payload = jwt.decode(token, self.secret_key, algorithms=[self.algorithm])
            return payload
        except JWTError:
            return JSONResponse(status_code=403, content={"message": "Invalid token"})