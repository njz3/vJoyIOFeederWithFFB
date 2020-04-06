/*
  Management of configuration for IO board
*/
#include "Protocol.h"
#include "Globals.h"
#include "Utils.h"
#include "PlatformSpecific.h"

namespace Protocol {

void SetupPort()
{
  // initialize serial communications at maximum baudrate bps:
  long baudrate = 1000000;
  switch(Config::ConfigFile.SerialSpeed) {
    case Config::COM57600:
      baudrate = 57600;
      break;
    case Config::COM115200:
        baudrate = 115200;
        break;
    case Config::COM250000:
        baudrate = 250000;
        break;
    case Config::COM500000:
        baudrate = 500000;
        break;
    case Config::COM1000000:
        baudrate = 1000000;
        break;
  }
  Serial.begin(baudrate);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB
  }
  Serial.setTimeout(2);
}


void DebugMessageFrame(String debug)
{
  if (!Globals::VolatileConfig.DebugMode)
    return;
  Serial.print("M");
  Serial.println(debug);
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
  sprintf(buff, Iformat, Globals::Controller.Buttons&0xFF);
  Serial.print(buff);
  // Second 8
  sprintf(buff, Iformat, (Globals::Controller.Buttons>>8)&0xFF);
  Serial.print(buff);
#else
  // First 8
  Utils::ConvertToNDigHex(Globals::Controller.Buttons&0xFF, 2, buff);
  Serial.write('I'); Serial.write(buff, 2);
  // Second 8
  Utils::ConvertToNDigHex((Globals::Controller.Buttons>>8)&0xFF, 2, buff);
  Serial.write('I'); Serial.write(buff, 2);
#endif

#ifdef ARDUINO_AVR_MEGA2560
  // Second 8x Digital inputs on PORTC (pins 30-37)
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Iformat, (~PINC)&0xFF);
  Serial.print(buff);
#else
  Utils::ConvertToNDigHex(~PINC, 2, buff);
  Serial.write('I'); Serial.write(buff, 2);
#endif
#endif

  // Analog inputs, 3 nibbles
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Aformat, Globals::Controller.Steer&0xFFF);
  Serial.print(buff);
  sprintf(buff, Aformat, Globals::Controller.Accel&0xFFF);
  Serial.print(buff);
  sprintf(buff, Aformat, Globals::Controller.Brake&0xFFF);
  Serial.print(buff);
  sprintf(buff, Aformat, Globals::Controller.Clutch&0xFFF);
  Serial.print(buff);
