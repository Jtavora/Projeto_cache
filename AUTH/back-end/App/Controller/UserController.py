from App.Models import UserModel
from App.Models import PermissionModel
from App.Auth import crypto
from App.RBAC import Rbac
from App.LogService import LoggingService
from fastapi.responses import JSONResponse

class UserController:
    def __init__(self):
        self.all_permissions = [permission['name'] for permission in PermissionModel.get_all_permissions()]
        self.rbac = Rbac()
        self.log = LoggingService()

    def create_user(self, user_data, token_data):
        try:
            if not self.rbac.can_acess(token_data['role'], 'create_user'):
                self.log.log(f"[CREATE USER FAILED] User: {token_data['username']} | Permission denied for {token_data['role']} to create user")
                return JSONResponse(status_code=403, content={"message": "Permission denied"})
        except Exception as e:
            self.log.log(f"[CREATE USER FAILED] {e}")
            return JSONResponse(status_code=400, content={"message": str(e)})
        
        hashed_password = crypto.hash(user_data.hashed_password)
        new_user = UserModel(
            username=user_data.username,
            hashed_password=hashed_password,
            role=user_data.role
        )

        if not self.rbac.exist_permission(user_data.role):
            self.log.log(f"[CREATE USER FAILED] User: {token_data['username']} | Invalid role")
            return JSONResponse(status_code=400, content={"message": "Invalid role"})

        user_id = UserModel.create_user(new_user)
        
        self.log.log(f"[CREATE USER SUCCESS] User: {token_data['username']} | New user created: {user_data.username}")
        return JSONResponse(status_code=201, content=user_id)
    
    def update_user(self, user_data, token_data):
        try:
            if not self.rbac.can_acess(token_data['role'], 'update_user'):
                self.log.log(f"[UPDATE USER FAILED] User: {token_data['username']} | Permission denied for {token_data['role']} to update user")
                return JSONResponse(status_code=403, content={"message": "Permission denied"})
        except Exception as e:
            self.log.log(f"[UPDATE USER FAILED] {e}")
            return JSONResponse(status_code=400, content={"message": str(e)})
        
        user = UserModel.get_user_by_username(user_data.username)
        if not user:
            self.log.log(f"[UPDATE USER FAILED] User: {token_data['username']} | User not found: {user_data.username}")
            return JSONResponse(status_code=404, content={"message": "User not found"})
        
        hashed_password = crypto.hash(user_data.hashed_password)
        user.hashed_password = hashed_password
        user.role = user_data.role

        if not self.rbac.exist_permission(user_data.role):
            self.log.log(f"[UPDATE USER FAILED] User: {token_data['username']} | Invalid role")
            return JSONResponse(status_code=400, content={"message": "Invalid role"})

        UserModel.update_user(user)
        
        self.log.log(f"[UPDATE USER SUCCESS] User: {token_data['username']} | User updated: {user_data.username}")
        return JSONResponse(status_code=200, content={"message": "User updated successfully"})

    def get_user_by_username(self, username):
        user = UserModel.get_user_by_username(username)
        if user:
            return user
        return None

    def get_all_users(self, token_data):
        try:
            if not self.rbac.can_acess(token_data['role'], 'get_all_users'):
                return JSONResponse(status_code=403, content={"message": "Permission denied"})
        except Exception as e:
            self.log.log(f"[GET ALL USERS FAILED] {e}")
            return JSONResponse(status_code=400, content={"message": str(e)})
        
        users = UserModel.get_all_users()
        return [user for user in users]