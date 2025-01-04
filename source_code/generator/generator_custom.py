import paho.mqtt.client as mqtt
import random
import time
import json
import os
from threading import Thread
 
MQTT_BROKER = "localhost"
MQTT_PORT = 1883
MQTT_TOPIC = "sensor/data"

 
class GeneratorManager:
    def __init__(self, broker, port, topic):
        self.broker = broker
        self.port = port
        self.topic = topic
        self.client = mqtt.Client()
        self.connect()
 
    def connect(self):
        self.client.connect(self.broker, self.port)
        self.client.loop_start()
 
    def publish_sensor_data(self, sensor_id: int, sensor_type: str, value_range: tuple, interval: float):
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
 
    def publish_single_value(self, sensor_id: int, sensor_type: str, value: float):
        data = {
            "sensor_id": sensor_id,
            "sensor_type": sensor_type,
            "value": value,
            "timestamp": time.time()
        }
        self.client.publish(self.topic, json.dumps(data))
        print(f"Published single value: {data}")
 
def main():
    generator = GeneratorManager(MQTT_BROKER, MQTT_PORT, MQTT_TOPIC)
 
    # TODO tutaj trzeba uzupelnic wartosci
    single_value_sensor_id = 5
    single_value_sensor_type = "speed"
    single_value = 10.0
    generator.publish_single_value(single_value_sensor_id, single_value_sensor_type, single_value)
 
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Stopping simulation...")
 
if __name__ == '__main__':
    main()
 