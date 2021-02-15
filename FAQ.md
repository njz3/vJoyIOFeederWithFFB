# Frequently Asked Questions

(FR)
1. _Réinstallation de vJoy, plus de FFB_ :
si vous avez réinstallé vJoy, alors  vous devez refaire la configuration de 
vJoy via vJoy Configure : reset, puis remettre la config, puis Apply.
2. _FFBPlugin de Boomslangnz_ : 
cochez (activez) le mode Alternative FFB. Si vous avez un gimx, alors laissez 
décoché.
3. _Mon volant oscille de gauche à droite dans certains jeux (Daytona 1 et 2)_ :
baissez le Global Gain et ajoutez une petite deadband (zone morte) de 0.05.
4. _J'ai mis le bazar dans BackForceFeeder, comment réinitialiser la config ?_
Allez dans %USERPROFILE%\Documents\BackForceFeeder et supprimez les fichiers xml
5. _Parfois il y a un gros ralentissement et un décalage dans la position du 
volant et les effets_ :
votre PC est peut être à bout (CPU à 100%) et le décodage des trames entre le 
PC et l'arduino a pris du retard. Soit vous baissez la qualité du jeu, soit vous devez changer de config PC.

(EN)
1. _After I reinstalled vJoy, I do not have FFB anymore_: 
if you needed to reinstall vJoy, you need to reconfigure it using the vJoy 
Configure tool: perform a reset, then set your config, finally apply it.
2. _FFBPlugin from Boomslangnz_: 
select the Alternative FFB mode. If you have a GIMX adapter, then you can 
leave Alternative mode unchecked.
3. _My wheel oscillate from left to right in some games (Daytona 1 and 2)_: 
lower the Global Gain and add a small deadband of 0.05
4. _I have completly messed up my configuration in BackForceFeeder, how do I reset it ?_
Go to %USERPROFILE%\Documents\BackForceFeeder and delete all xml files
5. _I sometime have the feeling that there is a huge slowdown and a delay
between my wheel angle and the effects_: 
your computer might be too much overloaded (100% CPU) and the decoding of the
frames between the PC and the Arduino gateway got some delay. 
Either you need to lower the game quality, or upgrade your PC.
6. _Some options in the hardware configuration are greyed out, why ?_
This is because the manager is running (green) and you must first stop it but
clicking on the status button. Conversely, some options are greyed out when
the manager is stopped.

## Per emulator hints:

#### Enable force feedback

- For MAME, Nebula Model2, Supermodel (model3) and Teknoparrot: use FFB Plugin
from Boomslangnz https://github.com/Boomslangnz/FFBArcadePlugin

#### Changing vJoy to be seen as a wheel and not a gamepad:

- Crazy Taxi 3 PC and other PC racing games do not recognize my vJoy as a FFB steering wheel: 
to enable vJoy to be seen as a wheel, patch the registry as explained 
here http://vjoystick.sourceforge.net/site/index.php/forum/5-Discussion/1063-how-richard-burns-rally-detects-a-wheel#3226
or here https://www.gamoover.net/Forums/index.php?topic=42310.msg669263#msg669263


#### Key stroke

- Use DirectInput as the preferred API and check that your game or emulator does
capture these keystroke.
- For MAME, in the configuration specify to use keyboardprovider = dinput (or `-keyboardprovider=dinput` in the command line)

#### Lamps Output

- For MAME: activate either Windows output system (command line argument :
`-output windows`) or TCP network output system on port 8000 (command
line argument : `-output network`)

- Nebula Model2 and Teknoparrot: use OutputBlaster Plugin from Boomslangnz
https://github.com/Boomslangnz/OutputBlaster then use either MAME output system
or network output system.

- For Daytona (Saturn Adds) on model 2 you must use SailorSat's lua script to 
enable outputs. Thanks to her amazing work.
 
- For supermodel (model 3), use `-wide-screen -no-throttle -show-fps 
-input-system=dinput -outputs=win`

## Per game help/hints:

- MAME Virtua Racing: to get FFB, set upright cabinet in service menu. See 
http://forum.arcadecontrols.com/index.php?topic=145454.0

- Crazy Taxi 3 PC and other PC racing games do not recognize my vJoy as a FFB steering wheel:
change registry settings of vJoy to be a wheel instead of a joystick (see above)

- MAME Outrun: set top upright

## Game assets:

I have placed in gameassets/ some scripts or useful files for model2/model3 emulator.
Most are not made by me but gleaned over the internet. Please consider to give credits
to the respective authors.


