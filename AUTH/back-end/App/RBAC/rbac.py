from App.Models import PermissionModel

class Rbac:
    def __init__(self):
        self.all_permissions = [permission['name'] for permission in PermissionModel.get_all_permissions()]

    def exist_permission(self, role):
        return role in self.all_permissions
    
    def can_acess(self, role, feature):
        for permission in PermissionModel.get_features_by_role(role):
            if permission == feature or permission == 'all':
                return True
        
        return False