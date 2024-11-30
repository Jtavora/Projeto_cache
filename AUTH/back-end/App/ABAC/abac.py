class Abac:
    def __init__(self):
        self.permited_locations = ["SP-Office", "DF-Office"]
    
    def can_acess(self, location):
        if location in self.permited_locations:
            return True
        return False