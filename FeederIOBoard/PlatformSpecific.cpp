/*
  Platform dependant code
  04/2020 Benjamin Maurin
*/

#include "PlatformSpecific.h"
#include "Globals.h"
#include "IOs.h"
#include "Protocol.h"


namespace PlatformSpecific {

//-----------------------------------------------------------------------------
// Default IOs pin mapping
//-----------------------------------------------------------------------------

int analogInSteeringPin = A0;  // Analog input pin that the potentiometer is attached to
int analogInAccelPin    = A1;  // Analog input pin that the potentiometer is attached to
int analogInBrakePin    = A2;  // Analog input pin that the potentiometer is attached to
int analogInClutchPin   = A3;  // Analog input pin that the potentiometer is attached to

int FwdPWMPin           = D9;  // Analog output pin for forward PWM (dual PWM) or centered PWM
int RevPWMOrFwdDirPin   = D10; // Analog output pin for reverse PWM (dual PWM) or forward dir (PWM+Dir)
int EnableOrRevDirPin   = D11; // digital output pin for enable or reverse dir (PWM+Dir)

int DOutLEDPin          = D13; // Analog output pin that the LED is attached to

#ifdef ARDUINO_AVR_MEGA2560
// M2PAC pinout, but order as of sega's lamp byte
int DOutLCoin1Pin       = A9; // digital output
int DOutLCoin2Pin       = A15; // digital output
int DOutLStartPin       = A10; // digital output
int DOutLView1Pin       = A11; // digital output
int DOutLView2Pin       = A12; // digital output
int DOutLView3Pin       = A13; // digital output
int DOutLView4Pin       = A14; // digital output
int DOutLLeaderPin      = A8; // digital output
#endif

// Common buttons pinout
int DInBtn1Pin          = D2; // digital input
int DInBtn2Pin          = D3; // digital input
int DInBtn3Pin          = D4; // digital input
int DInBtn4Pin          = D5; // digital input

int DInBtn5Pin          = D6; // digital input
int DInBtn6Pin          = D7; // digital input
int DInBtn7Pin          = D8; // digital input
int DInBtn8Pin          = D12; // digital input


#if defined(ARDUINO_AVR_LEONARDO)
// Specific Leonardo buttons pinout
int DInBtn9Pin          = A4; // digital input
int DInBtn10Pin         = A5; // digital input
int DInBtn11Pin         = D0; // digital input - only when not using digital PWM
int DInBtn12Pin         = D1; // digital input - only when not using digital PWM
#elif defined(ARDUINO_AVR_MEGA2560)
// Common Mega 2560 buttons pinout
int DInBtn9Pin          = D38; // digital input
int DInBtn10Pin         = D39; // digital input
int DInBtn11Pin         = D40; // digital input
int DInBtn12Pin         = D41; // digital input
int DInBtn13Pin         = D50; // digital input
int DInBtn14Pin         = D51; // digital input
int DInBtn15Pin         = D52; // digital input
int DInBtn16Pin         = D53; // digital input
#else
int DInBtn9Pin          = A4; // digital input
int DInBtn10Pin         = A5; // digital input
#endif


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


//-----------------------------------------------------------------------------
// Servoboard control, analog inputs, buttons and lamps
//-----------------------------------------------------------------------------


void ControlCommand(Control::Control& controller)
{
  // Dual mode PWM or PWM+Dir?
  if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_DUAL)!=0) {
    // Dual mode on D9 and D10 for L620X
    int fwdPwm, revPwm, en;
    
    // Forward direction PWM?
    if (Globals::Controller.ForwardCmd) {
      fwdPwm = controller.TorqueCmd; // 4095
    } else {
      fwdPwm = 0;
    }
    // Reverse direction PWM?
    if (Globals::Controller.ReverseCmd) {
      revPwm = controller.TorqueCmd; // 4095
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
  
    // Digital PWM
    if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_DIGITAL)!=0) {
      // Must use PWM centered mode on PC side for Aganyte's PWM2M2, PWM2HAPP and FFB Converter
      if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_CENTERED)!=0) {
        // Sign value
        int FFBConverter_torqueCmd = Globals::Controller.TorqueCmd-0x800; // Value between -2047..2047
        // Restrict range to 0..255
        unsigned char TorqueValue = map(FFBConverter_torqueCmd,-2047,2047,0,255);
        // Send value as 1 byte header + 1 byte value
        // limit sending of bytes every n ticks
        //if (Globals::Ticker.Tick%2==0) {
          #ifdef ARDUINO_AVR_LEONARDO
          // PWM2M2 on Pro micro/mini
          if (Serial1.availableForWrite()>2) {
            Serial1.write(0x69); // = 'i'
            Serial1.write(TorqueValue); // center = 0x80
          }
          #elif ARDUINO_AVR_MEGA2560
          // FFB Converter on Mega2560
          // Will use ConfigFile.DigitalPWMSerialSpeed in the future
          if (DIG_PWM_SERIAL.availableForWrite()>2) {
            DIG_PWM_SERIAL.write(0x69); // = 'i'
            DIG_PWM_SERIAL.write(TorqueValue); // center = 0x80
          }
          #endif
        //}
      } else {
        // Transfer full PWM value as 1 byte header and 2 bytes value
        #ifdef ARDUINO_AVR_LEONARDO
          if (Serial1.availableForWrite()>3) {
            Serial1.write(0x69); // = 'i'
            Serial1.write((Globals::Controller.TorqueCmd>>8)&0xFF);
            Serial1.write(Globals::Controller.TorqueCmd&0xFF);
          }
        #elif ARDUINO_AVR_MEGA2560
          // Will use ConfigFile.DigitalPWMSerialSpeed in the future
          if (DIG_PWM_SERIAL.availableForWrite()>3) {
            DIG_PWM_SERIAL.write(0x69);
            DIG_PWM_SERIAL.write((Globals::Controller.TorqueCmd>>8)&0xFF);
            DIG_PWM_SERIAL.write(Globals::Controller.TorqueCmd&0xFF);
          }
        #endif
      }
    }
  
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


