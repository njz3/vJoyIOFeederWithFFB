/*
  Watchdog for safety output
  04/2020 Benjamin Maurin
*/
#pragma once
#include "Config.h"

namespace Watchdog {

typedef void (*TriggerFct)(void);

class Watchdog
{
  public:
  byte Enabled = false;
  uint32_t Timeout = 10;
  
  void SetEvent(TriggerFct trigger) { Trigger = trigger; };
  void Revive(uint32_t time) { LastRefreshTick = time; };
  void Check(uint32_t time);

protected:
  uint32_t LastRefreshTick = 0;
  // Event
  TriggerFct Trigger;
};

}
