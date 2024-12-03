from App.Models import PermissionModel
from App.RBAC import Rbac
from App.LogService import LoggingService
from fastapi.responses import JSONResponse

class PermissionController:
    def __init__(self):
        self.rbac = Rbac()
        self.log = LoggingService()

    def create_permission(self, permission_data, token_data):
        try:
            if not self.rbac.can_acess(token_data['role'], 'create_permission'):
                self.log.log(f"[PERMISSION FAILED] User: {token_data["username"]} | Permission denied")
                return JSONResponse(status_code=403, content={"message": "Permission denied"})
        except Exception as e:
            self.log.log(f"[PERMISSION FAILED] {e}")
            return JSONResponse(status_code=400, content={"message": str(e)})
        
        new_permission = PermissionModel(
            name=permission_data.name,
            features=permission_data.features
        )
        permission_id = PermissionModel.create_permission(new_permission)

        self.log.log(f"[PERMISSION CREATED] User: {token_data["username"]} | New Permission: {new_permission.name}")
        return permission_id

    def delete_permission(self, permission_id, token_data):
        try:
            if not self.rbac.can_acess(token_data['role'], 'delete_permission'):
                self.log.log(f"[PERMISSION FAILED] User: {token_data["username"]} | Permission denied")
                return JSONResponse(status_code=403, content={"message": "Permission denied"})
        except Exception as e:
            self.log.log(f"[PERMISSION FAILED] {e}")
            return JSONResponse(status_code=400, content={"message": str(e)})
        
        deleted = PermissionModel.delete_permission(permission_id)
        self.log.log(f"[PERMISSION DELETED] User: {token_data["username"]} | Permission ID: {permission_id}")
        
        return deleted

    def get_all_permissions(self, token_data):
        try:
            if not self.rbac.can_acess(token_data['role'], 'get_all_permissions'):
                self.log.log(f"[GET ALL PERMISSIONS FAILED] User: {token_data["username"]} | Permission denied")
                return JSONResponse(status_code=403, content={"message": "Permission denied"})
        except Exception as e:
            self.log.log(f"[GET ALL PERMISSIONS FAILED] {e}")
            return JSONResponse(status_code=400, content={"message": str(e)})
        
        permissions = PermissionModel.get_all_permissions()
        return [permission for permission in permissions]