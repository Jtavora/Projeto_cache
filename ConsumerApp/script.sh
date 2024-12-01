#!/bin/bash
set -e

echo "Iniciando script"
echo "Diretório atual: $(pwd)"
echo "Conteúdo do diretório:"
ls -l

if [ -d "venv" ]; then
  source venv/bin/activate
  echo "Ambiente virtual ativado"
fi

echo "Verificando pacotes instalados"
pip freeze

echo "Executando launcher.py"
python launcher.py

echo "Script finalizado com sucesso"