from fastapi import APIRouter
from fastapi.security import HTTPBearer
from fastapi import Depends
from App.Auth import Auth
from fastapi import HTTPException

authe = Auth()
token_scheme = HTTPBearer()

def token_verifier(token: str = Depends(token_scheme)):
    data = authe.verify_token(token.credentials)

    if not data:
        raise HTTPException(status_code=401, detail="Invalid token")

    return data

userRouter = APIRouter(dependencies=[Depends(token_verifier)])
permissionRouter = APIRouter(dependencies=[Depends(token_verifier)])
loginRouter = APIRouter()