from App.Models import PermissionModel
from App.RBAC import Rbac
from fastapi.responses import JSONResponse

class PermissionController:
    def __init__(self):
        self.rbac = Rbac()

    def create_permission(self, permission_data, token_data):
        if not self.rbac.can_acess(token_data['role'], 'create_permission'):
            return JSONResponse(status_code=403, content={"message": "Permission denied"})
        
        new_permission = PermissionModel(
            name=permission_data.name,
            features=permission_data.features
        )
        permission_id = PermissionModel.create_permission(new_permission)
        return permission_id

    def delete_permission(self, permission_id, token_data):
        if not self.rbac.can_acess(token_data['role'], 'delete_permission'):
            return JSONResponse(status_code=403, content={"message": "Permission denied"})
        
        deleted = PermissionModel.delete_permission(permission_id)
        return deleted

    def get_all_permissions(self, token_data):
        if not self.rbac.can_acess(token_data['role'], 'get_all_permissions'):
            return JSONResponse(status_code=403, content={"message": "Permission denied"})
        
        permissions = PermissionModel.get_all_permissions()
        return [permission for permission in permissions]