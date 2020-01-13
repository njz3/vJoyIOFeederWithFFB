# vJoyIOFeederWithFFB

Exploratory code to manage FFB game effects from a C# feeder application made with vJoy, using an IO board based on arduino to make physical effects.

This is strongly based on the amazing work done by Shaul Eizikovich.
Without him, this would never be started!

First install vJoy 2.1.9.1.

Next, configure first virtual joystick with following:

![vJoy configuration](https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/docs/vJoyConfig.jpg)

Flash an Arduino Leonardo with:
https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/FeederIOBoard/FeederIOBoard.ino

Hardcoded wiring :
- 8 Buttons are mapped to D2-D8 (seven) and D12 (plus one)
- Wheel "volume" potentiometer is A0
- Accel "volume" is A1
- Brake "volume" is A2
- PWM output is D9
- Direction output is D10 for forward, D11 for backward.

Open the solution file, modify COM port in ManagingThread.cs. Build solution, then run it in x64 Debug mode.

![COM port](https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/docs/COMport.jpg)

Check with vJoy Monitor if something is alive.

![vJoyMonitor](https://github.com/njz3/vJoyIOFeederWithFFB/blob/master/docs/vJoyMonitor.jpg)


