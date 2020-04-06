/*
  Various utilities
  04/2020 Benjamin Maurin
*/

#include "Utils.h"

#include <avr/wdt.h>

namespace Utils {
  
char nibbletable[] = "0123456789ABCDEFX";

void ConvertToNDigHex(uint32_t value, uint32_t N, char hex[])
{
  int32_t i;
  for(i=N-1; i>=0; i--) {
    uint32_t nibble = value & 0xF; // Récupère le nibble 'i'
    hex[i] = nibbletable[nibble];
    value = value>>4;
  }
  hex[N] = 0;
}

uint32_t ConvertHexToInt(char hex[], int N)
{
  int i;
  uint32_t value = 0;
  for(i=0; i<N; i++) {
    char valhex;
    if (hex[1]>='0' && hex[i]<='9')
      valhex = hex[i]-'0';
    else if (hex[i]>='A' && hex[i]<='F')
      valhex = hex[i]-'A'+0xA;
    else 
      valhex = 0;
    uint32_t nibble = (uint32_t)(valhex&0xF); // Récupère le nibble 'i'
    value = nibble + (value<<4);
  }
  return value;
}

// Reset function using the avr watchdog
void SoftwareReboot()
{
  wdt_enable(WDTO_15MS);
  while(1) {  }
}

String GetValue(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length()-1;

  for(int i=0; i<=maxIndex && found<=index; i++){
    if(data.charAt(i)==separator || i==maxIndex){
        found++;
        strIndex[0] = strIndex[1]+1;
        strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }

  return found>index ? data.substring(strIndex[0], strIndex[1]) : "";
}


}
