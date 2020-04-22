/*
  CRC computations
*/
#pragma once
#include "Config.h"

namespace CRC {
  
unsigned crc8x_fast(unsigned crc, void const *mem, int len);

}
