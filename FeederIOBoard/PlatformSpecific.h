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

// OTHER
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
const int analogInAccelPin    = A1;  // Analog input pin that the potentiometer is attached to
const int analogInBrakePin    = A2;  // Analog input pin that the potentiometer is attached to
const int analogInClutchPin   = A3;  // Analog input pin that the potentiometer is attached to

const int FwdPWMPin           = D9;  // Analog output pin for forward PWM
const int RevPWMOrFwdDirPin   = D10; // Analog output pin for reverse PWM or forward dir for PWM+Dir
const int EnableOrRevDirPin   = D11; // digital output pin for enable or reverse dir for PWM+Dir

const int DOutLEDPin          = D13; // Analog output pin that the LED is attached to

#ifdef ARDUINO_AVR_MEGA2560
// M2PAC pinout, but order as of sega's lamp byte
const int DOutLCoin1Pin      = A9; // digital output
const int DOutLCoin2Pin      = A15; // digital output
const int DOutLStartPin       = A10; // digital output
const int DOutLView1Pin       = A11; // digital output
const int DOutLView2Pin       = A12; // digital output
const int DOutLView3Pin       = A13; // digital output
const int DOutLView4Pin       = A14; // digital output
const int DOutLLeaderPin      = A8; // digital output
#endif

// Common pinout
const int DInBtn1Pin = D2; // digital input
const int DInBtn2Pin = D3; // digital input
const int DInBtn3Pin = D4; // digital input
const int DInBtn4Pin = D5; // digital input

const int DInBtn5Pin = D6; // digital input
const int DInBtn6Pin = D7; // digital input
const int DInBtn7Pin = D8; // digital input
const int DInBtn8Pin = D12; // digital input

const int DInBtn9Pin = A4; // digital input
const int DInBtn10Pin = A5; // digital input
const int DInBtn11Pin = D0; // digital input - only when not using digital PWM
const int DInBtn12Pin = D1; // digital input - only when not using digital PWM


void SetupBoard();

void Do_Init();

}
