from fastapi import FastAPI
from App.Routes import *

app = FastAPI()

@app.get("/")
def read_root():
    return {"Hello": "World"}

app.include_router(userRouter, prefix="/users", tags=["users"])
app.include_router(permissionRouter, prefix="/permissions", tags=["permissions"])
app.include_router(loginRouter, tags=["login"])

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=4000)