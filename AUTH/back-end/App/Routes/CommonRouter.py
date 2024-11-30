from fastapi import APIRouter
from fastapi.security import HTTPBearer
from fastapi import Depends
from App.Auth import Auth

authe = Auth()
token_scheme = HTTPBearer()

def token_verifier(token: str = Depends(token_scheme)):
    data = authe.verify_token(token.credentials)
    return data

userRouter = APIRouter(dependencies=[Depends(token_verifier)])
permissionRouter = APIRouter(dependencies=[Depends(token_verifier)])
loginRouter = APIRouter()