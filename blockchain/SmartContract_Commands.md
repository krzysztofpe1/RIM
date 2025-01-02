# 📋 Lista kluczowych komend do pracy ze smart kontraktami

### 1. Instalacja narzędzi
```
npm install -g truffle ganache
```

### 2. Tworzenie projektu Truffle
```
mkdir MySmartContract
cd MySmartContract
truffle init
```

### 3. Uruchamianie Ganache
Uruchomienie lokalnego blockchaina:
```
ganache --port 7545 --accounts 20 --defaultBalanceEther 200 --db ./blockchain-data
```

### 4. Kompilacja kontraktów
Kompilacja plików `.sol` w folderze `contracts`:
```
truffle compile
```

### 5. Migracja kontraktów
Wdrożenie kontraktów na lokalnym blockchainie:
```
truffle migrate --network development
```

### 6. Otwieranie konsoli Truffle
Dostęp do konsoli do interakcji z kontraktami:
```
truffle console --network development
```

### 7. Interakcja z kontraktem w konsoli
#### Pobranie instancji kontraktu:
```
let instance = await MyContract.deployed();
```

#### Wywołanie funkcji zapisu:
```
await instance.setTemperature(25);
```

#### Wywołanie funkcji odczytu:
```
let temp = await instance.getTemperature();
temp.toString(); // Wyświetli wartość
```

### 8. Uruchamianie testów
Automatyczne testy kontraktów w folderze `test/`:
```
truffle test
```

### 9. Wykonywanie skryptów
Uruchomienie własnych skryptów do interakcji z kontraktami:
```
truffle exec scripts/myScript.js --network development
```

### 10. Migracja na inne sieci
#### Ropsten (testnet Ethereum):
1. Skonfiguruj sieć w `truffle-config.js`.
2. Wdrożenie:
```
truffle migrate --network ropsten
```

#### Mainnet (główna sieć Ethereum):
1. Skonfiguruj `truffle-config.js` z kluczem prywatnym.
2. Wdrożenie:
```
truffle migrate --network mainnet
```

### 11. Dodatkowe komendy
#### Reset migracji:
```
truffle migrate --reset --network development
```

#### Debugowanie transakcji:
```
truffle debug TRANSACTION_HASH
```

#### Sprawdzenie adresu kontraktu:
```
instance.address;
```

#### Sprawdzenie stanu blockchaina:
```
ganache-cli --db ./blockchain-data
```

### 12. Instalacja dodatkowych bibliotek
#### OpenZeppelin (gotowe kontrakty, np. ERC-20, ERC-721):
```
npm install @openzeppelin/contracts
```

### 13. Używanie web3.js
#### Instalacja:
```
npm install web3
```

#### Podstawowa interakcja:
```
const Web3 = require("web3");
const web3 = new Web3("http://127.0.0.1:7545");
```

### 14. Wyjście z konsoli Truffle
```
.exit
```


Do czego służą approve / allowance i dlaczego nie wystarczy zwykły transfer?
approve i allowance
Mechanizm approve (zatwierdzania) i allowance (limit) to część standardu ERC-20.
approve(spender, amount) oznacza: „Pozwalam adresowi spender wydać (przelać) w moim imieniu do amount moich tokenów”.
allowance(owner, spender) pokazuje, ile jeszcze spender może wydać z konta own