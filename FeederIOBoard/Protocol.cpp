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




char buff[32];
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
char Iformat[] = "I%02X";
char Aformat[] = "A%03X";
char Fformat[] = "F%08X%08X";
#endif

// https://stackoverflow.com/questions/51752284/how-to-calculate-crc8-in-c
static unsigned char const crc8x_table[] = {
    0x00, 0x31, 0x62, 0x53, 0xc4, 0xf5, 0xa6, 0x97, 0xb9, 0x88, 0xdb, 0xea, 0x7d,
    0x4c, 0x1f, 0x2e, 0x43, 0x72, 0x21, 0x10, 0x87, 0xb6, 0xe5, 0xd4, 0xfa, 0xcb,
    0x98, 0xa9, 0x3e, 0x0f, 0x5c, 0x6d, 0x86, 0xb7, 0xe4, 0xd5, 0x42, 0x73, 0x20,
    0x11, 0x3f, 0x0e, 0x5d, 0x6c, 0xfb, 0xca, 0x99, 0xa8, 0xc5, 0xf4, 0xa7, 0x96,
    0x01, 0x30, 0x63, 0x52, 0x7c, 0x4d, 0x1e, 0x2f, 0xb8, 0x89, 0xda, 0xeb, 0x3d,
    0x0c, 0x5f, 0x6e, 0xf9, 0xc8, 0x9b, 0xaa, 0x84, 0xb5, 0xe6, 0xd7, 0x40, 0x71,
    0x22, 0x13, 0x7e, 0x4f, 0x1c, 0x2d, 0xba, 0x8b, 0xd8, 0xe9, 0xc7, 0xf6, 0xa5,
    0x94, 0x03, 0x32, 0x61, 0x50, 0xbb, 0x8a, 0xd9, 0xe8, 0x7f, 0x4e, 0x1d, 0x2c,
    0x02, 0x33, 0x60, 0x51, 0xc6, 0xf7, 0xa4, 0x95, 0xf8, 0xc9, 0x9a, 0xab, 0x3c,
    0x0d, 0x5e, 0x6f, 0x41, 0x70, 0x23, 0x12, 0x85, 0xb4, 0xe7, 0xd6, 0x7a, 0x4b,
    0x18, 0x29, 0xbe, 0x8f, 0xdc, 0xed, 0xc3, 0xf2, 0xa1, 0x90, 0x07, 0x36, 0x65,
    0x54, 0x39, 0x08, 0x5b, 0x6a, 0xfd, 0xcc, 0x9f, 0xae, 0x80, 0xb1, 0xe2, 0xd3,
    0x44, 0x75, 0x26, 0x17, 0xfc, 0xcd, 0x9e, 0xaf, 0x38, 0x09, 0x5a, 0x6b, 0x45,
    0x74, 0x27, 0x16, 0x81, 0xb0, 0xe3, 0xd2, 0xbf, 0x8e, 0xdd, 0xec, 0x7b, 0x4a,
    0x19, 0x28, 0x06, 0x37, 0x64, 0x55, 0xc2, 0xf3, 0xa0, 0x91, 0x47, 0x76, 0x25,
    0x14, 0x83, 0xb2, 0xe1, 0xd0, 0xfe, 0xcf, 0x9c, 0xad, 0x3a, 0x0b, 0x58, 0x69,
    0x04, 0x35, 0x66, 0x57, 0xc0, 0xf1, 0xa2, 0x93, 0xbd, 0x8c, 0xdf, 0xee, 0x79,
    0x48, 0x1b, 0x2a, 0xc1, 0xf0, 0xa3, 0x92, 0x05, 0x34, 0x67, 0x56, 0x78, 0x49,
    0x1a, 0x2b, 0xbc, 0x8d, 0xde, 0xef, 0x82, 0xb3, 0xe0, 0xd1, 0x46, 0x77, 0x24,
    0x15, 0x3b, 0x0a, 0x59, 0x68, 0xff, 0xce, 0x9d, 0xac
    };

unsigned crc8x_fast(unsigned crc, void const *mem, int len)
{
    unsigned char const *data = (unsigned const char*)mem;
    if (data == NULL)
        return 0xff;
    crc &= 0xff;
    while (len--)
        crc = crc8x_table[crc ^ *data++];
    return crc;
}

// Binary protocol, not yet used
// Header = 
// - Start of frame 24='$' (1byte)
// - commandcode (1 byte) - identify the frame (0xFF reserved for extension)
// - datalength (1 byte) - max 255 bytes in a frame, not including EOF
// - CRC8 (1 byte)
void SendHeader(byte cmdcode, const char*data, byte datalength)
{
  byte crc8 = crc8x_fast(0, data, datalength);
  Serial.write(0x24);
  Serial.write(cmdcode);
  Serial.write(datalength);
  Serial.write(crc8);
}

// Footer = (end of frame)
// - End of line '\n' (1 byte)
void SendEOF()
{
  // Add '\n' for end-of-frame
  Serial.write('\n');
}

