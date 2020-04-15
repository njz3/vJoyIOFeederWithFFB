/*
  Management of configuration for IO board
*/
#include "Config.h"
#include <EEPROM.h>

namespace Config {

EEPROM_CONFIG ConfigFile;

int SaveConfigToEEPROM()
{
  if (EEPROM.length()<sizeof(EEPROM_CONFIG)) {
    return -1;
  }
  
  byte* pBlock = (byte*)&ConfigFile;
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
  byte* pBlock = (byte*)&ConfigFile;
  for (int i = 0 ; i < (int)sizeof(EEPROM_CONFIG); i++) {
    pBlock[i] = EEPROM.read(i);
  }
  return 1;
}

void ResetConfig()
{
  ConfigFile.PWMMode = 0;
  ConfigFile.SerialSpeed = PCSERIAL_BAUDRATE;
  ConfigFile.WheelMode = CONFIG_WHEELMODE_FILTER;
  ConfigFile.PedalMode = 0;
}

}