uint16_t ButtonsPressed = 0;
uint16_t ButtonsPushed = 0;
uint16_t ButtonsReleased = 0;
uint16_t LastButtonsPressed = 0;

#define IS_PUSHED(btn) ((ButtonsPushed & (btn))!=0)
#define IS_PRESSED(btn) ((ButtonsPressed & (btn))!=0)
#define IS_RELEASED(btn) ((ButtonsPressed & (btn))==0)

void Buttons(Control::Control& controller)
{
  uint16_t buttons = 0;
  uint16_t buttonchanged = 0;

  uint16_t btn1 = !IOs::DigitalReadFilter(DInBtn1Pin);
  uint16_t btn2 = !IOs::DigitalReadFilter(DInBtn2Pin);
  uint16_t btn3 = !IOs::DigitalReadFilter(DInBtn3Pin);
  uint16_t btn4 = !IOs::DigitalReadFilter(DInBtn4Pin);
  
  uint16_t btn5 = !IOs::DigitalReadFilter(DInBtn5Pin);
  uint16_t btn6 = !IOs::DigitalReadFilter(DInBtn6Pin);
  uint16_t btn7 = !IOs::DigitalReadFilter(DInBtn7Pin);
  uint16_t btn8 = !IOs::DigitalReadFilter(DInBtn8Pin);

  uint16_t btn9 = !IOs::DigitalReadFilter(DInBtn9Pin);
  uint16_t btn10 = !IOs::DigitalReadFilter(DInBtn10Pin);
  uint16_t btn11 = 0;
  uint16_t btn12 = 0;
  uint16_t btn13 = 0;
  uint16_t btn14 = 0;
  uint16_t btn15 = 0;
  uint16_t btn16 = 0;
  
  #ifdef ARDUINO_AVR_LEONARDO
  // If no digital PWM: pin D0 and D1 can be used for button inputs
  if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_DIGITAL)==0) {
    btn11 = !IOs::DigitalReadFilter(DInBtn11Pin);
    btn12 = !IOs::DigitalReadFilter(DInBtn12Pin);
  }
  #endif
  
  #ifdef ARDUINO_AVR_MEGA2560
  btn11 = !IOs::DigitalReadFilter(DInBtn11Pin);
  btn12 = !IOs::DigitalReadFilter(DInBtn12Pin);
  btn13 = !IOs::DigitalReadFilter(DInBtn13Pin);
  btn14 = !IOs::DigitalReadFilter(DInBtn14Pin);
  btn15 = !IOs::DigitalReadFilter(DInBtn15Pin);
  btn16 = !IOs::DigitalReadFilter(DInBtn16Pin);
  #endif
  
  buttons = 
    (btn1<<0) + (btn2<<1) + (btn3<<2) + (btn4<<3) +
    (btn5<<4) + (btn6<<5) + (btn7<<6) + (btn8<<7) +
    (btn9<<8) + (btn10<<9) + (btn11<<10) + (btn12<<11) +
    (btn13<<12) + (btn14<<13) + (btn15<<14) + (btn16<<15);


  LastButtonsPressed = ButtonsPressed;
  ButtonsPressed = buttons;
  // Detect low->high or high->low transitions
  buttonchanged = ButtonsPressed^LastButtonsPressed;
  if (buttonchanged!=0) {
    // Detect low->high transitions
    ButtonsPushed = buttonchanged & ButtonsPressed;
    // Detect high->low transitions
    ButtonsReleased = buttonchanged & LastButtonsPressed;
  } else {
    // No changes
    ButtonsPushed = 0;
    ButtonsReleased = 0;
  }
  // Only report pressed boutons for now
  controller.Buttons = ButtonsPressed;
}