void SendXWord(uint32_t val, int ndigits)
{
  char buff[16];
  Utils::ConvertToNDigHex(val, ndigits, buff);
  Serial.write(buff, ndigits);
}

void SendI2XWord(uint16_t din)
{
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Iformat, din&0xFF);
  Serial.print(buff);
#else
  Serial.write('I'); 
  SendXWord(din&0xFF, 2);
#endif
}

void SendA3XWord(uint16_t ain)
{
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Aformat, ain&0xFFF);
  Serial.print(buff);
#else
  Serial.write('A'); 
  SendXWord(ain&0xFFF, 3);
#endif
}

void SendE8XWord(uint32_t enc)
{
  Serial.write('E'); 
  SendXWord(enc, 8);
}

void SendF8X8YWord(uint32_t vel_hex, uint32_t acc_hex)
{
#ifdef USE_SPRINTF_FOR_STATUS_FRAME
  sprintf(buff, Fformat, vel_hex, acc_hex);
  Serial.print(buff);
#else
  Serial.write('F');
  SendXWord(vel_hex, 8);
  SendXWord(acc_hex, 8);
#endif
}


void DebugMessageFrame(String debug)
{
  if (!Globals::VolatileConfig.DebugMode)
    return;
  Serial.print("M");
  Serial.print(debug);
  SendEOF();
}

void SendStatusFrame()
{
  // 2x 8xDigital inputs, 2 nibbles each
  SendI2XWord(Globals::Controller.Buttons);
  SendI2XWord(Globals::Controller.Buttons>>8);

#ifdef ARDUINO_AVR_MEGA2560
  // Second 8x Digital inputs on PORTC (pins 30-37)
  SendI2XWord(~PINC);
#endif

  // Analog inputs, 3 nibbles each
  SendA3XWord(Globals::Controller.Steer);
  SendA3XWord(Globals::Controller.Accel);
  SendA3XWord(Globals::Controller.Brake);
  SendA3XWord(Globals::Controller.Clutch);

  // Additionnal states for 1st input (wheel)
  uint32_t* vel_hex = (uint32_t*)&Globals::Controller.SteerVel;
  uint32_t* acc_hex = (uint32_t*)&Globals::Controller.SteerAcc;
  SendF8X8YWord(*vel_hex, *acc_hex);
  
  // Add end-of-frame ('\n')
  SendEOF();
}

void SendErrorFrame(int code, String msg)
{
  Serial.write('S');
  SendXWord(code, 4);
  Serial.write(' ');
  Serial.print(msg);
  SendEOF();
}

void SendMessageFrame(String msg)
{
  Serial.write("M");
  Serial.print(msg);
  SendEOF();
}


enum Types : byte {
  BYTE = 0,
  UINT8,
  INT8,
  UINT16,
  INT16
};

typedef struct
{
  const char *Key;
  enum Types Type;
  void *pValue;
} DictionaryParamEntry;

DictionaryParamEntry DictionaryParam []= {
  { "pwmmode", BYTE, (void*)&Config::ConfigFile.PWMMode},
  { "serialspeed", BYTE, (void*)&Config::ConfigFile.SerialSpeed},
  { "wheelmode", BYTE, (void*)&Config::ConfigFile.WheelMode},
  { "pedalmode", BYTE, (void*)&Config::ConfigFile.PedalMode},
};

typedef struct
{
  const char *Keyword;
  void (*pHandler)(const String &key);
} DictionaryKeywordEntry;

void ResetCfgHandler(const String &keyval);
void LoadCfgHandler(const String &keyval);
void SaveCfgHandler(const String &keyval);
void GetHandler(const String &key);
void SetHandler(const String &key);
void HelpHandler(const String &key);

DictionaryKeywordEntry DictionaryKeyword []= {
  { "resetcfg", ResetCfgHandler},
  { "loadcfg", LoadCfgHandler},
  { "savecfg", SaveCfgHandler},
  { "get", GetHandler},
  { "set", SetHandler},
  { "help", HelpHandler},
};

void GetHandler(const String &key)
{
  DebugMessageFrame("get key=" + key);
  int count = sizeof(DictionaryParam)/sizeof(DictionaryParam[0]);
  int i;
  for(i=0; i<count; i++) {
    if (key.equals(DictionaryParam[i].Key)) {
      switch(DictionaryParam[i].Type) {
        case BYTE: {
          byte value = *((byte*)DictionaryParam[i].pValue);
          SendMessageFrame(key + "=" + String(value));
        } break;
        case UINT8: {
          uint8_t value = *((uint8_t*)DictionaryParam[i].pValue);
          SendMessageFrame(key + "=" + String(value));
        } break;
        case UINT16: {
          uint16_t value = *((uint16_t*)DictionaryParam[i].pValue);
          SendMessageFrame(key + "=" + String(value));
        } break;
        default: {
          SendErrorFrame(3, "Unknown type " + String(DictionaryParam[i].Type) + " for param " + key);
        } break;
      }
      break;
    }
  }
  if (i==count) {
    SendErrorFrame(3, "Key " + key + " not found");    
  }
}

