FROM python:3.10-slim
LABEL authors="mikitomi21"

# ENV MQTT_BROKER=mqtt-broker
# ENV MQTT_PORT=1883
# ENV MQTT_TOPIC=sensor/data

# 

WORKDIR /app

COPY generator.py /app/generator.py
COPY generator_custom.py /app/generator_custom.py
RUN pip install paho-mqtt

CMD ["python", "generator_custom.py"]