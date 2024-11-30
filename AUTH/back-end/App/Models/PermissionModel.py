from pymongo import MongoClient
from bson import ObjectId
import uuid

# Configuração da conexão com o MongoDB
client = MongoClient("mongodb://user:password@mongo:27017/")
db = client["AppDatabase"]
permissions_collection = db["permissions"]

class PermissionModel:
    def __init__(self, name, features):
        self.id = str(uuid.uuid4())
        self.name = name
        self.features = features

    def to_dict(self):
        return {
            '_id': self.id,
            'name': self.name,
            'features': self.features
        }

    @staticmethod
    def create_permission(permission_data):
        """Cria uma nova permissão com funcionalidades associadas."""
        result = permissions_collection.insert_one(permission_data.to_dict())
        if result.acknowledged:
            return permission_data
        return None

    @staticmethod
    def delete_permission(permission_id):
        """Deleta uma permissão pelo ID."""
        result = permissions_collection.delete_one({"_id": permission_id})
        return result.deleted_count > 0

    @staticmethod
    def get_all_permissions():
        """Retorna todas as permissões."""
        permissions = permissions_collection.find()
        return list(permissions)
    
    @staticmethod
    def get_features_by_role(role):
        """Retorna as funcionalidades de uma permissão."""
        permission = permissions_collection.find_one({"name": role})
        return permission['features'] if permission else None