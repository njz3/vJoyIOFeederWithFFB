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

uint32_t ConvertHexToInt(const char hex[], int N)
{
  int i;
  uint32_t value = 0;
  for(i=0; i<N; i++) {
    char valhex;
    if (hex[i]>='0' && hex[i]<='9')
      valhex = hex[i]-'0';
    else if (hex[i]>='a' && hex[i]<='f')
      valhex = hex[i]-'a'+0xA;
    else if (hex[i]>='A' && hex[i]<='F')
      valhex = hex[i]-'A'+0xA;
    else {
      // Probably end of string.
      valhex = 0;
      break;
    }
    uint32_t nibble = (uint32_t)(valhex&0xF); // Récupère le nibble 'i'
    value = nibble + (value<<4);
  }
  return value;
}

byte ReadByteValue(char *sc)
{
  return (uint16_t)Utils::ConvertHexToInt(sc, 2);
}
unsigned char ReadUINT8Value(char *sc)
{
  return (unsigned char)Utils::ConvertHexToInt(sc, 2);
}
unsigned short ReadUINT16Value(char *sc)
{
  return (unsigned char)Utils::ConvertHexToInt(sc, 4);
}

// We need to be able to handle both MSB First and MSB Last
// which means we need to flip the byte order, this is a space-efficient
// way to do this ( http://omarfrancisco.com/reversing-bits-in-a-byte/ )
#ifdef ARDUINO_AVR_MEGA2560
uint8_t reverseByte(uint8_t x)
{
  asm(
      "ldi        r25, 0x80\n" // Set r25 to 0b10000000
      "1:\n"                   // Label 1
      "rol        %0\n"        // Rotate x left 1 bit, bit 7 goes into carry and others move up
      "ror        r25\n"       // Rotate output (r25) right 1 bit, carry goes into bit 7, bit 0 goes into carry, others shift down
      "brcc       1b\n"        // If the carry is stil zero, loop to 1: (r25 started as 0b10000000 and those zeros a rippling down)
      "mov        %0, r25\n"   // Put output back into x
      : "=r" (x) /* Out */
      : "0"  (x) /* In  */
      : "r25"    /* Clobber */
   );
   return x;
}
#endif

// Reset function using the avr watchdog
void SoftwareReboot()
{
#ifdef ARDUINO_AVR_LEONARDO
  // Use hardware watchdog for Leonardo
  wdt_enable(WDTO_15MS);
  while(1) {  }
#endif
#ifdef ARDUINO_AVR_MEGA2560
  // Use branch to address 0 to restart the code
  void(* resetFunc) (void) = 0;//declare reset function at address 0
  resetFunc();
#endif
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
