# BackForceFeeder

## What is this about?

This is a now stable code to manage FFB game effects from a C# feeder application
made with vJoy, using an IO board based on arduino to make physical effects played
on almost all arcade race cabinets with hardware ranging from Sega Model 2 up to 
recent PC-based cabinets. Some cabinets equiped with DC motors (like Midway's ones)
are also supported through third-party electronics. 

This work has been done in a few weeks thanks to  other people who paved the road 
before me.

In particular, this project is strongly based on the amazing work done by 
Shaul Eizikovich who did vJoy with support for force feedback effects.
Without him, this would have never been started!

This vJoy+Arduino strategy is not new and was an idea of BigPanik (M2Pac author)
and SailorSat (DaytonaUSB author) who made the first proof-of-concepts of 
controlling a Sega Model 2/3 drive board from an Arduino.

Compared to their respective developments, this project has now complete
support for digital outputs to control lamps/relays and also support many
different arcade boards with a working translation mode using either native 
protocols (Sega) or DIY electronics boards to make the translation
(FFB converter, PWM2M2 or PWM2HAPP). This allow playing all force feedback effects
on almost any cabinet, making it a universal PC-based platform.

I started to document Sega's Driveboard and servoboard commands. The information
is placed [in this page](DRIVEBOARD.md).


## What is working?

The software supports 4x Analog inputs for 1 steering wheel and 3 pedals, 12 to 16 
digital inputs for buttons (Optionnaly 8 more for a keypad on the Mega2560).
Force feedback output is handled with following modes :
- analog PWM+Dir (pins D9/D10/D11), analog centered PWM (pins D9)
- digital PWM
- almost all Sega Model 1/2 cabinets using PWM2M2 motor control board: Daytona, Sega Rally
- almost all Sega Model 2/3 driveboards that are clone to Indy500: Super GT, Touring Cars, Le Mans 24, ...
- almost all Sega Model 3 driveboards using the Sega Rally 2 EPROM which has the best torque control capability: Scud Race, Daytona 2, Sega Rally 2, E.C.A, Dirt Devils, Nascar, F355 Challenge, ...
- almost all Sega Naomi, Lingbergh, Ringedge/wide up to the latest PC based servoboard using FFB converter : Initial D series, Outrun 2 series, ...
- almost all Happ or DC-motor based cabinets (Midway, new Sega cabinets) using PWM2HAPP motor control board: Grid, Sega Rally 3, Cruis'n series

In most cases, FFB effects are emulated by cheating with fast constant-torque commands@5ms (200Hz).
This allows for full effects to be played on your setup, whatever the
underlying driveboard can provide. Emulating requires at least the constant torque
effect to be operationnal on your motor driver (ie a rumble-only setup cannot be used).

Calibration of the wheel rotation, and analog inputs can pe performed from the GUI 
to map your physical wheel rotation to the maximum vJoy amplitude (0..32768), same 
for pedals motion.
Digital inputs are mapped to buttons and remapping is possible using the GUI. 
This allows to finally handle your personnal setup.

Outputs to drive lamps are retrieved for MAME, Supermodel (model 3), m2emulator,
and OutputBlasters plugin games (Teknoparrot). 
Already a lot of games are handled properly, and more are added over the time.
Many thanks to SailorSat& BigPanik and Boomslangnz for all there work.

The software also allows to define "control sets" for a unique configuration per game or
per emulator. The control sets store parameters that can be tuned according to each game
or emulator behavior. Using runtime auto-detection based on current running process and
main windows title, the software will automatically switch its configuration according
to the current game playing.

The feeder software also allow to define keystrokes (keyboard emulation). This can be a 
replacement for joytokey or autohotkey in most use case.
In particular, it allows to detect combined panel buttons press to emits special keystrokes like
Alt+F4, or ESC when the user press or maintain panel buttons. 


## What is next?

The next steps I plan are:
- add schematics to help people do their cabling
- add I2c support for lowcost digital IO extensions
- support encoder feedback to get very fine angle resolution, perhaps making a better 
FFB feeling.


## How to use it

Two possibilities are offered : either pick the latest released installer/setup, or
compile the software by your own.

To build the application, please install Visual Studio 2019 Community Edition
with C# for Desktop.

The software expect vJoy 2.2.1 to be installed, so please install it separatly
(see subdirectory tools/vJoySetup_2.2.1 signed.exe).

Next, configure the first virtual joystick using the Configure vJoy tool with
following options:

![vJoy configuration](https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/docs/vJoyConfig.jpg)

**Note:** Only 3 or 4 vJoy axes are useful. The 5th axis Dial/Slider2 is only 
used as a monitoring value to see how much torque is send to the motor driver
when using PWM mode.

## Configuring and cabling the hardware

Depending on your hardware, different options are possible.

#### Model 1/2, Happ or DC-based motors (DIY wheel): PWM mode on Arduino Mega2560 or Leonardo

Analog PWM or Dual PWM output can be used for DIY steering wheels.

For PWM2M2 or PWM2HAPP installation (Sega Model 1/2, Midway cabinets), crawl on the web for information.
The system shall be configured with PWM_CENTERED and digital PWM enabled (serial communication).

Hardcoded wiring on the Arduino Leonardo:
- 12 Buttons are mapped to D2-D8 (seven inputs), D12 (plus one) and D0/D1/A4/A5 (four more)
- Wheel "volume" potentiometer is A0
- Accel "volume" is A1
- Brake "volume" is A2
- Clutch "volume" is A3
- analog PWM output is D9 (configured for fast PWM at 15,6kHz)
- digital ouput for Direction is D10 for forward, D11 for backward.
- for digital PWM, use serial port Tx on D1.

For the mega2560, same wiring except:
- analog PWM output is D9 __only 490Hz!__ (not yet configured for fast PWM at 15,6kHz)

In all cases, use Arduino and flash it with the common Ino code (compatible between Leonardo
and Mega2560):
https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/FeederIOBoard/FeederIOBoard.ino

#### Sega Model2/3 Drive Board with parallel communication and Arduino Mega2560

In this case, you must use a Mega2560 Arduino board.
For cabling the Model 2/3 drive board with a parallel communication bus connected
to an Arduino Mega2560, see
http://superusr.free.fr/model3.htm

Hardcoded wiring on the Arduino Mega2560:
- 8 Buttons are mapped to D2-D8 (seven inputs) and D12 (plus one)
- Wheel "volume" potentiometer is A0
- Accel "volume" is A1
- Brake "volume" is A2
- Clutch "volume" is A3
- Pins 22-29 are for 8x digital outputs (PORTA) connected to driveboard RX
- Pins 30-37 are for 8x digital inputs in pull-up mode (PORTC) connected to
driveboard TX

In order to communicate with the DriveBoard, you need to flash the Mega2560 
with the common Ino code:
https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/FeederIOBoard/FeederIOBoard.ino


#### Lindbergh, Ringedge, PC-based Servo Board with Midi, RS422 or RS232: 

Use Aganyte's FFB Converter board (based on Arduino Mega2560).
This FFB Converter board will translate PWM commands from the feeder application
directly to Sega's proprietary protocols without using another Arduino.
The system shall be configured with PWM_CENTERED and digital PWM enabled (serial communication).


## Starting the Feeder application

If using the installer, simply runs vJoyIOFeedeerGUI and go to the hardware page
to configure your setup.

If building from Visual Studio, open the solution file __vJoyIOFeeder.sln__, 

Build the solution in x64, then run it in Debug mode.
Go to the Log Window to see messages from the internal modules.
Check that you got at least these messages (in below example, the Arduino 
Mega2560 is seen as COM3):
```
10:47:54 | [MANAGER] Program configured for MODEL3_SCUD_DRVBD
10:47:54 | [USBSerial] The following serial ports were found:
10:47:54 | [USBSerial] COM3
10:47:54 | [USBSerial] Attempting to connect each with 115200bauds...
10:47:57 | [MANAGER] Found io board on COM3 version=1.0.0.0 type=IO BOARD ON MEGA2560
```


Check with vJoy Monitor if something is alive (like wheel position feedback).

![vJoyMonitor](https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/docs/vJoyMonitor.jpg)

You can then test the force feedback using tools/fedit.exe and selecting and 
effect you would like to try.

## Mapping buttons, lamps, and defining Keystroke

To be explained...

## Configurating the application

Once the application is run and then closed, an XML configuration file is created to
your "My Documents" directory (%USERPROFILE%\Documents\BackForceFeeder), which usually maps to :
`C:\Users\LOGIN\Documents\BackForceFeeder\`

## Frequent question 

The FAQ is in a separate file [here](FAQ.md).
