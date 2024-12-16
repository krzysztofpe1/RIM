import paho.mqtt.client as mqtt
import random
import time
import json
import os
from threading import Thread

MQTT_BROKER = os.getenv("MQTT_BROKER", "localhost")  # Domyślnie 'localhost', jeśli zmienna nie jest ustawiona
MQTT_PORT = int(os.getenv("MQTT_PORT", 1883))
MQTT_TOPIC = os.getenv("MQTT_TOPIC", "sensor/data")

class GeneratorManager:
    def __init__(self, broker, port, topic):
        self.broker = broker
        self.port = port
        self.topic = topic
        self.client = mqtt.Client(mqtt.CallbackAPIVersion.VERSION2, "client_id")

        self.connect()

    def connect(self):
        self.client.connect(self.broker, self.port)
        self.client.loop_start()

    def publish_sensor_data(self, sensor_id: int, sensor_type: str, value_range: tuple, interval: float) -> str:
        while True:
            value = round(random.uniform(value_range[0], value_range[1]), 2)
            data = {
                "sensor_id": sensor_id,
                "sensor_type": sensor_type,
                "value": value,
                "timestamp": time.time()
            }
            self.client.publish(self.topic, json.dumps(data))
            print(f"Published: {data}")
            time.sleep(interval)


def main():
    sensor_definitions = [
        {"type": "speed", "value_range": (0, 150)},
        {"type": "light", "value_range": (0, 1000)},
        {"type": "temperature", "value_range": (-30, 50)},
        {"type": "vibration", "value_range": (0, 10)}
    ]

    sensors_per_type = 4
    publish_rate_per_minute = 12

    generator = GeneratorManager(MQTT_BROKER, MQTT_PORT, MQTT_TOPIC)

    threads = []
    for sensor_type in sensor_definitions:
        for sensor_id in range(sensors_per_type):
            interval = 60 / publish_rate_per_minute
            thread = Thread(
                target=generator.publish_sensor_data,
                args=(len(threads), sensor_type["type"], sensor_type["value_range"], interval)
            )
            thread.daemon = True
            threads.append(thread)
            thread.start()

    # Keep main thread alive
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Stopping simulation...")

    # sensor_count = 16
    # publish_interval = 5
    #
    # generator = GeneratorManager(MQTT_BROKER, MQTT_PORT, MQTT_TOPIC)
    #
    #
    # for sensor_id in range(sensor_count):
    #     generator.publish_sensor_data(sensor_id, publish_interval)



#
# def on_connect(client, userdata, flags, rc):
#     print("Connected with result code " + str(rc))
# from paho.mqtt import client as mqtt_client

# def main():
#     from paho.mqtt import client as mqtt_client
#     import random
#
#     broker = 'broker.emqx.io'
#     port = 1883
#     topic = "python/mqtt"
#     client_id = f'python-mqtt-{random.randint(0, 1000)}'
#     client = mqtt_client.Client(mqtt_client.CallbackAPIVersion.VERSION2, client_id)
#     print(client)

if __name__ == '__main__':
    main()

