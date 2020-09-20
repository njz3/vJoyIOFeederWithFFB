/*
  Global variables
*/
#pragma once
#include "Config.h"

namespace Control {
  
class Control;
typedef void (*TorqueOutFct)(Control&);
typedef void (*AnalogReadFct)(Control&);
typedef void (*ButtonReadFct)(Control&);
typedef void (*LampOutFct)(Control&);
typedef void (*KeypadBtnsReadFct)(Control&);

class Control {
  public:
  uint32_t Steer = 0;      // Analog 0..4095 (0xFFF)
  uint32_t PrevSteer = 0;  // Analog 0..4095 (0xFFF)
  uint32_t Accel = 0;      // Analog 0..4095 (0xFFF)
  uint32_t Brake = 0;      // Analog 0..4095 (0xFFF)
  uint32_t Clutch = 0;     // Analog 0..4095 (0xFFF)
  uint32_t Buttons = 0;    // Bitfield
  uint32_t KeypadBtns = 0; // Bitfield
  uint32_t Encoder = 0;    // Quad encoder

  float SteerVel;      // float [cts/s]
  float PrevSteerVel;  // float [cts/s]
  float SteerAcc;      // float [cts/s2]
  
  uint32_t ForwardCmd = 0; // 0/1
  uint32_t ReverseCmd = 0; // 0/1
  uint32_t TorqueCmd = 0;  // PWM output value 0..4095 (0xFFF)

  uint32_t Lamps = 0;      // Bitfield

  void ResetCommands(int pwmmode);
  void SetTorqueOut(TorqueOutFct torqueOut) { TorqueOut = torqueOut; };
  void SetAnalogRead(AnalogReadFct analog) { AnalogRead = analog; };
  void SetButtonsRead(ButtonReadFct button) { ButtonsRead = button; };
  void SetLampOut(LampOutFct lamp) { LampOut = lamp; };
  void SetKeypadButtonsRead(KeypadBtnsReadFct keypad) { KeypadBtnsRead = keypad; };
  void Refresh();

  protected:
  // Event
  TorqueOutFct TorqueOut;
  AnalogReadFct AnalogRead;
  ButtonReadFct ButtonsRead;
  LampOutFct LampOut;
  KeypadBtnsReadFct KeypadBtnsRead;
};
  
}
