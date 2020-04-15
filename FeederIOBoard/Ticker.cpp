/*
  IOBoard for Analog&Dig inputs, analog&Dig outputs
  - "Simple" serial comm
  - Watchdog for safety output
  
  Taken from http://www.arduino.cc/en/Tutorial/AnalogInOutSerial
  01/2020 Benjamin Maurin
*/
#include "Ticker.h"

namespace Ticker {
  
void Ticker::Init()
{
  Nexttick_us = micros() + TickPeriod_us + TimeOffset_us;
}

void Ticker::Refresh()
{
  // Increase tick
  Tick++;
  // Compute time in us until next tick -- will roll-over
  Nexttick_us += TickPeriod_us;
  // Time of execution for scheduler -- will roll-over
  unsigned long timesched_us = micros() + TimeOffset_us;
  // Get difference, this cancels possible roll-overs
  long delay_us = Nexttick_us - timesched_us;
  if (delay_us>0) {
    // If delay, pause mcu
    if (delay_us<10000) {
      delayMicroseconds(delay_us);
    } else {
      delay(delay_us/1000);
    }
  } else {
    // Overrun! Immediatly execute overrun and tick will follow
    Overrun(*this);
  }
  // Save current time
  TimeNow_us = micros() + TimeOffset_us;
  
  // Execute ticker
  if (Ticker!=nullptr)
    Ticker(*this);
  
  // Compute duration
  Duration_us = micros() + TimeOffset_us - TimeNow_us;
  
  // Remaining time left for Arduino's internal job!
}

}
