from pymongo import MongoClient, ReadPreference
from bson import ObjectId
import uuid

# Configuração da conexão com o MongoDB
client = MongoClient("mongodb://user:password@mongo:27017/", read_preference=ReadPreference.PRIMARY)
db = client["AppDatabase"]  # Substitua pelo nome do seu banco de dados
users_collection = db["users"]

class UserModel:
    def __init__(self, username, hashed_password, role, user_id=None):
        self.id = user_id if user_id else str(uuid.uuid4())
        self.username = username
        self.hashed_password = hashed_password
        self.role = role

    def to_dict(self):
        return {
            '_id': self.id,
            'username': self.username,
            'hashed_password': self.hashed_password,
            'role': self.role
        }

    @staticmethod
    def create_user(user_data):
        result = users_collection.insert_one(user_data.to_dict())
        if result:
            return user_data.to_dict()
        return None

    @staticmethod
    def update_user(user_data):
        result = users_collection.update_one({"_id": user_data.id}, {"$set": user_data.to_dict()})
        if result:
            return user_data.to_dict()
        return None

    @staticmethod
    def get_user_by_username(username):
        user_data = users_collection.find_one({"username": username})
        if user_data:
            return UserModel(
                username=user_data['username'],
                hashed_password=user_data['hashed_password'],
                role=user_data['role'],
                user_id=str(user_data['_id'])  # Converte ObjectId para string
            )
        return None

    @staticmethod
    def get_all_users():
        users = users_collection.find()
        return [UserModel(
            username=user['username'],
            hashed_password=user['hashed_password'],
            role=user['role'],
            user_id=str(user['_id'])
        ) for user in users]