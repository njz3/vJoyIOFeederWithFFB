/*
  IOBoard for Analog&Dig inputs, analog&Dig outputs
  - "Simple" serial comm
  - Watchdog for safety output
  
  Taken from http://www.arduino.cc/en/Tutorial/AnalogInOutSerial
  01/2020 Benjamin Maurin
*/
#include <avr/wdt.h>

#define VERSION_NUMBER "V0.1.6.0"

#ifdef ARDUINO_AVR_LEONARDO
#define PLATFORM_STRING "IO BOARD ON LEONARDO"
#elif ARDUINO_AVR_UNO
#define PLATFORM_STRING  "IO BOARD ON UNO"
#elif ARDUINO_AVR_MEGA2560
#define PLATFORM_STRING  "IO BOARD ON MEGA2560"
#elif ARDUINO_SAM_DUE
#define PLATFORM_STRING  "IO BOARD ON DUE"
#else
#define PLATFORM_STRING  "IO BOARD ON UNKNOWN"
#endif
#define VERSION_STRING (VERSION_NUMBER " " PLATFORM_STRING)

// For Aganyte FFB Converter (Digital PWM)
//#define FFB_CONVERTER_DIG_PWM
// For PWM +/- or L620X dual bridge on D9(fwd)/D10(rev)/D11(enable)
//#define DUAL_PWM_L620X

// Fastest RS232 com (Leonard, Mega2560, Due)
// - 115200 is the standard hihg speed baudrate, but the 
//   Mega2560@16Mhz has some timing issues (2-3% frames errors)
// - 250000, 5000000 or 1000000 is more stable on the Mega2560
//   and other native USB like Leonardo or Due have no issues 
//   whatever speed is choosen.
// => Take maximum speed 1000000 to reduce transmission delays
// Note: USB based com (Leonardo, Due) can go up to 2000000 (2Mbps)
#define PCSERIAL_BAUDRATE (1000000)

// Protocole version reply has 32bit unsigned hex in ascii format: 
// "?XXXXYYYY" where XXXX=major version, YYYY minor version
#define PROTOCOL_VERSION_MAJOR (0x0001)
#define PROTOCOL_VERSION_MINOR (0x0000)

// Faster Analog Read https://forum.arduino.cc/index.php/topic,6549.0.html
#define FASTADC 1
// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif

// If SPRINTF is to be used instead of raw conversion
//#define USE_SPRINTF_FOR_STATUS_FRAME

// Uart - do not use it for now
#define D0 (0)
#define D1 (1)

// Inputs (buttons)
#define D2 (2)
#define D3 (3)
#define D4 (4)
#define D5 (5)

#define D6 (6)
#define D7 (7)
#define D8 (8)
#define D12 (12)
#define D0 (0)
#define D1 (1)

// PWM/directions
#define D9 (9)
#define D10 (10)
#define D11 (11)

// LED
#define D13 (13)

// LAMPS & OTHER
#define D38 (38)
#define D39 (39)
#define D40 (40)
#define D41 (41)

#define D50 (50)
#define D51 (51)
#define D52 (52)
#define D53 (53)

// These constants won't change. They're used to give names to the pins used:
const int analogInSteeringPin = A0;  // Analog input pin that the potentiometer is attached to
const int analogInAccelPin = A1;  // Analog input pin that the potentiometer is attached to
const int analogInBrakePin = A2;  // Analog input pin that the potentiometer is attached to
const int analogInClutchPin = A3;  // Analog input pin that the potentiometer is attached to

#ifdef DUAL_PWM_L620X
const int FwdDirPin = D9; // Analog output pin for forward direction PWM
const int RevDirPin = D10; // Analog output pin for reverse direction PWM
const int EnablePin = D11; // digital
#else
const int TorqueOutPin = D9; // Analog output pin for PWM@20kHz
const int FwdDirPin = D10; // digital output pin for forward direction
const int RevDirPin = D11; // digital output pin for reverse direction
#endif

const int DOutLEDPin = D13; // Analog output pin that the LED is attached to

