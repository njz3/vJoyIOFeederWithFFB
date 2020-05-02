# vJoyIOFeeder

## What is this about?

This is an exploratory code to manage FFB game effects from a C# feeder application
made with vJoy, using an IO board based on arduino to make physical effects played
on Sega Model2/3 hardware. This work has been done in a few weeks thanks to other
people who paved the road before me.

In particular, this project is strongly based on the amazing work done by 
Shaul Eizikovich who did vJoy with support for force feedback effects.
Without him, this would have never been started!

This vJoy+Arduino strategy is not new and was an idea of BigPanik (M2Pac author)
and SailorSat (DaytonaUSB author) who made the first proof-of-concepts of 
controlling a Sega Mode2/3 drive board from an Arduino.

Compared to their respective developments, this project still misses complete
support for digital outputs to control lamps/relays and also other Sega
DriveBoards with a working translation mode, otherwise we will be leaving many
people off the road.

## What is working?

Currently, analog inputs steering wheel angle and for pedals work.
Force feedback is handled for PWM+Dir mode (pins D9/D10/D11) and Model 2
driveboards (only Lemans, I have bad behavior with Model 3 Scud Race).

Missing effects are emulated by cheating with fast constant-torque commands@5ms (200Hz).
This allows for full effects to be played on your setup, whatever the
underlying driveboard can provide. Emulating requires at least the constant torque
effect to be operationnal on your motor driver (ie a rumble-only setup cannot be used).

Calibration of analog inputs can pe performed from the GUI to map your physical
wheel rotation to the maximum vJoy amplitude (0..32768), same for pedals motion.
Digital inputs are mapped to buttons and remapping is possible using the GUI. 
This allows to finally handle your personnal setup.

Outputs to drive lamps are retrieved for MAME, Supermodel (model 3), m2emulator,
and OutputBlasters plugin games (Teknoparrot). Many thanks to SailorSat& BigPanik
and Boomslangnz for all there work.
Few games are not handled properly yet, but it will probably be fixed sooner or later.

## What is next?

The next steps I plan are:
- add schematics to help people do their cabling
- add more translation modes for other Model2/3 driveboards (SailorSat already has a 
long list of commands)
- support encoder feedback


## How to use it

!!! WARNING as this is still an alpha-stage developpement !!!

Two possibilities are offered : either pick the latest released installer/setup, or
compile the software by your own.

To build the application, please install Visual Studio 2019 Community Edition
with C# for Desktop.

The software expect vJoy 2.2.0 to be installed, so please install it separatly
(see subdirectory tools/vJoySetup_2.2.0 signed.exe).

Next, configure the first virtual joystick using the Configure vJoy tool with
following options:

![vJoy configuration](https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/docs/vJoyConfig.jpg)

**Note:** The 5th axis Dial/Slider2 is only used as a monitoring value to see
how much torque is send to the motor driver when using PWM mode.

## Configuring and cabling the hardware

Depending on your hardware, different options are possible.


#### Model2/3 Drive Board with parallel communication and Arduino Mega2560

!!!TESTED OK WITH A LEMANS!!!

For cabling the Model 3 drive board with a parallel communication bus connected
to an Arduino Mega2560, see
http://superusr.free.fr/model3.htm

Hardcoded wiring on the Arduino Mega2560:
- 8 Buttons are mapped to D2-D8 (seven inputs) and D12 (plus one)
- Wheel "volume" potentiometer is A0
- Accel "volume" is A1
- Brake "volume" is A2
- Clutch "volume" is A3
- PWM output is D9 __only 490Hz!__ (not yet configured for fast PWM at 15,6kHz)
- Direction output is D10 for forward, D11 for backward.
- Pins 22-29 are for 8x digital outputs (PORTA) connected to driveboard RX
- Pins 30-37 are for 8x digital inputs in pull-up mode (PORTC) connected to
driveboard TX

In order to communicate with the DriveBoard, you need to flash the Mega2560 
with the common Ino code (compatible between Leonardo and Mega2560):
https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/FeederIOBoard/FeederIOBoard.ino


#### Model 2 with PWM mode, PWM2M2 and Arduino Leonardo

For PWM2M2 installation, crawl on the web for information.

Hardcoded wiring on the Arduino Leonardo:
- 12 Buttons are mapped to D2-D8 (seven inputs), D12 (plus one) and D0/D1/A4/A5 (four more)
- Wheel "volume" potentiometer is A0
- Accel "volume" is A1
- Brake "volume" is A2
- Clutch "volume" is A3
- PWM output is D9 (configured for fast PWM at 15,6kHz)
- Direction output is D10 for forward, D11 for backward.

If using PWM mode to control a Model 2 motor with PWM2M, use an Arduino 
Leonardo, flash it with the common Ino code (compatible between Leonardo
and Mega2560):
https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/FeederIOBoard/FeederIOBoard.ino


#### Lindbergh Drive Board with RS232-RS422 and Aganyte's FFB Converter board (Arduino Mega2560)

Not yet done

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


## Configurating the application

Once the application is run and then closed, an XML configuration file is created to
your %APPDATA% directory, which usually maps to :
`C:\Users\LOGIN\AppData\Roaming\vJoyIOFeeder\`


## Frequent question 

The FAQ is in a separate file [here](FAQ.md).