#if defined(USE_KEYPAD)
// Use ascii table to get button order
// char "./0123456789" -> in hex 0123456789AB
// '.'='*'    '/'='#'
char keys[KEYPAD_ROWS][KEYPAD_COLS] = {
  {'1','2','3'},
  {'4','5','6'},
  {'7','8','9'},
  {'.','0','/'} 
};
byte rowPins[KEYPAD_ROWS] = {KEYPAD_ROW1_Pin, KEYPAD_ROW2_Pin, KEYPAD_ROW3_Pin, KEYPAD_ROW4_Pin}; //connect to the row pinouts of the keypad
byte colPins[KEYPAD_COLS] = {KEYPAD_COL1_Pin, KEYPAD_COL2_Pin, KEYPAD_COL3_Pin}; //connect to the column pinouts of the keypad

Keypad keypad = Keypad( makeKeymap(keys), rowPins, colPins, KEYPAD_ROWS, KEYPAD_COLS );

void KeypadButtons(Control::Control& controller)
{
  if (keypad.getKeys()) {
    // Set bit according to key.
    for (int i=0; i<LIST_MAX; i++) {
      if (keypad.key[i].kstate==PRESSED) {
        int bit = keypad.key[i].kchar-'.'; // '.' is first char (ascii 46)
        controller.KeypadBtns |= 1<<bit;
      }
      if (keypad.key[i].kstate==RELEASED) {
        int bit = keypad.key[i].kchar-'.'; // '.' is first char (ascii 46)
        controller.KeypadBtns &= ~(1<<bit);
      }
    }
  }
}
#endif

void Lamps(Control::Control& controller)
{
#ifdef ARDUINO_AVR_MEGA2560
  IOs::DigitalWrite(DOutLCoin1Pin, (controller.Lamps>>0)&1);
  IOs::DigitalWrite(DOutLCoin2Pin, (controller.Lamps>>1)&1);
  IOs::DigitalWrite(DOutLStartPin, (controller.Lamps>>2)&1);
  IOs::DigitalWrite(DOutLView1Pin, (controller.Lamps>>3)&1);
  IOs::DigitalWrite(DOutLView2Pin, (controller.Lamps>>4)&1);
  IOs::DigitalWrite(DOutLView3Pin, (controller.Lamps>>5)&1);
  IOs::DigitalWrite(DOutLView4Pin, (controller.Lamps>>6)&1);
  IOs::DigitalWrite(DOutLLeaderPin,(controller.Lamps>>7)&1);
#endif
}

