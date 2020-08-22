/*
  Management of configuration for IO board
*/
#include "Config.h"
#include <EEPROM.h>
#include "CRC.h"

namespace Config {

EEPROM_CONFIG ConfigFile;

int SaveConfigToEEPROM()
{
  if (EEPROM.length()<sizeof(EEPROM_CONFIG)) {
    return -1;
  }
  // Pointer to record
  byte* pBlock = (byte*)&ConfigFile;  
  // Compute CRC8 to detect wrong eeprom data
  byte crc8 = CRC::crc8x_fast(0, pBlock+1, sizeof(EEPROM_CONFIG)-1);
  // Update CRC in record
  ConfigFile.CRC8 = crc8;
  // Write record to EEPROM
  for (int i = 0 ; i < (int)sizeof(EEPROM_CONFIG); i++) {
    EEPROM.write(i, pBlock[i]);
  }
  return 1;
}

int LoadConfigFromEEPROM()
{
  if (EEPROM.length()<sizeof(EEPROM_CONFIG)) {
    return -1;
  }
  // Pointer to a new record on MCU stack
  EEPROM_CONFIG newCfg;
  byte* pBlock = (byte*)&newCfg;
  // Read new record from EEPROM
  for (int i = 0 ; i < (int)sizeof(EEPROM_CONFIG); i++) {
    pBlock[i] = EEPROM.read(i);
  }
  // Compute CRC8 to detect wrong eeprom data
  byte crc8 = CRC::crc8x_fast(0, pBlock+1, sizeof(EEPROM_CONFIG)-1);
  // Check CRC match?
  if (crc8 != newCfg.CRC8) {
    // Wrong CRC
    return -2;
  }
  // Ok, store new config
  ConfigFile = newCfg;
  return 1;
}

void ResetConfig()
{
  ConfigFile.PWMMode = 0;
  ConfigFile.SerialSpeed = PCSERIAL_BAUDRATE;
  ConfigFile.WheelMode = CONFIG_WHEELMODE_FILTER;
  ConfigFile.PedalMode = 0;
  ConfigFile.FFBController = 0;
}

}
