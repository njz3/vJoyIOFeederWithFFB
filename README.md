# vJoyIOFeederWithFFB

Exploratory code to manage FFB game effects from a C# feeder application made
with vJoy, using an IO board based on arduino to make physical effects played
on Sega MOdel2/3 hardware.

This is strongly based on the amazing work done by Shaul Eizikovich.
Without him, this would never been started!

First install vJoy 2.1.9.1 (see subdirectory tools/vJoySetup_2.1.9.1.exe).

Next, configure the first virtual joystick with following options:

![vJoy configuration](https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/docs/vJoyConfig.jpg)

**Note:** The 4th axis Dial/Slider2 is only used as a monitoring value to see
how much torque is send to the motor driver when using PWM mode.

## Configuring and cabling the hardware

Depending on your hardware, different options are possible.

#### Model 2 with PWM mode, PWM2M2 and Arduino Leonardo


For PWM2M2 installation, crawl on the web for information.

Hardcoded wiring on the Arduino Leonardo:
- 8 Buttons are mapped to D2-D8 (seven inputs) and D12 (plus one)
- Wheel "volume" potentiometer is A0
- Accel "volume" is A1
- Brake "volume" is A2
- PWM output is D9 (configured for fast PWM at 15,6kHz)
- Direction output is D10 for forward, D11 for backward.

If using PWM mode to control a Model 2 motor with PWM2M, use an Arduino 
Leonardo, flash it with the common Ino code (compatible between Leonardo
and Mega2560):
https://github.com/njz3/vJoyIOFeederWithFFB/blob/model3_mega2560/FeederIOBoard_mega2560/FeederIOBoard_mega2560.ino


#### Model3 Drive Board with parallel communication and Arduino Mega2560

For cabling the Model 3 drive board with a parallel communication bus connected
to an Arduino Mega2560, see
http://superusr.free.fr/model3.htm

Hardcoded wiring on the Arduino Mega2560, it is almost like Leonardo:
- 8 Buttons are mapped to D2-D8 (seven inputs) and D12 (plus one)
- Wheel "volume" potentiometer is A0
- Accel "volume" is A1
- Brake "volume" is A2
- PWM output is D9 (configured for fast PWM at 15,6kHz)
- Direction output is D10 for forward, D11 for backward.
- Pins 22-29 are for 8x digital outputs (PORTA) connected to driveboard RX
- Pins 30-37 are for 8x digital inputs in pull-up mode (PORTC) connected to
driveboard TX

In order to communicate with the DriveBoard, you need to flash the Mega2560 
with the common Ino code (compatible between Leonardo and Mega2560):
https://github.com/njz3/vJoyIOFeederWithFFB/blob/model3_mega2560/FeederIOBoard_mega2560/FeederIOBoard_mega2560.ino


#### Lindbergh Drive Board with RS232-RS422 adapter and Arduino Mega2560

Not yet done

## Starting the Feeder application


Open the solution file __vJoyIOFeeder.sln__, modify __FFBTranslatingMode__ in 
vJoyManager.cs according to your needs (default is Model 3 with parallel 
communication and Arduino Mega2560).

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

