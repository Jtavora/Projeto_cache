from App.Models import UserModel
from App.Models import PermissionModel
from App.Auth import crypto
from App.RBAC import Rbac
from fastapi.responses import JSONResponse

class UserController:
    def __init__(self):
        self.all_permissions = [permission['name'] for permission in PermissionModel.get_all_permissions()]
        self.rbac = Rbac()

    def create_user(self, user_data, token_data):
        if not self.rbac.can_acess(token_data['role'], 'create_user'):
            return JSONResponse(status_code=403, content={"message": "Permission denied"})

        hashed_password = crypto.hash(user_data.hashed_password)
        new_user = UserModel(
            username=user_data.username,
            hashed_password=hashed_password,
            role=user_data.role
        )

        if not self.rbac.exist_permission(user_data.role):
            return JSONResponse(status_code=400, content={"message": "Invalid role"})

        user_id = UserModel.create_user(new_user)
        return JSONResponse(status_code=201, content=user_id)

    def get_user_by_username(self, username):
        user = UserModel.get_user_by_username(username)
        if user:
            return user
        return None

    def get_all_users(self, token_data):
        if not self.rbac.can_acess(token_data['role'], 'get_all_users'):
            return JSONResponse(status_code=403, content={"message": "Permission denied"})
        
        users = UserModel.get_all_users()
        return [user for user in users]