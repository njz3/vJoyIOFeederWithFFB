/*
  Platform dependant code
  04/2020 Benjamin Maurin
*/

#include "PlatformSpecific.h"
#include "Globals.h"
#include "IOs.h"
#include "Protocol.h"


namespace PlatformSpecific {

// On Leonardo, use fast PWM on pin D9 and D10
#ifdef ARDUINO_AVR_LEONARDO
// Frequence PWM 15,6kHz (15655.57730Hz exactement, voir code ESPWheel d'Etienne)
// Valeur pwm entre 0 et 511
#define PWM_MAX    (511)
void InitPWM_D9_D10(uint32_t top_value);
// D9: pwm entre 0 et PWM_MAX
void SetPWM_D9(uint16_t pwm);
// D10: pwm entre 0 et PWM_MAX
void SetPWM_D10(uint16_t pwm);

void InitPWM_D9_D10(uint32_t top_value)
{
  // Set the frequency for timer1 (D9)
  // See https://github.com/pololu/zumo-shield/blob/master/ZumoMotors/ZumoMotors.cpp
  // or https://reso-nance.org/wiki/logiciels/arduino-timer/accueil
#if defined(__AVR_ATmega168__)|| defined(__AVR_ATmega328P__) || defined(__AVR_ATmega32U4__)
  // PWM frequency calculation : 16MHz / 1 (prescaler) / 2 (phase-correct) / 1000 (top) = 8 kHz
  // PWM frequency calculation : 16MHz / 1 (prescaler) / 2 (phase-correct) / 400 (top) = 20 kHz
  // PWM frequency calculation : 16MHz / 1 (prescaler) / 2 (phase-correct) / 512 (top) = 15,6kHz
  TCCR1A = 0b10100000;
  TCCR1B = 0b00010001;
  ICR1 = top_value; // Top value
  OCR1A = 0; // PWM mis à 0 sur D9
  OCR1B = 0; // PWM mis à 0 sur D10
#endif
}

// D9: pwm entre 0 et PWM_MAX
void SetPWM_D9(uint16_t pwm)
{
  if (pwm>PWM_MAX)
    pwm = PWM_MAX;
  OCR1A = pwm;
}

// D10: pwm entre 0 et PWM_MAX
void SetPWM_D10(uint16_t pwm)
{
  if (pwm>PWM_MAX)
    pwm = PWM_MAX;
  OCR1B = pwm;
}
#endif


void ControlCommand(Control::Control& controller)
{
  // Dual mode PWM or PWM+Dir?
  if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_DUAL)!=0) {
    // Dual mode on D9 and D10 for L620X
    int fwdPwm, revPwm, en;
    
    // Forward direction PWM?
    if (Globals::Controller.ForwardCmd) {
      fwdPwm = Globals::Controller.TorqueCmd; // 4095
    } else {
      fwdPwm = 0;
    }
    // Reverse direction PWM?
    if (Globals::Controller.ReverseCmd) {
      revPwm = Globals::Controller.TorqueCmd; // 4095
    } else {
      revPwm = 0;
    }
    
    // Enable/Disable
    if (Globals::Controller.ForwardCmd || Globals::Controller.ReverseCmd) {
      // Enable ON
      en = 1;
    } else {
      // Enable OFF
      en =0;
    }
    
    // Fast PWM on Leonardo on 9bits 0..511
    #ifdef ARDUINO_AVR_LEONARDO
      PlatformSpecific::SetPWM_D9(fwdPwm>>3); // 4095 shifted by 3 = 511
      PlatformSpecific::SetPWM_D10(revPwm>>3);
    #else
      // Arduino's analogWrite is limited to 0..255 8bits range
      IOs::AnalogWrite(FwdPWMPin, fwdPwm>>4); // 4095 shifted by 4 = 255
      IOs::AnalogWrite(RevPWMOrFwdDirPin, revPwm>>4);
    #endif
    
    IOs::DigitalWrite(EnableOrRevDirPin, en);
    
  } else {
    // PWM+Dir or centered PWM
    
    // torqueCmd is a 12bits integer 0..4096
    // Fast PWM on Leonardo on 9bits 0..511
    #ifdef ARDUINO_AVR_LEONARDO
      PlatformSpecific::SetPWM_D9(Globals::Controller.TorqueCmd>>3); // 4095 shifted by 3 = 511
    #else
      // Arduino's analogWrite is limited to 0..255 8bits range
      IOs::AnalogWrite(FwdPWMPin, Globals::Controller.TorqueCmd>>4);
    #endif
  
  // Digital PWM for Aganyte FFB Converter - Must use PWM centered mode
  #ifdef FFB_CONVERTER_DIG_PWM
    int FFBConverter_digitalTorqueCmd = Globals::Controller.TorqueCmd-0x800; // Value between -2047..2047
    // Now do whatever ...
    //Serial2.Write(FFBConverter_digitalTorqueCmd);
  #endif
  
    // Direction/Disable
    IOs::DigitalWrite(RevPWMOrFwdDirPin, Globals::Controller.ForwardCmd);
    IOs::DigitalWrite(EnableOrRevDirPin, Globals::Controller.ReverseCmd);
  }

}

