// // SPDX-License-Identifier: MIT
// // pragma solidity ^0.7.6;

// // contract TemperatureStorage {
// //     int256 private temperature;
    

// //     // Funkcja zapisywania temperatury
// //     function setTemperature(int256 _temperature) public {
// //         temperature = _temperature;
// //     }

// //     // Funkcja odczytu temperatury
// //     function getTemperature() public view returns (int256) {
// //         return temperature;
// //     }
// // }


// // SPDX-License-Identifier: MIT
// pragma solidity ^0.7.6;

// interface IERC20 {
//     function totalSupply() external view returns (uint256);
//     function balanceOf(address account) external view returns (uint256);
//     function transfer(address recipient, uint256 amount) external returns (bool);
//     function allowance(address owner, address spender) external view returns (uint256);
//     function approve(address spender, uint256 amount) external returns (bool);
//     function transferFrom(address sender, address recipient, uint256 amount) external returns (bool);

//     event Transfer(address indexed from, address indexed to, uint256 value);
//     event Approval(address indexed owner, address indexed spender, uint256 value);
// }

// contract TemperatureStorageERC20 is IERC20 {
//     // ERC-20 Variables
//     string public name = "Temperature Token";
//     string public symbol = "TEMP";
//     uint8 public decimals = 18;
//     uint256 private _totalSupply;

//     mapping(address => uint256) private _balances;
//     mapping(address => mapping(address => uint256)) private _allowances;

//     // Temperature variable
//     int256 private temperature;

//     // Constructor to initialize total supply and assign it to deployer
//     constructor(uint256 initialSupply) {
//         _totalSupply = initialSupply * (10 ** uint256(decimals));
//         _balances[msg.sender] = _totalSupply;
//         emit Transfer(address(0), msg.sender, _totalSupply);
//     }

//     // ERC-20 Functions
//     function totalSupply() public view override returns (uint256) {
//         return _totalSupply;
//     }

//     function balanceOf(address account) public view override returns (uint256) {
//         return _balances[account];
//     }

//     function transfer(address recipient, uint256 amount) public override returns (bool) {
//         require(_balances[msg.sender] >= amount, "Insufficient balance");
//         _balances[msg.sender] -= amount;
//         _balances[recipient] += amount;
//         emit Transfer(msg.sender, recipient, amount);
//         return true;
//     }

//     // check if user has enough balance and has approved the amount to spend
//     function allowance(address owner, address spender) public view override returns (uint256) {
//         return _allowances[owner][spender];
//     }

//     // approve the amount to spend
//     function approve(address spender, uint256 amount) public override returns (bool) {
//         _allowances[msg.sender][spender] = amount;
//         emit Approval(msg.sender, spender, amount);
//         return true;
//     }

//     function transferFrom(address sender, address recipient, uint256 amount) public override returns (bool) {
//         require(_balances[sender] >= amount, "Insufficient balance");
//         require(_allowances[sender][msg.sender] >= amount, "Allowance exceeded");
        
//         _balances[sender] -= amount;
//         _balances[recipient] += amount;
//         _allowances[sender][msg.sender] -= amount;

//         emit Transfer(sender, recipient, amount);
//         return true;
//     }

//     // Temperature Functions
//     function setTemperature(int256 _temperature) public {
//         temperature = _temperature;
//     }

//     function getTemperature() public view returns (int256) {
//         return temperature;
//     }
// }


// SPDX-License-Identifier: MIT
// pragma solidity ^0.7.6;

// // import "@openzeppelin/contracts/token/ERC20/ERC20.sol";

// contract SensorRewardToken is ERC20 {
//     address public admin;

//     // Mapowanie do przypisywania adresów portfeli do sensorów
//     mapping(address => bool) public authorizedSensors;

//     constructor(uint256 initialSupply) ERC20("SensorRewardToken", "SRT") {
//         _mint(msg.sender, initialSupply); // Tworzenie początkowej podaży tokenów dla administratora
//         admin = msg.sender; // Ustawienie administratora
//     }

//     // Funkcja do autoryzacji sensorów
//     function authorizeSensor(address sensorAddress) external {
//         require(msg.sender == admin, "Only admin can authorize sensors");
//         authorizedSensors[sensorAddress] = true;
//     }

//     // Funkcja do nagradzania sensorów tokenami
//     function rewardSensor(address sensorAddress, uint256 amount) external {
//         require(authorizedSensors[sensorAddress], "Sensor is not authorized");
//         require(balanceOf(msg.sender) >= amount, "Not enough tokens to reward");
//         _transfer(msg.sender, sensorAddress, amount); // Przesyłanie tokenów
//     }
// }
