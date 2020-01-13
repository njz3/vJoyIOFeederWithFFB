/*
  IOBoard for Analog&Dig inputs, analog&Dig outputs
  - "Simple" serial comm
  - Watchdog for safety output
  
  Taken from http://www.arduino.cc/en/Tutorial/AnalogInOutSerial
  01/2020 Benjamin Maurin
*/

#ifdef ARDUINO_AVR_LEONARDO
#define VERSION_STRING  "V1.0.0.0 IO BOARD ON LEONARDO"
#elif ARDUINO_AVR_UNO
#define VERSION_STRING  "V1.0.0.0 IO BOARD ON UNO"
#elif ARDUINO_AVR_MEGA2560
#define VERSION_STRING  "V1.0.0.0 IO BOARD ON MEGA2560"
#elif ARDUINO_SAM_DUE
#define VERSION_STRING  "V1.0.0.0 IO BOARD ON DUE"
#else
#define VERSION_STRING  "V1.0.0.0 IO BOARD ON UNKNOWN"
#endif


// Faster Analog Read https://forum.arduino.cc/index.php/topic,6549.0.html
#define FASTADC 1
// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif

// Uart
#define D0 (0)
#define D1 (1)
// Buttons
#define D2 (2)
#define D3 (3)
#define D4 (4)
#define D5 (5)

#define D6 (6)
#define D7 (7)
#define D8 (8)
#define D12 (12)

// PWM/directions
#define D9 (9)
#define D10 (10)
#define D11 (11)

// LED
#define D13 (13)

// These constants won't change. They're used to give names to the pins used:
const int analogInSteeringPin = A0;  // Analog input pin that the potentiometer is attached to
const int analogInAccelPin = A1;  // Analog input pin that the potentiometer is attached to
const int analogInBrakePin = A2;  // Analog input pin that the potentiometer is attached to

const int TorqueOutPin = D9; // Analog output pin for PWM@20kHz
const int FwdDirPin = D10; // digital output pin for forward direction
const int RevDirPin = D11; // digital output pin for reverse direction

const int DOutLEDPin = D13; // Analog output pin that the LED is attached to




// Durée d'un tick
#define TICK_MS (10UL)
#define TICK_US (TICK_MS*1000L)
#define TICK_HZ (1000.0f/(float)TICK_MS)

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
  OCR1A = 0;
#endif
}

void SetPWM(uint16_t pwm)
{
  if (pwm>PWM_MAX)
    pwm = PWM_MAX;
  OCR1A = pwm;
}
#endif

// Etat du blink
bool blink = false;
// Compteur de tick pour le blink
int blinktick_cnt = BLINK_TCK;

char nibletable[] = "0123456789ABCDEFX";

String ConvertToNDigHex(uint32_t value, uint32_t N = 2)
{
  uint32_t i;
  String result = ""; 
  for(i=0; i<N; i++) {
    uint32_t nibble = value & 0xF; // Récupère le nibble 'i'
    result = nibletable[nibble] + result;
    value = value>>4;
  }
  return result;
}

uint32_t ConvertHexToInt(char *hex, int N = 2)
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
  
  // For Due, zero, enforce analog read to be 0..4095 (0xFFF)
#ifdef ARDUINO_ARCH_SAM
  analogReadResolution(12);
#endif

  // initialize serial communications at 9600 bps:
  Serial.begin(115200); // Fastest RS232 com (all avrs)
  //Serial.begin(2000000); // USB based com (Leonardo, Due) up to 2000000 (2Mbps)
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB
  }
  Serial.setTimeout(1);

  // Potentiometers
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(A2, INPUT);

  // Buttons
  pinMode(D2, INPUT_PULLUP);
  pinMode(D3, INPUT_PULLUP);
  pinMode(D4, INPUT_PULLUP);
  pinMode(D5, INPUT_PULLUP);

  pinMode(D6, INPUT_PULLUP);
  pinMode(D7, INPUT_PULLUP);
  pinMode(D8, INPUT_PULLUP);
  pinMode(D12, INPUT_PULLUP);

  // PWM and direction
  pinMode(D9, OUTPUT); // Dedicated fast PWM pin on D9
  pinMode(D10, OUTPUT); // Forward
  pinMode(D11, OUTPUT); // REverse

  pinMode(D13, OUTPUT); // Led
 
  nexttick_us = micros() + (TICK_MS*1000) + timoffset_us;
}

bool DoStreaming = false;
bool WatchdogEnabled = false;
uint32_t WatchdoglastRefreshTick = 0;

// Analog inputs
uint32_t steer = 0, accel = 0, brake = 0, buttons = 0;
// Velocity and accel of steering as 32bits float (will be converted to hex)
float steer_vel = 0.0, steer_acc = 0.0;
uint32_t prev_steer = 0;
float prev_vel = 0.0;

// Outputs
uint32_t directionCmd = 0; // 0/1
uint32_t torqueCmd = 0; // value output to the PWM (analog out)

