const TemperatureStorage = artifacts.require("TemperatureStorage");

module.exports = function (deployer) {
  deployer.deploy(TemperatureStorage);
};