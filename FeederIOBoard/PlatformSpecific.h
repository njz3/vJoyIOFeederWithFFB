/*
  Platform dependant code
  04/2020 Benjamin Maurin
*/
#pragma once
#include "Config.h"

namespace PlatformSpecific {

// defines for setting and clearing register bits
#ifndef cbi
#define cbi(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
#endif
#ifndef sbi
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#endif

//-----------------------------------------------------------------------------
// Arduino pins naming
//-----------------------------------------------------------------------------

// Uart - do not use it for now
#define D0 (0)
#define D1 (1)

// Inputs (buttons)
#define D0 (0)
#define D1 (1)
#define D2 (2)
#define D3 (3)
#define D4 (4)
#define D5 (5)

#define D6 (6)
#define D7 (7)
#define D8 (8)

// PWM/directions
#define D9 (9)
#define D10 (10)
#define D11 (11)

#define D12 (12)

// LED
#define D13 (13)

// OTHER - MEGA 2560
// PORT B
#define D42 (42)
#define D43 (43)
#define D44 (44)
#define D45 (45)
#define D46 (46)
#define D47 (47)
#define D48 (48)

// PORT D+G
#define D38 (38)
#define D39 (39)
#define D40 (40)
#define D41 (41)
// Port L
#define D42 (42)
#define D43 (43)
#define D44 (44)
#define D45 (45)
#define D46 (46)
#define D47 (47)
#define D48 (48)
#define D49 (49)
// PORT K
#define D50 (50)
#define D51 (51)
#define D52 (52)
#define D53 (53)
#define D54 (54)
#define D55 (55)


//-----------------------------------------------------------------------------
// IOs pin mapping
// These constants will change depending on the platform. They're used to give
// names to the pins used in the code.
//-----------------------------------------------------------------------------

// Analog input pin that the potentiometer is attached to
extern int analogInSteeringPin;
extern int analogInAccelPin;
extern int analogInBrakePin;
extern int analogInClutchPin;

// Analog output pin for forward PWM
extern int FwdPWMPin;
// Analog output pin for reverse PWM or forward dir for PWM+Dir
extern int RevPWMOrFwdDirPin;
// digital output pin for enable or reverse dir for PWM+Dir
extern int EnableOrRevDirPin;

// Analog output pin that the blink LED is attached to
extern int DOutLEDPin;

#ifdef ARDUINO_AVR_MEGA2560
// Lamps
extern int DOutLCoin1Pin;
extern int DOutLCoin2Pin;
extern int DOutLStartPin;
extern int DOutLView1Pin;
extern int DOutLView2Pin;
extern int DOutLView3Pin;
extern int DOutLView4Pin;
extern int DOutLLeaderPin;
#endif

// 8 common buttons pinout
extern int DInBtn1Pin;
extern int DInBtn2Pin;
extern int DInBtn3Pin;
extern int DInBtn4Pin;

extern int DInBtn5Pin;
extern int DInBtn6Pin;
extern int DInBtn7Pin;
extern int DInBtn8Pin;

extern int DInBtn9Pin;
extern int DInBtn10Pin;

#ifdef ARDUINO_AVR_LEONARDO
// 2 Additional buttons for Leonardo
extern int DInBtn11Pin; // digital input - only when not using digital PWM
extern int DInBtn12Pin; // digital input - only when not using digital PWM
#endif

#ifdef ARDUINO_AVR_MEGA2560
// 8 Additional buttons for Mega
extern int DInBtn11Pin;
extern int DInBtn12Pin;
extern int DInBtn13Pin;
extern int DInBtn14Pin;
extern int DInBtn15Pin;
extern int DInBtn16Pin;
#endif


// FFB Controller's define
// I/O
#define FFB_SELECT             D3
#define FFB_LEFT               D2
#define FFB_RIGHT              D5
#define FFB_ONEWIRE            D9
#define FFB_PWMOUT             D4
// Not used here
#define FFB_PWMIN              A1
#define FFB_DIRECTION_IN       D7
// Analog input pin that the potentiometer is attached to
#define FFB_WHEELPOT           A0
#define FFB_ACCEL              A1
#define FFB_BRAKE              A2
#define FFB_CLUTCH             A3

#define FFB_DIRECTION_OUT      D8
#define FFB_REV_DIRECTION_OUT  D10
#define FFB_LED                D6

// UART
#define FFB_PC                  Serial
#define FFB_LINE_A              Serial3
#define FFB_LINE_B              Serial2

// For Aganyte FFB Converter and PWM2M2 (Digital PWM)
//#define FFB_CONVERTER_DIG_PWM
// 115200 baudrate is very bad on Mega2560@16Mhz, prefer 38400
// see here: http://ruemohr.org/~ircjunk/avr/baudcalc/avrbaudcalc-1.0.8.php?postbitrate=&postclock=16
//#define PWM2M2_DIG_PWM_BAUDRATE (115200)
#define PWM2M2_DIG_PWM_BAUDRATE (38400)
#define DIG_PWM_SERIAL          Serial3

// Lamps - digital output
#define FFB_DOutLLeaderPin          A8
#define FFB_DOutLCoin1Pin           A9
#define FFB_DOutLStartPin           A10
#define FFB_DOutLView1Pin           A11
#define FFB_DOutLView2Pin           A12
#define FFB_DOutLView3Pin           A13
#define FFB_DOutLView4Pin           A14
#define FFB_DOutLCoin2Pin           A15

// Buttons - digital inputs
#define FFB_DInBtn1Pin              FFB_LEFT
#define FFB_DInBtn2Pin              FFB_RIGHT
#define FFB_DInBtn3Pin              FFB_SELECT
#define FFB_DInBtn4Pin              FFB_DIRECTION_IN
#define FFB_DInBtn5Pin              A4
#define FFB_DInBtn6Pin              A5
#define FFB_DInBtn7Pin              A6
#define FFB_DInBtn8Pin              A7
#define FFB_DInBtn9Pin              D38
#define FFB_DInBtn10Pin             D39
#define FFB_DInBtn11Pin             D40
#define FFB_DInBtn12Pin             D41
#define FFB_DInBtn13Pin             D50
#define FFB_DInBtn14Pin             D51
#define FFB_DInBtn15Pin             D52
#define FFB_DInBtn16Pin             D53

#if defined(USE_KEYPAD)
#define KEYPAD_ROWS                 (4)
#define KEYPAD_COLS                 (3)
#define KEYPAD_ROW1_Pin             D42
#define KEYPAD_ROW2_Pin             D43
#define KEYPAD_ROW3_Pin             D44
#define KEYPAD_ROW4_Pin             D45
#define KEYPAD_COL1_Pin             D46
#define KEYPAD_COL2_Pin             D47
#define KEYPAD_COL3_Pin             D48
#endif

void SetupBoard();

void Do_Init();

}