#else
  Utils::ConvertToNDigHex(Globals::Controller.Steer, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
  Utils::ConvertToNDigHex(Globals::Controller.Accel, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
  Utils::ConvertToNDigHex(Globals::Controller.Brake, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
  Utils::ConvertToNDigHex(Globals::Controller.Clutch, 3, buff);
  Serial.write('A'); Serial.write(buff, 3);
#endif

  // Additionnal states for 1st input (wheel)
  uint32_t* vel_hex = (uint32_t*)&Globals::Controller.SteerVel;
  uint32_t* acc_hex = (uint32_t*)&Globals::Controller.SteerAcc;
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Fformat, *vel_hex, *acc_hex);
  Serial.print(buff);
#else
  Serial.write('F');
  Utils::ConvertToNDigHex(*vel_hex, 8, buff);
  Serial.write(buff, 8);
  Utils::ConvertToNDigHex(*acc_hex, 8, buff);
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


void ProcessOneMessage()
{
  if (Serial.available()>0) {
    char msg[64];
    
    size_t read = Serial.readBytesUntil('\n', msg, sizeof(msg));
    if (read>0) {
      // Enforce null-terminated string (remove '\n')
      msg[read] = 0;
            
      size_t index = 0;
      int DigOut_block = 0;
      
      while(index<read) {
        
        switch(msg[index++]) {
        case 'D': {
          Globals::VolatileConfig.DebugMode = !Globals::VolatileConfig.DebugMode;
          DebugMessageFrame("Debug mode ON");
        } break;
        case '?': {
          // Handshaking!
          char buff[32];
          // Read protocol version
          char *sc = (char*)(msg+index);
          int major = Utils::ConvertHexToInt(sc, 4);
          int minor = Utils::ConvertHexToInt(sc+4, 4);
          DebugMessageFrame("recv major=" + String(major,HEX) + " minor=" + String(minor,HEX));
          if (major>=PROTOCOL_VERSION_MAJOR) {
            DebugMessageFrame("handshaking ok");
          }

          // Send protocol version - hardcoded
          sprintf(buff, "?%04X%04X\n", PROTOCOL_VERSION_MAJOR, PROTOCOL_VERSION_MINOR);
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
          Utils::SoftwareReboot();
        } break;
        case 'C': {
          // configuration option, no space allowed
          // Creset: reset config and save
          // Cread: read eeprom
          // Cwrite: write eeprom
          // Ccenteredpwm=1: set PWM centered
          // Cdualpwm=1: set dual PWM (for L620X)
          String line = String((char*)&msg[0]+index);
          String command = Utils::GetValue(line, '=', 0);
          if (command.equals("reset")) {
            Config::ResetConfig();
            Config::SaveConfigToEEPROM();
            SendMessageFrame("EEPROM reset and write"); 
          } else if (command.equals("read")) {
            Config::LoadConfigFromEEPROM();
            SendMessageFrame("EEPROM load"); 
          } else if (command.equals("write")) {
            Config::SaveConfigToEEPROM();
            SendMessageFrame("EEPROM write"); 
          } else if (command.equals("centeredpwm")) {
            String value = Utils::GetValue(line, '=', 1);
            SendMessageFrame("value=" + value);               
            if(value.equals("1"))
              Config::ConfigFile.PWMMode |= CONFIG_PWMMODE_CENTERED;
            else
              Config::ConfigFile.PWMMode &= ~(CONFIG_PWMMODE_CENTERED);
            SendMessageFrame("PWMMode=" + String(Config::ConfigFile.PWMMode));
          } else if (command.equals("dualpwm")) {
            String value = Utils::GetValue(line, '=', 1);
            SendMessageFrame("value=" + value);              
            if(value.equals("1"))
              Config::ConfigFile.PWMMode |= CONFIG_PWMMODE_DUAL;
            else
              Config::ConfigFile.PWMMode &= ~(CONFIG_PWMMODE_DUAL);
            SendMessageFrame("PWMMode=" + String(Config::ConfigFile.PWMMode)); 
          }
          index = read;
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
          PlatformSpecific::Do_Init();
          // Send back ready "R" message followed by some comment
          Serial.println("RInitialization done");
          // Reset ticker time
          Globals::Ticker.Init();
        } break;
        
        case 'U': {
          // Send single status frame
          if (!Globals::VolatileConfig.DoStreaming) {
            SendStatusFrame();
          }
        } break;
        
        case 'W': {
          // Start watchdog safety
          Globals::WatchdogSafety.Enabled = true;
          DebugMessageFrame("WD enabled");
          index = read;
        } break;
        case 'T': {
          // Stop Watchdog safety
          Globals::WatchdogSafety.Enabled = false;
          DebugMessageFrame("WD disabled");
          index = read;
        } break;
        
        case 'S': {
          // Start streaming
          Globals::VolatileConfig.DoStreaming = true;
          DebugMessageFrame("Start streaming");
          index = read;
        } break;
        
        case 'H': {
          // Halt streaming
          Globals::VolatileConfig.DoStreaming = false;
          DebugMessageFrame("Stop streaming");
          index = read;
        } break;

        case 'O': { // partially done
          char *sc = (char*)(msg+index);
          int do_value = Utils::ConvertHexToInt(sc, 2);
          switch(DigOut_block) {
            case 0:
              // direction/enable
              Globals::Controller.ForwardCmd = (do_value>>0)&0x1;
              Globals::Controller.ReverseCmd = (do_value>>1)&0x1;
              // 6 Lamps
              Globals::Controller.Lamps = (do_value>>2)&0xF;
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
          Globals::Controller.TorqueCmd = Utils::ConvertHexToInt(sc, 3);
          DebugMessageFrame("pwm=" + String(Globals::Controller.TorqueCmd, HEX));
          index+=3;
        } break;

        case 'E': { // Not yet done
          char *sc = (char*)(msg+index);
          Globals::Controller.Encoder = Utils::ConvertHexToInt(sc, 8);
          DebugMessageFrame("enc=" + String(Globals::Controller.Encoder, HEX));
          index+=8;
        }
        break;

        default:
          SendErrorFrame(0, "UNKNOWN CMD " + String(msg));
          index = read;
          break;
        }
      }
      
      // Refresh WD
      Globals::WatchdogSafety.Revive(Globals::Ticker.Tick);
    }
  }
}

}
