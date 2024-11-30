from fastapi import HTTPException
from App.Models.PydanticModels import *
from App.Controller import UserController
from .CommonRouter import userRouter
from fastapi import Depends
from .CommonRouter import token_verifier

user_controller = UserController()

@userRouter.post("/create_user")
def create_user(user: User, token_data: dict = Depends(token_verifier)):
    usuario = user_controller.create_user(user, token_data)
    return usuario

@userRouter.get("/get_all_users")
def get_all_users(token_data: dict = Depends(token_verifier)):
    return user_controller.get_all_users(token_data)