void Analog(Control::Control& controller)
{
  // For all (Uno, Mega, Leonardo, Due), make it into 0..4095
  if ((Config::ConfigFile.WheelMode & CONFIG_WHEELMODE_FILTER)!=0) {
    controller.Steer = IOs::AnalogReadFilter(analogInSteeringPin, 3);
  } else {
    controller.Steer = IOs::AnalogRead(analogInSteeringPin);  
  }
  controller.Accel = IOs::AnalogRead(analogInAccelPin);
  controller.Brake = IOs::AnalogRead(analogInBrakePin);
  controller.Clutch = IOs::AnalogRead(analogInClutchPin);

  // Compute vel&acc in float - this is a difficult task for our poors arduinos!
  // Analog values are not stable (you should add a capacitor)
  int32_t diff_steer = (int32_t)controller.Steer - (int32_t)controller.PrevSteer;
  if (diff_steer>0x200 || diff_steer<(-0x200)) {
    Protocol::DebugMessageFrame("MJump in position! Freezing vel&acc, diff=" + String(diff_steer));
  } else {
    // Should use a predictive observer for speed&accel given pwm control as accel
    // Do not forget to scale by tick frequency to get unit per [s]
    controller.SteerVel = ((float)diff_steer)*(TICK_HZ);
    controller.SteerAcc = (controller.SteerVel - controller.PrevSteerVel)*(TICK_HZ);
  }
  controller.PrevSteer = controller.Steer;
  controller.PrevSteerVel = controller.SteerVel;
  
}

void Buttons(Control::Control& controller)
{
  int btn1 = !IOs::DigitalRead(DInBtn1Pin);
  int btn2 = !IOs::DigitalRead(DInBtn2Pin);
  int btn3 = !IOs::DigitalRead(DInBtn3Pin);
  int btn4 = !IOs::DigitalRead(DInBtn4Pin);
  
  int btn5 = !IOs::DigitalRead(DInBtn5Pin);
  int btn6 = !IOs::DigitalRead(DInBtn6Pin);
  int btn7 = !IOs::DigitalRead(DInBtn7Pin);
  int btn8 = !IOs::DigitalRead(DInBtn8Pin);

  int btn9 = !IOs::DigitalRead(DInBtn9Pin);
  int btn10 = !IOs::DigitalRead(DInBtn10Pin);
  int btn11 = !IOs::DigitalRead(DInBtn11Pin);
  int btn12 = !IOs::DigitalRead(DInBtn12Pin);
  
  controller.Buttons = 
    (btn1<<0) + (btn2<<1) + (btn3<<2) + (btn4<<3) +
    (btn5<<4) + (btn6<<5) + (btn7<<6) + (btn8<<7) +
    (btn9<<8) + (btn10<<9) + (btn11<<10) + (btn12<<11);

}

