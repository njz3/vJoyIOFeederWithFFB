/*
  Management of configuration for IO board
*/
#include "IOs.h"

namespace IOs {

int AnalogRead(int pin)
{
#ifdef ARDUINO_ARCH_SAMD
  // For Due, zero, full 12 bits resolution 0..4095
  return analogRead(pin);
#else
  // For all (Uno, Mega, Leonardo), only 10bits shift by 2 to make it into 0..4095
  // Trash first reading to get a stable value
  //int a = analogRead(pin);
  return analogRead(pin)<<2;
#endif
}

int AnalogReadFilter(int pin, uint16_t log2_extra_samples)
{
  uint32_t cumulative_value = 0;
  for(int i=0; i<(1<<log2_extra_samples); i++) { 
    cumulative_value += AnalogRead(pin);
  }
  return (int)(cumulative_value>>log2_extra_samples);
}

void AnalogWrite(int pin, int value)
{
  // Arduino's analogWrite is limited to 0..255 8bits range
  analogWrite(pin, value);
}


int DigitalRead(int pin)
{
  return digitalRead(pin);
}

uint8_t filterpin_state[64];
int DigitalReadFilter(int pin, uint16_t nb_sample_debounce_filter)
{
  int val = 0;
  uint16_t count = 0;
  for(uint16_t i=0; i<nb_sample_debounce_filter; i++) {
    if (i==0) {
      val = DigitalRead(pin);
        count++;
    } else {
      if (val==DigitalRead(pin)) {
        count++;
      }
    }
  }
  if (count==nb_sample_debounce_filter) {
    filterpin_state[pin] = val;
  }
  return (int)filterpin_state[pin];
}

void DigitalWrite(int pin, int value)
{
  digitalWrite(pin, value);
}

uint32_t EncoderRead(int enc)
{
  return 0;
}

}
