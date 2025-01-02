// SPDX-License-Identifier: MIT
pragma solidity ^0.8.10;

interface IERC20 {
    /**
     * @dev Przesyła określoną ilość tokenów (`value`) na adres `to`.
     * Musi zwrócić `true`, jeśli transakcja się powiedzie.
     * @param to Adres odbiorcy tokenów.
     * @param value Liczba tokenów do przesłania.
     * @return Zwraca `true`, jeśli operacja się powiedzie.
     */
    function transfer(address to, uint256 value) external returns (bool);

    /**
     * @dev Zwraca saldo tokenów znajdujące się na koncie danego adresu (`who`).
     * @param who Adres, dla którego chcemy sprawdzić saldo.
     * @return Liczba tokenów należąca do podanego adresu.
     */
    function balanceOf(address who) external view returns (uint256);

    /**
     * @dev Sprawdza, ile tokenów właściciel (`owner`) zatwierdził do użycia przez adres (`spender`).
     * @param tokenOwner Adres właściciela tokenów.
     * @param spender Adres, który ma prawo do wykorzystania tokenów.
     * @return Liczba tokenów, które adres `spender` może wydać.
     */
    function allowance(address tokenOwner, address spender) external view returns (uint256);

    /**
     * @dev Zatwierdza adresowi (`spender`) możliwość wydania określonej liczby tokenów (`value`) 
     * w imieniu właściciela kontraktu.
     * Musi zwrócić `true`, jeśli operacja się powiedzie.
     * @param spender Adres, który będzie mógł wydać tokeny.
     * @param value Liczba tokenów do zatwierdzenia.
     * @return Zwraca `true`, jeśli operacja się powiedzie.
     */
    function approve(address spender, uint256 value) external returns (bool);

    /**
     * @dev Przesyła tokeny z jednego adresu (`from`) na inny (`to`), jeśli zostały wcześniej zatwierdzone.
     * Musi zwrócić `true`, jeśli operacja się powiedzie.
     * @param from Adres, z którego tokeny są przesyłane.
     * @param to Adres, na który tokeny są przesyłane.
     * @param value Liczba tokenów do przesłania.
     * @return Zwraca `true`, jeśli operacja się powiedzie.
     */
    function transferFrom(address from, address to, uint256 value) external returns (bool);

    /**
     * @dev Zwraca całkowitą liczbę tokenów dostępnych w systemie.
     * @return Całkowita liczba tokenów w obiegu.
     */
    function totalSupply() external view returns (uint256);
}

contract SensorContract is IERC20 {
    string public name = "Sensor Token";
    string public symbol = "SEN";
    uint8 public decimals = 18; // ile bitow reprezentuje ten token
    uint256 public totalSupply = 1000000 * 10 ** uint256(decimals);
    address public owner;
    mapping(address => uint256) public balances;
    mapping(address => mapping(address => uint256)) public allowed; // mapping  owner kontraktu => (klucz klienta  => wartość)

    constructor() {
        owner = msg.sender;
        balances[owner] = totalSupply;
    }

    function transfer(address to, uint256 value) public override returns (bool) {
        require(balances[msg.sender] >= value, "Insufficient balance");
        balances[msg.sender] -= value;
        balances[to] += value;
        emit Transfer(msg.sender, to, value);
        return true;
    }

    function balanceOf(address who) public view override returns (uint256) {
        return balances[who];
    }

    function allowance(address tokenOwner, address spender) public view override returns (uint256) {
        return allowed[tokenOwner][spender];
    }

    function approve(address spender, uint256 value) public override returns (bool) {
        allowed[msg.sender][spender] = value;
        emit Approval(msg.sender, spender, value);
        return true;
    }

    function transferFrom(address from, address to, uint256 value) public override returns (bool) {
        require(balances[from] >= value, "Insufficient balance");
        require(allowed[from][msg.sender] >= value, "Not allowed to spend");
        balances[from] -= value;
        balances[to] += value;
        allowed[from][msg.sender] -= value;
        emit Transfer(from, to, value);
        return true;
    }

    function rewardSensor(address sensor, uint256 amount) external onlyOwner 
    {
    require(sensor != address(0), "Invalid sensor address");
    require(balances[owner] >= amount, "Insufficient tokens in the contract");
    balances[owner] -= amount;
    balances[sensor] += amount;
    emit Transfer(owner, sensor, amount);
    }

    modifier onlyOwner() {
        require(msg.sender == owner);
        _;
    }
    event Transfer(address indexed from, address indexed to, uint256 value);
    event Approval(address indexed owner, address indexed spender, uint256 value);
}
