FROM python:3.10-slim
LABEL authors="mikitomi21"

WORKDIR /app

COPY generator.py /app/generator.py
RUN pip install paho-mqtt

CMD ["python", "generator.py"]