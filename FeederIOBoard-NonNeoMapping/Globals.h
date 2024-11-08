/*
  Global variables
*/
#pragma once
#include "Config.h"
#include "Ticker.h"
#include "Watchdog.h"
#include "Blink.h"
#include "Control.h"

namespace Globals {

//const uint32_t timoffset_us = 0;// 4290000000ULL;
// Ticker/Scheduler
extern Ticker::Ticker Ticker;
// Watchdog for Safety
extern Watchdog::Watchdog WatchdogSafety;
// Led Blink
extern Blink::Blink BlinkLED;
// Control-command
extern Control::Control Controller;

// General non-permanent config
class InternalConfig {
  public:
  bool DebugMode;
  bool DoStreaming;
};

extern InternalConfig VolatileConfig;
  
}
