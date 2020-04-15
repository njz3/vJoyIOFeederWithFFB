/*
 Simple blink class
 04/2020 Benjamin Maurin
*/
#include "Blink.h"

namespace Blink {

void Blink::Refresh()
{
  if (!Enabled)
    return;
  blinktick_cnt--;
  //Counter elapsed ?
  if (blinktick_cnt <= 0) {
    // Reset counter
    blinktick_cnt = BlinkPeriod;
    // Blink
    if (Blink!=nullptr)
      Blink(BlinkState);
    // Flip state
    BlinkState = !BlinkState;
  }
}

}
