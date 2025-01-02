#!/bin/bash
# Uruchomienie Ganache w tle
# ganache --port 7545 --host 0.0.0.0 --accounts 20 --defaultBalanceEther 200 --chain.chainId 581234 --chain.networkId 581234 > /truffle/ganache-output.log 2>&1 &
# ganache --port 7545 --host 0.0.0.0 --accounts 20 --defaultBalanceEther 200 --chain.chainId 581234 --chain.networkId 581234 | tee /truffle/ganache-full.log &
# Czekanie na dostępność Ganache
echo "Czekam na uruchomienie Ganache... dupa"
until curl --silent http://ganache:7545 > /dev/null; do
    echo "Ganache jeszcze nie gotowe..."
    sleep 1
done

echo "Ganache uruchomione. Startuję Truffle migrację."

# Uruchomienie Truffle
truffle migrate --network development