const int DOutLStartPin = D38; // digital output
const int DOutLView1Pin = D53; // digital output
const int DOutLView2Pin = D52; // digital output
const int DOutLView3Pin = D51; // digital output
const int DOutLView4Pin = D50; // digital output
const int DOutLLeaderPin = D39; // digital output

const int DInBtn1Pin = D2; // digital input
const int DInBtn2Pin = D3; // digital input
const int DInBtn3Pin = D4; // digital input
const int DInBtn4Pin = D5; // digital input

const int DInBtn5Pin = D6; // digital input
const int DInBtn6Pin = D7; // digital input
const int DInBtn7Pin = D8; // digital input
const int DInBtn8Pin = D12; // digital input

const int DInBtn9Pin = D0; // digital input
const int DInBtn10Pin = D1; // digital input
const int DInBtn11Pin = A4; // digital input
const int DInBtn12Pin = A5; // digital input




// Durée d'un tick
#define TICK_MS (5UL)
#define TICK_US (TICK_MS*1000L)
#define TICK_HZ (1000.0f/(float)TICK_MS)
#define TICK_KHZ (1.0f/(float)TICK_MS)

// Periode blink
#define BLINK_HZ (2)
// Durée blink en ms
#define BLINK_MS ((TICK_MS*100UL)/BLINK_HZ)
// Durée blink en ticks
#define BLINK_TCK (BLINK_MS/TICK_MS)

// Watchdog de reception
#define WD_TIMEOUT_MS (100)
#define WD_TIMEOUT_TCK (WD_TIMEOUT_MS/TICK_MS)

// On Leonardo, use fast PWM on pin D9
#ifdef ARDUINO_AVR_LEONARDO

// Frequence PWM 15,6kHz (15655.57730Hz exactement, voir code ESPWheel d'Etienne)
// Valeur pwm entre 0 et 511
#define PWM_MAX    (511)
void InitPWM(uint32_t top_value)
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

// Etat du blink
bool blink = false;
// Compteur de tick pour le blink
int blinktick_cnt = BLINK_TCK;

char nibbletable[] = "0123456789ABCDEFX";

void ConvertToNDigHex(uint32_t value, uint32_t N = 2, char hex[] = NULL)
{
  int32_t i;
  for(i=N-1; i>=0; i--) {
    uint32_t nibble = value & 0xF; // Récupère le nibble 'i'
    hex[i] = nibbletable[nibble];
    value = value>>4;
  }
  hex[N] = 0;
}

uint32_t ConvertHexToInt(char hex[], int N = 2)
{
  int i;
  uint32_t value = 0;
  for(i=0; i<N; i++) {
    char valhex;
    if (hex[1]>='0' && hex[i]<='9')
      valhex = hex[i]-'0';
    else if (hex[i]>='A' && hex[i]<='F')
      valhex = hex[i]-'A'+0xA;
    else 
      valhex = 0;
    uint32_t nibble = (uint32_t)(valhex&0xF); // Récupère le nibble 'i'
    value = nibble + (value<<4);
  }
  return value;
}

// Tick counter
uint32_t tick_cnt = 0;

// Ticker/Scheduler
const uint32_t timoffset_us = 0;// 4290000000ULL;
uint32_t timenow_us;
uint32_t nexttick_us;


// General config
bool DebugMode = false;
bool DoStreaming = false;
bool WatchdogEnabled = false;
uint32_t WatchdoglastRefreshTick = 0;

// Analog inputs
uint32_t steer = 0, accel = 0, brake = 0, clutch = 0, buttons = 0;
// Velocity and accel of steering as 32bits float (will be converted to hex)
float steer_vel = 0.0, steer_acc = 0.0;
uint32_t prev_steer = 0;
float prev_vel = 0.0;

// Outputs
uint32_t fwdCmd; // 0/1
uint32_t revCmd; // 0/1
uint32_t torqueCmd; // value output to the PWM (analog out)
uint32_t lamps; // Bitfield

// Reset function
void softwareReboot()
{
  wdt_enable(WDTO_15MS);
  while(1) {  }
}

