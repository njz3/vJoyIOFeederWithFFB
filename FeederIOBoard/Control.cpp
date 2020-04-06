/*
  Global variables
*/
#include "Control.h"
#include "Globals.h"

namespace Control {
  
void Control::ResetCommands(int pwmmode)
{
#ifdef FFB_CONVERTER_DIG_PWM
  TorqueCmd = 0x800; // Centered PWM  
#else
  // Centered PWM?
  if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_CENTERED)!=0) {
    TorqueCmd = 0x800; // Centered PWM
  } else {
    TorqueCmd = 0; // PWM+Dir
  }
#endif

  ForwardCmd = 0; // Disable output
  ReverseCmd = 0;
}

void Control::Refresh()
{
  // Control-commands
  if (TorqueOut!=nullptr)   TorqueOut(*this);
  // Analog inputs : steering, pedals
  if (AnalogRead!=nullptr)  AnalogRead(*this);
  // Buttons  
  if (ButtonsRead!=nullptr) ButtonsRead(*this);
  // Lamps
  if (LampOut!=nullptr)     LampOut(*this);
}

}
