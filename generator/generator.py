import paho.mqtt.client as mqtt
import random
import time
import json

MQTT_BROKER = "localhost"
MQTT_PORT = 1883
MQTT_TOPIC = "sensor/data"

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

    def publish_sensor_data(self, sensor_id: int, publish_interval: int) -> None:

        while True:
            data = self.generate_sensor_data(sensor_id)
            self.client.publish(self.topic, data)
            print(f"Published {data}")
            time.sleep(publish_interval)


    def generate_sensor_data(self, sensor_id: int) -> str:
        sensor = {
            "siema": "elo"
        }

        data = {
            "sensor_id": sensor_id,
            "sensor_type": "siema",
            "value": sensor["siema"],
            "timestamp": time.time()
        }
        return json.dumps(data)


def main():
    sensor_count = 16
    publish_interval = 5

    generator = GeneratorManager(MQTT_BROKER, MQTT_PORT, MQTT_TOPIC)


    for sensor_id in range(sensor_count):
        generator.publish_sensor_data(sensor_id, publish_interval)



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