void setup()
{
  #ifdef ARDUINO_AVR_LEONARDO
    #if FASTADC
    // set prescale to 64: 1 1 0 (below issues with crosstalk)
    sbi(ADCSRA,ADPS2) ;
    sbi(ADCSRA,ADPS1) ;
    cbi(ADCSRA,ADPS0) ;
    #endif
    // Fast PWM at 15,6kHz on D9 with 0..512 range
    InitPWM(PWM_MAX);
  #endif

#ifdef ARDUINO_AVR_MEGA2560
  // D9 pwm@3.9kHz
  //TCCR2B = TCCR2B & B11111000 | B00000010;
#endif

  // For Due, zero, enforce analog read to be 0..4095 (0xFFF)
#ifdef ARDUINO_ARCH_SAM
  analogReadResolution(12);
#endif

  // initialize serial communications at maximum baudrate bps:
  Serial.begin(PCSERIAL_BAUDRATE);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB
  }
  Serial.setTimeout(2);

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

  // PWM and direction
#ifdef DUAL_PWM_L620X
  pinMode(FwdDirPin, OUTPUT); // Forward fast PWM pin on D9
  pinMode(RevDirPin, OUTPUT); // Reverse fast PWM pin on D10
  pinMode(EnablePin, OUTPUT); // Enable/Disable drive
#else
  pinMode(TorqueOutPin, OUTPUT); // Dedicated fast PWM pin on D9
  pinMode(FwdDirPin, OUTPUT); // Forward
  pinMode(RevDirPin, OUTPUT); // Reverse
#endif

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

// Set default torque cmd to 0
#ifdef FFB_CONVERTER_DIG_PWM
  torqueCmd = 0x800; // Centered PWM
  fwdCmd = 0; // Disable output
  revCmd = 0;
#else
  torqueCmd = 0; // PWM+Dir
  fwdCmd = 0;
  revCmd = 0;
#endif

  nexttick_us = micros() + (TICK_MS*1000) + timoffset_us;
}



void SendStatusFrame()
{
  char buff[32];
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  char Iformat[] = "I%02X";
  char Aformat[] = "A%03X";
  char Fformat[] = "F%08X%08X";
#endif
  
  // 2x 8xDigital inputs, 4 nibbles
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  // First 8
  sprintf(buff, Iformat, buttons&0xFF);
  Serial.print(buff);
  // Second 8
  sprintf(buff, Iformat, (buttons>>8)&0xFF);
  Serial.print(buff);
#else
  // First 8
  ConvertToNDigHex(buttons&0xFF, 2, buff);
  Serial.write('I'); Serial.write(buff, 2);
  // Second 8
  ConvertToNDigHex((buttons>>8)&0xFF, 2, buff);
  Serial.write('I'); Serial.write(buff, 2);
#endif

#ifdef ARDUINO_AVR_MEGA2560
  // Second 8x Digital inputs on PORTC (pins 30-37)
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Iformat, (~PINC)&0xFF);
  Serial.print(buff);
#else
  ConvertToNDigHex(~PINC, 2, buff);
  Serial.write('I'); Serial.write(buff, 2);
#endif
#endif

  // Analog inputs, 3 nibbles
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Aformat, steer&0xFFF);
  Serial.print(buff);
  sprintf(buff, Aformat, accel&0xFFF);
  Serial.print(buff);
  sprintf(buff, Aformat, brake&0xFFF);
  Serial.print(buff);
  sprintf(buff, Aformat, clutch&0xFFF);
  Serial.print(buff);