void Lamps(Control::Control& controller)
{
  IOs::DigitalWrite(DOutLStartPin, (controller.Lamps>>0)&1);
  IOs::DigitalWrite(DOutLView1Pin, (controller.Lamps>>1)&1);
  IOs::DigitalWrite(DOutLView2Pin, (controller.Lamps>>2)&1);
  IOs::DigitalWrite(DOutLView3Pin, (controller.Lamps>>3)&1);
  IOs::DigitalWrite(DOutLView4Pin, (controller.Lamps>>4)&1);
  IOs::DigitalWrite(DOutLLeaderPin,(controller.Lamps>>5)&1);
}

void SetupBoard()
{
  
#ifdef ARDUINO_AVR_LEONARDO
  #if FASTADC
  // set prescale to 64: 1 1 0 (below issues with crosstalk)
  sbi(ADCSRA,ADPS2) ;
  sbi(ADCSRA,ADPS1) ;
  cbi(ADCSRA,ADPS0) ;
  #endif
  // Fast PWM at 15,6kHz on D9 with 0..512 range
  InitPWM_D9_D10(PWM_MAX);
#endif

#ifdef ARDUINO_AVR_MEGA2560
  // D9 pwm@3.9kHz
  //TCCR2B = TCCR2B & B11111000 | B00000010;
#endif

  // For Due, zero, enforce analog read to be 0..4095 (0xFFF)
#ifdef ARDUINO_ARCH_SAM
  analogReadResolution(12);
#endif

  // Potentiometers
  pinMode(analogInSteeringPin, INPUT);
  pinMode(analogInAccelPin, INPUT);
  pinMode(analogInBrakePin, INPUT);
  pinMode(analogInClutchPin, INPUT);

  // Buttons
  pinMode(DInBtn1Pin, INPUT_PULLUP);
  pinMode(DInBtn2Pin, INPUT_PULLUP);
  pinMode(DInBtn3Pin, INPUT_PULLUP);
  pinMode(DInBtn4Pin, INPUT_PULLUP);

  pinMode(DInBtn5Pin, INPUT_PULLUP);
  pinMode(DInBtn6Pin, INPUT_PULLUP);
  pinMode(DInBtn7Pin, INPUT_PULLUP);
  pinMode(DInBtn8Pin, INPUT_PULLUP);

  pinMode(DInBtn9Pin, INPUT_PULLUP);
  pinMode(DInBtn10Pin, INPUT_PULLUP);
  pinMode(DInBtn11Pin, INPUT_PULLUP);
  pinMode(DInBtn12Pin, INPUT_PULLUP);

  // Dual PWM+enable OR PWM+Dir
  pinMode(FwdPWMPin, OUTPUT); // Forward fast PWM pin on D9
  pinMode(RevPWMOrFwdDirPin, OUTPUT); // Reverse fast PWM pin on D10 OR forward dir
  pinMode(EnableOrRevDirPin, OUTPUT); // Enable drive OR reverse dir

  pinMode(DOutLEDPin, OUTPUT); // Led
  
#ifdef ARDUINO_AVR_MEGA2560
  // Mega pins 22-29 : 8x digital outputs for driveboard RX
  DDRA = 0xFF;
  PORTA = 0; // All outpus set to 0
  // Mega pins 30-37 : 8x digital inputs with pull-up for driveboard TX
  DDRC = 0x0;
  PORTC = 0xFF; // Activate internal pull-up resistors
  // Lamps
  pinMode(DOutLStartPin, OUTPUT);
  pinMode(DOutLView1Pin, OUTPUT);
  pinMode(DOutLView2Pin, OUTPUT);
  pinMode(DOutLView3Pin, OUTPUT);
  pinMode(DOutLView4Pin, OUTPUT);
  pinMode(DOutLLeaderPin, OUTPUT);
#endif


  Globals::Controller.SetTorqueOut(ControlCommand);
  Globals::Controller.SetAnalogRead(Analog);
  Globals::Controller.SetButtonsRead(Buttons);
  Globals::Controller.SetLampOut(Lamps);
}

void Do_Init()
{
  delay(100);  
}

}