//-----------------------------------------------------------------------------
// Setup and initialization
//-----------------------------------------------------------------------------

void SetupBoard()
{

#ifdef ARDUINO_ARCH_SAM

  // For Due, zero, enforce analog read to be 0..4095 (0xFFF)
  analogReadResolution(12);
  
  // Common pinmode for button is below
  
#elif ARDUINO_AVR_LEONARDO

  #if FASTADC
  // set ADC prescale to 64: 1 1 0 (solve issues with crosstalk when using fast read)
  sbi(ADCSRA, ADPS2) ;
  sbi(ADCSRA, ADPS1) ;
  cbi(ADCSRA, ADPS0) ;
  #endif
  
  // Fast PWM at 15,6kHz on D9 with 0..512 range
  InitPWM_D9_D10(PWM_MAX);
  
  if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_DIGITAL)!=0) {
    // PWM2M2 on Pro micro/mini
    // D0(RX)/D1(TX) will be managed by Leonardo's uart
    Serial1.begin(PWM2M2_DIG_PWM_BAUDRATE);
  } else {
    // Additionnal button input on D0/D1
    pinMode(DInBtn11Pin, INPUT_PULLUP);
    pinMode(DInBtn12Pin, INPUT_PULLUP);
  }
  
  // Common pinmode for button is below
  
