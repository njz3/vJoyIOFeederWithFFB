/*
  IOBoard for Analog&Dig inputs, analog&Dig outputs
  - "Simple" serial comm
  - Watchdog for safety output
  
  Taken from http://www.arduino.cc/en/Tutorial/AnalogInOutSerial
  01/2020 Benjamin Maurin
*/
#include "Config.h"
#include "Utils.h"
#include "PlatformSpecific.h"
#include "IOs.h"
#include "Globals.h"
#include "Protocol.h"
#include "Watchdog.h"


void BlinkDelegate(bool state)
{ 
  if ((Config::ConfigFile.FFBController & CONFIG_FFBCONTROLLER_PRESENT)!=0) {
    // FFB controller led is too bright
    if (state)
      IOs::AnalogWrite(PlatformSpecific::DOutLEDPin, 5);
    else 
      IOs::AnalogWrite(PlatformSpecific::DOutLEDPin, 0);
  } else {
      IOs::DigitalWrite(PlatformSpecific::DOutLEDPin, state);
  }
}

void WatchdogSafetyDelegate(void)
{ 
  // Put outputs in safety state
  Globals::Controller.ResetCommands(Config::ConfigFile.PWMMode);
  // Output an error
  Protocol::SendErrorFrame(2, "WD TRIGGERED");
}

void OverrunDelegate(Ticker::Ticker &ticker)
{
  if (Globals::VolatileConfig.DebugMode && (Serial.availableForWrite()>32)) {
    Protocol::DebugMessageFrame("Overrun detected! duration:" + String(ticker.Duration_us) + "us");
  }
}

void TickDelegate(Ticker::Ticker &ticker)
{
  Globals::BlinkLED.Refresh();

  Globals::Controller.Refresh();

  // Send update when streaming on
  if (Globals::VolatileConfig.DoStreaming==true && Serial.availableForWrite()>32) {
    Protocol::SendStatusFrame();
  }

  // Process protocol message
  while(Protocol::ProcessOneMessage()>0) {
    delayMicroseconds(100);
  }
    
  // Watchdog management
  Globals::WatchdogSafety.Check(Globals::Ticker.Tick);
  
  if (Globals::VolatileConfig.DebugMode && (Serial.availableForWrite()>32) && ((ticker.Tick&0xF)==0)) {
    Protocol::DebugMessageFrame("Tick " + String(ticker.Tick) + " duration " + String(ticker.Duration_us) + " us");
  }
}

void setup()
{
  int stt = Config::LoadConfigFromEEPROM();
  if (stt!=1)
    Config::ResetConfig();
  
  PlatformSpecific::SetupBoard();
  
  Globals::BlinkLED.BlinkPeriod = BLINK_TCK;
  Globals::BlinkLED.SetEvent(BlinkDelegate);
  
  Globals::WatchdogSafety.Timeout = WD_TIMEOUT_TCK;
  Globals::WatchdogSafety.SetEvent(WatchdogSafetyDelegate);

  Globals::Ticker.TickPeriod_us = TICK_US;
  Globals::Ticker.SetEvent(TickDelegate);
  Globals::Ticker.SetOverrun(OverrunDelegate);

  Protocol::SetupPort();

  // Set default torque cmd to 0
  Globals::Controller.ResetCommands(Config::ConfigFile.PWMMode);

  Globals::Ticker.Init();
}

// Arduino's loop - need to leave control for Arduino's internal job
void loop()
{
  Globals::Ticker.Refresh();
  // Remaining time left for Arduino's internal job
}