#else
  ConvertToNDigHex(steer, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
  ConvertToNDigHex(accel, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
  ConvertToNDigHex(brake, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
  ConvertToNDigHex(clutch, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
#endif

  // Additionnal states for 1st input (wheel)
  uint32_t* vel_hex = (uint32_t*)&steer_vel;
  uint32_t* acc_hex = (uint32_t*)&steer_acc;
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Fformat, *vel_hex, *acc_hex);
  Serial.print(buff);
#else
  Serial.write('F');
  ConvertToNDigHex(*vel_hex, 8, buff);
  Serial.write(buff, 8);
  ConvertToNDigHex(*acc_hex, 8, buff);
  Serial.write(buff, 8);
#endif
    
  // Add '\n' for end-of-frame
  Serial.write('\n');
}

void SendErrorFrame(int code, String msg)
{
  char c[10];
  sprintf(c, "S%04d ", code);
  Serial.print(c);
  Serial.println(msg);
}

void SendMessageFrame(String msg)
{
  Serial.print("M");
  Serial.println(msg);
}

void DebugMessageFrame(String debug)
{
  if (!DebugMode)
    return;
  Serial.print("M");
  Serial.println(debug);
}

void Do_Init()
{
  delay(100);  
}


void tick()
{
  
  tick_cnt++;

  // Blink led
  if ((blinktick_cnt--) <= 0) {
    blinktick_cnt = BLINK_TCK;
    // Turn indicator light on.
    if (blink) {
      digitalWrite(DOutLEDPin, 1);
      blink = false;
    } else {
      digitalWrite(DOutLEDPin, 0);
      blink = true;
    }
  }

#ifdef DUAL_PWM_L620X
  // Direction/torque
  if (fwdCmd) {
    SetPWM_D9(torqueCmd>>3); // 4095 shifted by 3 = 511
  } else {
    SetPWM_D9(0);
  }
  if (revCmd) {
    SetPWM_D10(torqueCmd>>3); // 4095 shifted by 3 = 511
  } else {
    SetPWM_D10(0);
  }
  // Enable/Disable
  if (fwdCmd || revCmd) {
    // Enable ON
    digitalWrite(EnablePin, 1);
  } else {
    // Enable OFF
    digitalWrite(EnablePin, 0);  
  }

#else

    // torqueCmd is a 12bits integer 0..4096
    // Fast PWM on Leonardo on 9bits 0..511
  #ifdef ARDUINO_AVR_LEONARDO
    SetPWM_D9(torqueCmd>>3); // 4095 shifted by 3 = 511
  #else
    // Arduino's analogWrite is limited to 0..255 8bits range
    analogWrite(TorqueOutPin, torqueCmd>>4);
  #endif

// Digital PWM for Aganyte FFB Converter - Must use PWM centered mode
#ifdef FFB_CONVERTER_DIG_PWM
  int FFBConverter_torqueCmd = torqueCmd-0x800; // Value between -2047..2047
  // Now do whatever ...
#endif

  // Direction/Disable
  digitalWrite(FwdDirPin, fwdCmd);
  digitalWrite(RevDirPin, revCmd);
#endif

  // Lamps
  digitalWrite(DOutLStartPin, (lamps>>0)&1);
  digitalWrite(DOutLView1Pin, (lamps>>1)&1);
  digitalWrite(DOutLView2Pin, (lamps>>2)&1);
  digitalWrite(DOutLView3Pin, (lamps>>3)&1);
  digitalWrite(DOutLView4Pin, (lamps>>4)&1);
  digitalWrite(DOutLLeaderPin,(lamps>>5)&1);
  
#ifdef ARDUINO_ARCH_SAMD
  // For Due, zero, full 12 bits resolution 0..4095
  steer = analogRead(analogInSteeringPin);
  accel = analogRead(analogInAccelPin);
  brake = analogRead(analogInBrakePin);
  clutch = analogRead(analogInClutchPin);
#else
  // For all (Uno, Mega, Leonardo), only 10bits shift by 2 to make it into 0..4095
  uint32_t steer_0 = analogRead(analogInSteeringPin)<<2;
  uint32_t steer_1 = analogRead(analogInSteeringPin)<<2;
  uint32_t steer_2 = analogRead(analogInSteeringPin)<<2;
  uint32_t steer_3 = analogRead(analogInSteeringPin)<<2;
  steer = (steer_0 + steer_1 + steer_2 + steer_3)>>2;
  accel = analogRead(analogInAccelPin)<<2;
  brake = analogRead(analogInBrakePin)<<2;
  clutch = analogRead(analogInClutchPin)<<2;
#endif

  // Compute vel&acc in float - this is a difficult task for our poors arduinos!
  // Analog values are not stable (you should add a capacitor)
  int32_t diff_steer = (int32_t)steer - (int32_t)prev_steer;
  if (diff_steer>0x200 || diff_steer<(-0x200)) {
    DebugMessageFrame("MJump in position! Freezing vel&acc, diff=" + String(diff_steer));
  } else {
    // Should use a predictive observer for speed&accel given pwm control as accel
    // Do not forget to scale by tick frequency to get unit per [s]
    steer_vel = ((float)diff_steer)*(TICK_HZ);
    steer_acc = (steer_vel - prev_vel)*(TICK_HZ);
  }
  prev_steer = steer;
  prev_vel = steer_vel;
  
  int btn1 = !digitalRead(DInBtn1Pin);
  int btn2 = !digitalRead(DInBtn2Pin);
  int btn3 = !digitalRead(DInBtn3Pin);
  int btn4 = !digitalRead(DInBtn4Pin);
  
  int btn5 = !digitalRead(DInBtn5Pin);
  int btn6 = !digitalRead(DInBtn6Pin);
  int btn7 = !digitalRead(DInBtn7Pin);
  int btn8 = !digitalRead(DInBtn8Pin);

  int btn9 = !digitalRead(DInBtn9Pin);
  int btn10 = !digitalRead(DInBtn10Pin);
  int btn11 = !digitalRead(DInBtn11Pin);
  int btn12 = !digitalRead(DInBtn12Pin);
  
  buttons = (btn1<<0) + (btn2<<1) + (btn3<<2) + (btn4<<3) +
            (btn5<<4) + (btn6<<5) + (btn7<<6) + (btn8<<7) +
            (btn9<<8) + (btn10<<9) + (btn11<<10) + (btn12<<11);
  
  // Send update when streaming on
  if (Serial.availableForWrite()>32 &&
      DoStreaming==true) {
    SendStatusFrame();
  }

  if (Serial.available()>0) {
    char msg[64];
    
    size_t read = Serial.readBytesUntil('\n', msg, sizeof(msg));
    if (read>0) {
      // Enforce null-terminated string
      msg[read+1] = 0;
      
      size_t index = 0;
      int DigOut_block = 0;
      //int pwm_block = 0; // If multiple PWM
      while(index<read) {
        
        switch(msg[index++]) {
        case 'D': {
          DebugMode = !DebugMode;
          DebugMessageFrame("Debug mode ON");
        } break;
        case '?': {
          // Handshaking!
          char buff[32];
          // Read protocol version
          char *sc = (char*)(msg+index);
          int major = ConvertHexToInt(sc, 4);
          int minor = ConvertHexToInt(sc+4, 4);
          DebugMessageFrame("recv major=" + String(major,HEX) + " minor=" + String(minor,HEX));
          if (major>=PROTOCOL_VERSION_MAJOR) {
            DebugMessageFrame("handshaking ok");
          }

          // Send protocol version - hardcoded
          sprintf(buff, "?%04X%04X", PROTOCOL_VERSION_MAJOR, PROTOCOL_VERSION_MINOR);
          Serial.print(buff);
          // frame terminated
          index = read;
        } break;
        case 'V': {
          // Board version - hardcoded
          Serial.println(VERSION_STRING);
          index = read;
        } break;
        case '~': {
          // Reset Board 
          softwareReboot();
        } break;
        
        case 'G': {
          // Hardware description - hardcoded
#ifdef ARDUINO_AVR_MEGA2560
          Serial.println("GI3A4O2P1F1"); // For 2560 : idem Leonardo and add 1xDI(x8) and 1xDO(x8)
#else
          Serial.println("GI2A4O1P1F1"); // 2xDI(x8),3xAIn,1xDO(x8),1xPWM, 1xFullstate, 0xEnc
#endif
          index = read;
        } break;

        case 'I': {
          index = read;
          // Notify caller that initialization is started
          SendMessageFrame("Initialization started");
          // Initialize Arduino code - this function will
          // block a long time, to timer tick will be
          // refreshed just after executing it
          Do_Init();
          // Send back ready "R" message followed by some comment
          Serial.println("RInitialization done");
          // Reset ticker time
          nexttick_us = micros() + (TICK_MS*1000) + timoffset_us;
          
        } break;
        
        case 'U': {
          // Send single status frame
          if (!DoStreaming) {
            SendStatusFrame();
          }
        } break;
        
        case 'W': {
          // Start watchdog safety
          WatchdogEnabled = true;
          DebugMessageFrame("WD enabled");
          index = read;
        } break;
        case 'T': {
          // Stop Watchdog safety
          WatchdogEnabled = false;
          DebugMessageFrame("WD disabled");
          index = read;
        } break;
        
        case 'S': {
          // Start streaming
          DoStreaming = true;
          DebugMessageFrame("Start streaming");
          index = read;
        } break;
        
        case 'H': {
          // Halt streaming
          DoStreaming = false;
          DebugMessageFrame("Stop streaming");
          index = read;
        } break;

        case 'O': { // partially done
          char *sc = (char*)(msg+index);
          int do_value = ConvertHexToInt(sc, 2);
          switch(DigOut_block) {
            case 0:
              // direction/enable
              fwdCmd = (do_value>>0)&0x1;
              revCmd = (do_value>>1)&0x1;
              // 6 Lamps
              lamps = (do_value>>2)&0xF;
              DebugMessageFrame("O0=" + String(do_value,HEX));
              break;
#ifdef ARDUINO_AVR_MEGA2560
            case 1:
              PORTA = do_value;
              DebugMessageFrame("PORTA=" + String(do_value,HEX));
              break;
#endif
            default:
              SendErrorFrame(3, "OUTBLOCK " + String(DigOut_block) + " NOT FOUND FOR " + String(do_value,HEX));
              break;
          }
          DigOut_block++;
          index+=2;
        } break;

        case 'P': { // partially done
          char *sc = (char*)(msg+index);
          torqueCmd = ConvertHexToInt(sc, 3);
          DebugMessageFrame("pwm=" + String(torqueCmd,HEX));
          index+=3;
        } break;

        case 'E': { // Not yet done
          char *sc = (char*)(msg+index);
          uint32_t encoder = ConvertHexToInt(sc, 8);
          DebugMessageFrame("enc=" + String(encoder,HEX));
          index+=8;
        }
        break;

        default:
          SendErrorFrame(0, "UNKNOWN CMD " + String(msg));
          index = read;
          break;
      }
      }
      WatchdoglastRefreshTick = tick_cnt;
    }
  }
  
  // Watchdog management
  if (WatchdogEnabled) {
    uint32_t diff = tick_cnt - WatchdoglastRefreshTick;
    if (diff>WD_TIMEOUT_TCK) {
      // Watchdog triggered !!
      // Put outputs in safety state
      fwdCmd = 0;
      revCmd = 0;
#ifdef FFB_CONVERTER_DIG_PWM
      torqueCmd = 0x800;
#else
      torqueCmd = 0;
#endif
      // Disable watchdog
      WatchdogEnabled = false;
      // Output an error
      SendErrorFrame(2, "WD TRIGGERED");
    }
  }
}

void loop()
{
  // Compute time in us until next tick -- will roll-over
  nexttick_us += TICK_US;
  // Time of execution for scheduler -- will roll-over
  unsigned long timesched_us = micros() + timoffset_us;
  // Get difference, this cancels possible roll-overs
  long delay_us = nexttick_us - timesched_us;
  if (delay_us>0) {
    // If delay, pause mcu
    if (delay_us<10000) {
      delayMicroseconds(delay_us);
    } else {
      delay(delay_us/1000);
    }
  } else {
    // Overrun! Immediatly execute tick
    if (Serial.availableForWrite()>32) {
      SendErrorFrame(4, "OVERRUN!");
    }
  }
  
  timenow_us = micros() + timoffset_us;
  // Execute tick
  tick();
  
  if (DebugMode && (Serial.availableForWrite()>32) && ((tick_cnt&0xF)==0)) {
    unsigned long timetook_us = micros() + timoffset_us - timenow_us;
    DebugMessageFrame("Tick duration:" + String(timetook_us) + "us");
  }
  // Remaining time left for Arduino's internal job
}
