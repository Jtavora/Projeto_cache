import os
import redis
import json

class Cache:
    def __init__(self):
        self.redis = redis.Redis(host='redis', port=6379, db=0)

    def get(self, key):
        value = self.redis.get(key)
        return json.loads(value) if value else None

    def set(self, key, value, ex):
        self.redis.set(key, json.dumps(value), ex=ex)