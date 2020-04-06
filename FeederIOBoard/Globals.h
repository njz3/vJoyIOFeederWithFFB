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

// Analog inputs
/*
extern uint32_t steer, accel, brake, clutch, buttons;
// Velocity and accel of steering as 32bits float (will be converted to hex)
extern float steer_vel, steer_acc;
extern uint32_t prev_steer;
extern float prev_vel;
*/
  
}
