import requests
import json
import uuid

class LoggingService:
    def __init__(self):
        self.log_service_url = "http://log-monitoring:5255/api/Logs"

    def log(self, message):
        log_entry = {
            "logId": str(uuid.uuid4()),
            "message": message
        }

        headers = {'Content-Type': 'application/json'}
        try:
            response = requests.post(self.log_service_url, data=json.dumps(log_entry), headers=headers)
            response.raise_for_status()
            print("Log enviado com sucesso.")
        except requests.exceptions.RequestException as e:
            print(f"Erro ao enviar log: {e}")

# Exemplo de uso
if __name__ == "__main__":
    logging_service = LoggingService("http://log-monitoring:5255/api/Logs")
    logging_service.log("Usuário admin tentou logar.")