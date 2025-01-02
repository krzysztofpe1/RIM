// Ładujemy artefakty (skompilowane ABI + bytecode kontraktu) do sieci ganachowej
const SensorContract = artifacts.require("SensorContract");

module.exports = function (deployer) {
  deployer.deploy(SensorContract);
};
