from pydantic import BaseModel, UUID4, Field
from typing import Optional, List

# Modelos de usuário
class User(BaseModel):
    username: str
    hashed_password: str
    role: str

class UserLogin(BaseModel):
    username: str
    password: str

class UserUpdate(BaseModel):
    username: str
    role: str

class UserResponse(BaseModel):
    id: UUID4
    username: str
    hashed_password: str
    role: str

class Login(BaseModel):
    username: str
    password: str
    location: str

# Modelos de permissão
class Permission(BaseModel):
    name: str
    features: List[str]

class PermissionResponse(BaseModel):
    id: UUID4
    name: str
    features: List[str]

class PermissionUpdate(BaseModel):
    name: str
    features: List[str]

class PermissionListResponse(BaseModel):
    permissions: List[PermissionResponse]