void SendStatusFrame()
{
  // Digital inputs, 2 nibbles
  Serial.print("I");
  Serial.print(ConvertToNDigHex(buttons, 2));
  
  // Analog inputs, 3 nibbles
  Serial.print("A");
  Serial.print(ConvertToNDigHex(steer, 3));
  Serial.print("A");
  Serial.print(ConvertToNDigHex(accel, 3));
  Serial.print("A");
  Serial.print(ConvertToNDigHex(brake, 3));
  // Additionnal states for 1st input (wheel)
  Serial.print("F");
  uint32_t* vel_hex = (uint32_t*)&steer_vel;
  Serial.print(ConvertToNDigHex(*vel_hex, 8));
  uint32_t* acc_hex = (uint32_t*)&steer_acc;
  Serial.print(ConvertToNDigHex(*acc_hex, 8));
  
  // Add '\r' for end-of-frame
  Serial.println();
/*
  Serial.print("M");
  Serial.print(torqueCmd);
  Serial.println();
*/
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

  // torqueCmd is a 12bits integer 0..4096
  // Fast PWM on Leonardo
#ifdef ARDUINO_AVR_LEONARDO
  SetPWM(torqueCmd>>3); // 4094 shifted by 3 = 512
#else
  // Arduino's analogWrite is limited to 0..255 8bits range
  analogWrite(TorqueOutPin, torqueCmd>>4);
#endif

  // Change direction
  if (directionCmd==0) {
    digitalWrite(FwdDirPin, HIGH);
    digitalWrite(RevDirPin, LOW);
  } else {
    digitalWrite(FwdDirPin, LOW);
    digitalWrite(RevDirPin, HIGH);
  
  }
#ifdef ARDUINO_ARCH_SAMD
  // For Due, zero, full 12 bits resolution 0..4095
  steer = analogRead(analogInSteeringPin);
  accel = analogRead(analogInAccelPin);
  brake = analogRead(analogInBrakePin);
#else
  // For all (Uno, Mega, Leonardo), only 10bits shift by 2 to make it into 0..4095
  steer = analogRead(analogInSteeringPin)<<2;
  accel = analogRead(analogInAccelPin)<<2;
  brake = analogRead(analogInBrakePin)<<2;
#endif

  // Compute vel&acc in float - this is a difficult task for our poors arduinos!
  // Analog values are not stable (you should add a capacitor)
  int32_t diff_steer = (int32_t)steer - (int32_t)prev_steer;
  if (diff_steer>0x200 || diff_steer<(-0x200)) {
    Serial.print("MJump in position! Freezing vel&acc, diff=");
    Serial.println(diff_steer);
  } else {
    // Do not forget to scale by 12bits=4096 to get unit per [s]
    steer_vel = ((float)diff_steer)*(TICK_HZ/4096.0f);
    steer_acc = (steer_vel - prev_vel)*(TICK_HZ/4096.0f);
  }
  prev_steer = steer;
  prev_vel = steer_vel;
  
  int b0 = !digitalRead(D2);
  int b1 = !digitalRead(D3);
  int b2 = !digitalRead(D4);
  int b3 = !digitalRead(D5);
  
  int b4 = !digitalRead(D6);
  int b5 = !digitalRead(D7);
  int b6 = !digitalRead(D8);
  int b7 = 0; //!digitalRead(D9);
  buttons = (b0<<0) + (b1<<1) + (b2<<2) + (b3<<3) +
                (b4<<4) + (b5<<5) + (b6<<6) + (b7<<7);
  /*
  Serial.print(tick_cnt);
  Serial.print(" ");
  Serial.print(timenow_us);
  Serial.print(" ");
  */
  if (Serial.availableForWrite()>32 &&
      DoStreaming==true) {
    SendStatusFrame();
  }

  if (Serial.available()>0) {
    byte msg[32];
    
    size_t read = Serial.readBytesUntil('\n', msg, sizeof(msg));
    if (read>0) {
      
      size_t index = 0;
      while(index<read) {
        
        switch(msg[index++]) {
        case 'V':
          // Version
          Serial.println(VERSION_STRING);
          index = read;
          break;
        
        case 'G':
          // Hardware
          Serial.println("GI1A3O1P1F1");
          index = read;
          break;

        case 'U':
          // Send single status frame
          if (!DoStreaming)
            SendStatusFrame();
          break;
        
        case 'W':
          // Start streaming
          WatchdogEnabled = true;
          index = read;
          break;
        case 'T':
          // Halt streaming
          WatchdogEnabled = false;
          index = read;
          break;
        
        case 'S':
          // Start streaming
          DoStreaming = true;
          index = read;
          break;
        
        case 'H':
          // Halt streaming
          DoStreaming = false;
          index = read;
          break;

        case 'O': { // partially done
          char *sc = (char*)(msg+index);
            directionCmd = ConvertHexToInt(sc, 2);
            /*Serial.print("Mdir=");
            Serial.println(directionCmd);*/
            index+=2;
          }
          break;

        case 'P': { // partially done
            char *sc = (char*)(msg+index);
            torqueCmd = ConvertHexToInt(sc, 3);
            /*Serial.print("Mtrq=");
            Serial.println(torqueCmd);*/
            index+=3;
        }
        break;

        case 'E': { // Not yet done
            char *sc = (char*)(msg+index);
            uint32_t encoder = ConvertHexToInt(sc, 8);
            /*Serial.print("Menc=");
            Serial.println(encoder);*/
            index+=8;
        }
        break;

        default:
        Serial.print("S0000 UNKNOWN COMMAND ");
        for(size_t i=0; i<read; i++) {
          Serial.print((char)msg[i]);
        }
        Serial.println();
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
      directionCmd = 0;
      torqueCmd = 0;
      // Disable watchdog
      WatchdogEnabled = false;
      // Output an error
      Serial.println("S0002 WATCHDOG TRIGGERED");
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
      Serial.println("MOverrun detected!");
    }
  }
  
  timenow_us = micros() + timoffset_us;
  // Execute tick
  tick();
  /*
  unsigned long timetook_us = micros() + timoffset_us - timenow_us;
  if (Serial.availableForWrite()>32) {
    Serial.print("Time took:");
    Serial.println(timetook_us);
  }*/
  // Remaining time left for Arduino's internal job
}
