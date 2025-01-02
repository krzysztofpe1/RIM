const { spawn } = require('child_process');
const fs = require('fs');

const ganache = spawn('ganache', [
  '--port', '7545',
  '--accounts', '10',
  '--defaultBalanceEther', '200',
  '--chain.chainId', '581234',
  '--chain.networkId', '581234'
]);

let output = '';

ganache.stdout.on('data', (data) => {
  output += data.toString();
});

ganache.stderr.on('data', (data) => {
  console.error(`Ganache error: ${data}`);
});

ganache.on('close', (code) => {
  console.log(`Ganache exited with code ${code}`);
  // Parsowanie wyjścia
  const accounts = [];
  const privateKeys = [];
  
  const accountRegex = /\((\d+)\)\s+(0x[a-fA-F0-9]{40})/g;
  const privateKeyRegex = /\((\d+)\)\s+(0x[a-fA-F0-9]{64})/g;

  let match;
  while ((match = accountRegex.exec(output)) !== null) {
    accounts.push({ index: match[1], address: match[2] });
  }

  while ((match = privateKeyRegex.exec(output)) !== null) {
    privateKeys.push({ index: match[1], privateKey: match[2] });
  }

  // Zapis danych do pliku
  const data = accounts.map((acc, index) => ({
    account: acc.address,
    privateKey: privateKeys[index]?.privateKey || null,
  }));

  fs.writeFileSync('accounts.json', JSON.stringify(data, null, 2));
  console.log('Dane kont zapisano do accounts.json');
});
