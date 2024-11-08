/*
 Simple blink class
 04/2020 Benjamin Maurin
*/
#pragma once
#include <Arduino.h>

namespace Blink {

typedef void (*BlinkFct)(bool);

class Blink
{
  public:
  byte Enabled = true;
  int16_t BlinkPeriod = 10;
    
  void SetEvent(BlinkFct blink) { Blink = blink; };
  void Refresh();

protected:
  // Etat du blink
  byte BlinkState = false;
  // Compteur de tick pour le blink
  int16_t blinktick_cnt = 0;
  // Event
  BlinkFct Blink;
};

}
