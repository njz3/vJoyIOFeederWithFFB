/*
  Watchdog for safety output
  04/2020 Benjamin Maurin
*/
#include "Config.h"
#include "Watchdog.h"

namespace Watchdog {
  
void Watchdog::Check(uint32_t time)
{  
  // Watchdog management
  if (Enabled) {
    uint32_t diff = time - LastRefreshTick;
    if (diff>Timeout) {
      // Watchdog triggered !!

      // Disable watchdog
      Enabled = false;
      if (Trigger!=nullptr)
        Trigger();
    }
  }
}

}