void SetHandler(const String &keyval)
{
  String key = Utils::GetValue(keyval, '=', 0);
  String value = Utils::GetValue(keyval, '=', 1);
  DebugMessageFrame("set key=" + key + ", value=" + value);
  int count = sizeof(DictionaryParam)/sizeof(DictionaryParam[0]);
  int i;
  for(i=0; i<count; i++) {
    if (key.equals(DictionaryParam[i].Key)) {
      switch(DictionaryParam[i].Type) {
        case BYTE: {
          byte val = (byte)Utils::ConvertHexToInt(value.c_str(), 2);
          *((byte*)DictionaryParam[i].pValue) = val;
          SendMessageFrame("Set byte " + key + " to " + String(val));
        } break;
        case UINT8: {
          uint8_t val = (uint8_t)Utils::ConvertHexToInt(value.c_str(), 2);
          *((uint8_t*)DictionaryParam[i].pValue) = val;
          SendMessageFrame("Set uint8 " + key + " to " + String(val));
        } break;
        case UINT16: {
          uint16_t val = (uint16_t)Utils::ConvertHexToInt(value.c_str(), 4);
          *((uint16_t*)DictionaryParam[i].pValue) = val;
          SendMessageFrame("Set uint16 " + key + " to " + String(val));
        } break;
        default: {
          SendErrorFrame(3, "Unknown type " + String(DictionaryParam[i].Type) + " for param " + key);
        } break;
      }
      break;
    }
  }
  if (i==count) {
    SendErrorFrame(3, "Key " + key + " not found");    
  }
}

void ResetCfgHandler(const String &keyval)
{
  Config::ResetConfig();
  Config::SaveConfigToEEPROM();
  SendMessageFrame("EEPROM reset and write");
}

void LoadCfgHandler(const String &keyval)
{
  Config::LoadConfigFromEEPROM();
  SendMessageFrame("EEPROM load");
}
void SaveCfgHandler(const String &keyval)
{
  Config::SaveConfigToEEPROM();
  SendMessageFrame("EEPROM save");
}

void HelpHandler(const String &keyval)
{
  int i;
  int countkwd = sizeof(DictionaryKeyword)/sizeof(DictionaryKeyword[0]);
  for(i=0; i<countkwd; i++) {
    SendMessageFrame("Keyword " + String(DictionaryKeyword[i].Keyword));
  }
  int countparam = sizeof(DictionaryParam)/sizeof(DictionaryParam[0]);
  for(i=0; i<countparam; i++) {
    SendMessageFrame("Param " + String(DictionaryParam[i].Key) + " type " + String(DictionaryParam[i].Type));
  }
  SendMessageFrame("Help listed " + String(countkwd) + " keywords and " + String(countparam) +" params");
}


// Cresetcfg: reset config and save
// Cloadcfg: load config from eeprom
// Csavecfg: write eeprom
// Cget/set pwmmode=XX: set PWM mode (dual PWM (for L620X))
// Cget/set serial=XX: set serial speed (0..4)
// Cget/set wheelmode=XX: set 
// Cget/set pedalmode=XX: set 
void InterpretCommand(char *pline)
{
  String line = String(pline);
  String command = Utils::GetValue(line, ' ', 0);
  DebugMessageFrame("command=" + command);
  int count = sizeof(DictionaryKeyword)/sizeof(DictionaryKeyword[0]);
  int i;
  for(i=0; i<count; i++) {
    if (command.equals(DictionaryKeyword[i].Keyword)) {
      String remaining = Utils::GetValue(line, ' ', 1);
      DictionaryKeyword[i].pHandler(remaining);
      break;
    }
  }
  if (i==count) {
    SendErrorFrame(3, "Syntax error: " + line);
  }
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
          sprintf(buff, "?%04X%04X", PROTOCOL_VERSION_MAJOR, PROTOCOL_VERSION_MINOR);
          Serial.print(buff);
          SendEOF();
          // frame terminated
          index = read;
        } break;
        case 'V': {
          // Board version - hardcoded
          Serial.print(VERSION_STRING);
          SendEOF();
          index = read;
        } break;
        case '~': {
          // Reset Board 
          Utils::SoftwareReboot();
        } break;
        case 'C': {
          // Command line, until newline
          InterpretCommand(&msg[0]+index);
          index = read;
        } break;
        
        case 'G': {
          // Hardware description - hardcoded
#ifdef ARDUINO_AVR_MEGA2560
          Serial.print("GI3A4O2P1F1"); // For 2560 : idem Leonardo and add 1xDI(x8) and 1xDO(x8)
#else
          Serial.print("GI2A4O1P1F1"); // 2xDI(x8),3xAIn,1xDO(x8),1xPWM, 1xFullstate, 0xEnc
#endif
          SendEOF();
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
          Serial.print("RInitialization done");
          SendEOF();
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
