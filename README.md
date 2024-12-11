# RIM

## Definition and explanation
RIM stands for Road Infrastructure Maintenance. This is a set of applications meant to be deployed as docker containers, which simulates road infrastructure maintenance system sensors.

## Contains

 1. RabbitMQ docker image
 2. MongoDB docker image
 3. Data Generator - pushes data from sensors into the queue system
 4. Data Retriever - retrieves data from the queue and persists it in the NOSQL database
 5. Frontend App - privides views with several business features
