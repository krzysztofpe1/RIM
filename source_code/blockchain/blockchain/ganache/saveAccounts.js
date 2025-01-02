const fs = require('fs');
const logFile = '/app/ganache-output.log';

const checkLogFile = () => {
  if (fs.existsSync(logFile)) {
    fs.readFile(logFile, 'utf8', (err, data) => {
      if (err) {
        console.error('Error reading log file:', err);
        return;
      }

      const accounts = [];
      const privateKeys = [];

      const accountRegex = /\((\d+)\)\s+(0x[a-fA-F0-9]{40})/g;
      const privateKeyRegex = /\((\d+)\)\s+(0x[a-fA-F0-9]{64})/g;

      let match;
      while ((match = accountRegex.exec(data)) !== null) {
        accounts.push({ index: match[1], address: match[2] });
      }

      while ((match = privateKeyRegex.exec(data)) !== null) {
        privateKeys.push({ index: match[1], privateKey: match[2] });
      }

      const result = accounts.map((acc, index) => ({
        account: acc.address,
        privateKey: privateKeys[index]?.privateKey || null,
      }));

      fs.writeFileSync('/app/accounts.json', JSON.stringify(result, null, 2));
      console.log('Dane kont zapisano do /app/accounts.json');
    });
  } else {
    console.log('Log file not found. Retrying...');
    setTimeout(checkLogFile, 1000); // Spróbuj ponownie za 1 sekundę
  }
};

// Rozpocznij sprawdzanie pliku logów
checkLogFile();