#elif ARDUINO_AVR_MEGA2560

  // D9 pwm@3.9kHz - Not yet used
  //TCCR2B = TCCR2B & B11111000 | B00000010;
  
  // Is it a FFB Controller shield on Mega 2560?
  if ((Config::ConfigFile.FFBController & CONFIG_FFBCONTROLLER_PRESENT)!=0) {
    
      // FFB Converter digital pwm on Serial 3
      // Will use ConfigFile.DigitalPWMSerialSpeed in the future
      DIG_PWM_SERIAL.begin(PWM2M2_DIG_PWM_BAUDRATE);
      
      // Reconfigure mapping
      // PWM & directions
      FwdPWMPin = FFB_PWMOUT;
      RevPWMOrFwdDirPin = FFB_DIRECTION_OUT;
      EnableOrRevDirPin = FFB_REV_DIRECTION_OUT;
      // Analog inputs
      analogInSteeringPin = FFB_WHEELPOT;
      analogInAccelPin = FFB_ACCEL;
      analogInBrakePin = FFB_BRAKE;
      analogInClutchPin = FFB_CLUTCH;
      // Buttons
      DInBtn1Pin = FFB_DInBtn1Pin;
      DInBtn2Pin = FFB_DInBtn2Pin;
      DInBtn3Pin = FFB_DInBtn3Pin;
      DInBtn4Pin = FFB_DInBtn4Pin;
      DInBtn5Pin = FFB_DInBtn5Pin;
      DInBtn6Pin = FFB_DInBtn6Pin;
      DInBtn7Pin = FFB_DInBtn7Pin;
      DInBtn8Pin = FFB_DInBtn8Pin;
      DInBtn9Pin = FFB_DInBtn9Pin;
      DInBtn10Pin = FFB_DInBtn10Pin;
      DInBtn11Pin = FFB_DInBtn11Pin;
      DInBtn12Pin = FFB_DInBtn12Pin;
      DInBtn13Pin = FFB_DInBtn13Pin;
      DInBtn14Pin = FFB_DInBtn14Pin;
      DInBtn15Pin = FFB_DInBtn15Pin;
      DInBtn16Pin = FFB_DInBtn16Pin;

      // Mega pins 22-29 : 8x digital inputs
      DDRA = 0x0;
      PORTA = 0xFF; // Activate internal pull-up resistors
    
      // Mega pins 30-37 : 8x digital inputs with pull-up for driveboard TX
      DDRC = 0x0;
      PORTC = 0xFF; // Activate internal pull-up resistors
    
      // Lamps
      DOutLCoin1Pin = FFB_DOutLCoin1Pin;
      DOutLStartPin = FFB_DOutLStartPin;
      DOutLView1Pin = FFB_DOutLView1Pin;
      DOutLView2Pin = FFB_DOutLView2Pin;
      DOutLView3Pin = FFB_DOutLView3Pin;
      DOutLView4Pin = FFB_DOutLView4Pin;
      DOutLLeaderPin = FFB_DOutLLeaderPin;
      DOutLCoin2Pin = FFB_DOutLCoin2Pin;
      
      DOutLEDPin = FFB_LED;
      
  } else {
    
    // Standard Mega2560 (standalone)
    
    // Digital PWM on serial 3 for Mega2560 
    if ((Config::ConfigFile.PWMMode & CONFIG_PWMMODE_DIGITAL)!=0) {
        // Will use ConfigFile.DigitalPWMSerialSpeed in the future
        DIG_PWM_SERIAL.begin(PWM2M2_DIG_PWM_BAUDRATE);
    }

    // Mega pins 22-29 : 8x digital outputs for driveboard RX
    DDRA = 0xFF;
    PORTA = 0; // All outpus set to 0
    
    // Mega pins 30-37 : 8x digital inputs with pull-up for driveboard TX
    DDRC = 0x0;
    PORTC = 0xFF; // Activate internal pull-up resistors
  }

  // Additionnal button input
  pinMode(DInBtn11Pin, INPUT_PULLUP);
  pinMode(DInBtn12Pin, INPUT_PULLUP);
  pinMode(DInBtn13Pin, INPUT_PULLUP);
  pinMode(DInBtn14Pin, INPUT_PULLUP);
  pinMode(DInBtn15Pin, INPUT_PULLUP);
  pinMode(DInBtn16Pin, INPUT_PULLUP);

  // Lamps
  pinMode(DOutLCoin1Pin, OUTPUT);
  pinMode(DOutLCoin2Pin, OUTPUT);
  pinMode(DOutLStartPin, OUTPUT);
  pinMode(DOutLView1Pin, OUTPUT);
  pinMode(DOutLView2Pin, OUTPUT);
  pinMode(DOutLView3Pin, OUTPUT);
  pinMode(DOutLView4Pin, OUTPUT);
  pinMode(DOutLLeaderPin, OUTPUT);

    // Common pinmode for button is below
    
  #endif

  // Dual PWM+enable OR PWM+Dir
  pinMode(FwdPWMPin,         OUTPUT); // Forward fast PWM pin on D9 OR centered PWM
  pinMode(RevPWMOrFwdDirPin, OUTPUT); // Reverse fast PWM pin on D10 OR forward dir
  pinMode(EnableOrRevDirPin, OUTPUT); // Enable drive OR reverse dir

  pinMode(DOutLEDPin,        OUTPUT); // Led


  // Potentiometers
  pinMode(analogInSteeringPin, INPUT);
  pinMode(analogInAccelPin,    INPUT);
  pinMode(analogInBrakePin,    INPUT);
  pinMode(analogInClutchPin,   INPUT);

  // Buttons
  pinMode(DInBtn1Pin,  INPUT_PULLUP);
  pinMode(DInBtn2Pin,  INPUT_PULLUP);
  pinMode(DInBtn3Pin,  INPUT_PULLUP);
  pinMode(DInBtn4Pin,  INPUT_PULLUP);

  pinMode(DInBtn5Pin,  INPUT_PULLUP);
  pinMode(DInBtn6Pin,  INPUT_PULLUP);
  pinMode(DInBtn7Pin,  INPUT_PULLUP);
  pinMode(DInBtn8Pin,  INPUT_PULLUP);

  pinMode(DInBtn9Pin,  INPUT_PULLUP);
  pinMode(DInBtn10Pin, INPUT_PULLUP);
  
  Globals::Controller.SetTorqueOut(ControlCommand);
  Globals::Controller.SetAnalogRead(Analog);
  Globals::Controller.SetButtonsRead(Buttons);
  Globals::Controller.SetLampOut(Lamps);
  
  // Extra keypad buttons ?
#if defined(USE_KEYPAD)
  Globals::Controller.SetKeypadButtonsRead(KeypadButtons);
#endif
}

void Do_Init()
{
  delay(1000);  
}


}
