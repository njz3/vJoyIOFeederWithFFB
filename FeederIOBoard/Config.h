/*
  Management of configuration for IO board
*/
#pragma once
// Arduino Framework
#include <Arduino.h>


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

// Protocole version reply has 32bit unsigned hex in ascii format: 
// "?XXXXYYYY" where XXXX=major version, YYYY minor version
#define PROTOCOL_VERSION_MAJOR (0x0001)
#define PROTOCOL_VERSION_MINOR (0x0000)

// Faster Analog Read https://forum.arduino.cc/index.php/topic,6549.0.html
#define FASTADC 1

// If SPRINTF is to be used instead of raw conversion
//#define USE_SPRINTF_FOR_STATUS_FRAME

// Durée d'un tick
#define TICK_MS (5UL)
#define TICK_US (TICK_MS*1000L)
#define TICK_HZ (1000.0f/(float)TICK_MS)
#define TICK_KHZ (1.0f/(float)TICK_MS)

// Periode blink
#define BLINK_HZ (2)
// Durée blink en ms
#define BLINK_MS (1000UL/BLINK_HZ)
// Durée blink en ticks
#define BLINK_TCK (BLINK_MS/TICK_MS)

// Watchdog de reception
#define WD_TIMEOUT_MS (100)
#define WD_TIMEOUT_TCK (WD_TIMEOUT_MS/TICK_MS)


namespace Config {

enum COMSPEED {
  COM57600 = 0,
  COM115200 = 1,
  COM250000 = 2,
  COM500000 = 3,
  COM1000000 = 4,
};

// Fastest RS232 com (Leonard, Mega2560, Due)
// - 115200 is the standard hihg speed baudrate, but the 
//   Mega2560@16Mhz has some timing issues (2-3% frames errors)
// - 250000, 5000000 or 1000000 is more stable on the Mega2560
//   and other native USB like Leonardo or Due have no issues 
//   whatever speed is choosen.
// => Take maximum speed 1000000 to reduce transmission delays
// Note: USB based com (Leonardo, Due) can go up to 2000000 (2Mbps)
#define PCSERIAL_BAUDRATE (COM1000000)

#define CONFIG_PWMMODE_CENTERED (1<<0)
#define CONFIG_PWMMODE_DUAL (1<<1)

#define CONFIG_WHEELMODE_ENCODER (1<<0)
#define CONFIG_WHEELMODE_FILTER (1<<1)
#define CONFIG_WHEELMODE_VELACC_OBSERVER (1<<2)

#define CONFIG_PEDALMODE_NO_CLUTCH (1<<0)
#define CONFIG_PEDALMODE_FILTER (1<<1)



// Non-volatile (eeprom) config, bytes field only
typedef struct {
  // bitfield PWM
  byte PWMMode;
  // enum baudrate
  byte SerialSpeed;
  // Wheel mode: first analog input, or encoder ?
  byte WheelMode;
  // Pedal mode: report clutch? Filter?
  byte PedalMode;
} EEPROM_CONFIG;


extern EEPROM_CONFIG ConfigFile;
int SaveConfigToEEPROM();
int LoadConfigFromEEPROM();
void ResetConfig();

}
