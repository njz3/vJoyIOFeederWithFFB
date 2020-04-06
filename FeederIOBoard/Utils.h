/*
  Various utilities
  04/2020 Benjamin Maurin
*/
#pragma once
#include "Config.h"

// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif


namespace Utils {

void ConvertToNDigHex(uint32_t value, uint32_t N = 2, char hex[] = NULL);
uint32_t ConvertHexToInt(char hex[], int N = 2);
void SoftwareReboot();
String GetValue(String data, char separator, int index);

}
