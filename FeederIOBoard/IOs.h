/*
  Management of configuration for IO board
*/
#pragma once
#include "Config.h"

namespace IOs {

int AnalogRead(int pin);
int AnalogReadFilter(int pin, uint16_t log2_extra_samples = 1);
void AnalogWrite(int pin, int value);

int DigitalRead(int pin);
int DigitalReadFilter(int pin, uint16_t nb_sample_debounce_filter = 2);
void DigitalWrite(int pin, int value);

uint32_t EncoderRead(int enc);

}
