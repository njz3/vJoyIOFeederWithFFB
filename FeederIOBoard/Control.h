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

class Control {
  public:
  uint32_t Steer;      // Analog 0..4095 (0xFFF)
  uint32_t PrevSteer;  // Analog 0..4095 (0xFFF)
  uint32_t Accel;      // Analog 0..4095 (0xFFF)
  uint32_t Brake;      // Analog 0..4095 (0xFFF)
  uint32_t Clutch;     // Analog 0..4095 (0xFFF)
  uint32_t Buttons;    // Bitfield
  uint32_t Encoder;    // Quad encoder

  float SteerVel;      // float [cts/s]
  float PrevSteerVel;  // float [cts/s]
  float SteerAcc;    // float [cts/s2]
  
  uint32_t ForwardCmd; // 0/1
  uint32_t ReverseCmd; // 0/1
  uint32_t TorqueCmd;  // PWM output value 0..4095 (0xFFF)

  uint32_t Lamps;      // Bitfield

  void ResetCommands(int pwmmode);
  void SetTorqueOut(TorqueOutFct torqueOut) { TorqueOut = torqueOut; };
  void SetAnalogRead(AnalogReadFct analog) { AnalogRead = analog; };
  void SetButtonsRead(ButtonReadFct button) { ButtonsRead = button; };
  void SetLampOut(LampOutFct lamp) { LampOut = lamp; };
  void Refresh();

  protected:
  // Event
  TorqueOutFct TorqueOut;
  AnalogReadFct AnalogRead;
  ButtonReadFct ButtonsRead;
  LampOutFct LampOut;
};
  
}
