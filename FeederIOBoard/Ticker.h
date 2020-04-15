/*
  IOBoard for Analog&Dig inputs, analog&Dig outputs
  - "Simple" serial comm
  - Watchdog for safety output
  
  Taken from http://www.arduino.cc/en/Tutorial/AnalogInOutSerial
  01/2020 Benjamin Maurin
*/
#pragma once
#include <Arduino.h>

namespace Ticker {

class Ticker;
typedef void (*TickerFct)(Ticker&);
typedef void (*OverrunFct)(Ticker&);

class Ticker
{
  public:
  uint32_t TickPeriod_us = 50000;
  uint32_t Tick = 0;
  uint32_t TimeOffset_us = 0;
  uint32_t TimeNow_us = 0;
  uint32_t Duration_us = 0;
  
  void Init();
  void SetEvent(TickerFct ticker) { Ticker = ticker; };
  void SetOverrun(OverrunFct overrun) { Overrun = overrun; };
  void Refresh();
  
  protected:
  uint32_t Nexttick_us = 0;
  // Event
  TickerFct Ticker;
  OverrunFct Overrun;
};

}
