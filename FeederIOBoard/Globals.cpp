/*
  Global variables
*/
#include "Globals.h"

namespace Globals {

// Ticker/Scheduler
Ticker::Ticker Ticker;
// Watchdog for Safety
Watchdog::Watchdog WatchdogSafety;
// Led Blink
Blink::Blink BlinkLED;
// Control-command
Control::Control Controller;

// General config
bool DebugMode = false;
bool DoStreaming = false;

InternalConfig VolatileConfig;

// Analog inputs
uint32_t steer = 0, accel = 0, brake = 0, clutch = 0, buttons = 0;
// Velocity and accel of steering as 32bits float (will be converted to hex)
float steer_vel = 0.0, steer_acc = 0.0;
uint32_t prev_steer = 0;
float prev_vel = 0.0;

// Outputs
uint32_t fwdCmd; // 0/1
uint32_t revCmd; // 0/1
uint32_t torqueCmd; // value output to the PWM (analog out)
uint32_t lamps; // Bitfield

}
