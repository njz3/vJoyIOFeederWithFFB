# Frequently Asked Questions

(FR)
1. Réinstallation de vJoy, plus de FFB : si vous avez réinstallé vJoy, alors vous devez refaire la configuration de vJoy via vJoy Configure : reset, puis remettre la config, puis Apply
2. FFBPlugin de Boomslangnz: cochez (activez) le mode Alternative FFB. Si vous avez un gimx, alors laissez décoché.
3. Mon volant oscille de gauche à droite dans certains jeux (Daytona 1 et 2): baissez le Global Gain et ajoutez une petite deadband (zone morte) de 0.05
4. J'ai mis le bazar dans vJoyIOFeeder, comment réinitialiser la config ? Allez dans %appdata%\vJoyIOFeeder et supprimez les fichiers xml
5. Parfois il y a un gros ralentissement et un décalage dans la position du volant et les effets : votre PC est peut être à bout (CPU à 100%) et le décodage des trames entre le PC et l'arduino a pris du retard. Soit vous baissez la qualité du jeu, soit vous devez changer de config PC.


## Per emulator hints:

#### Enable force feedback
- For MAME, Nebula Model2, Supermodel (model3) and Teknoparrot: use FFB Plugin
from Boomslangnz https://github.com/Boomslangnz/FFBArcadePlugin

#### Changing vJoy to be seen as a wheel and not a gamepad:
- Crazy Taxi 3 PC: to enable vJoy to be seen


#### Lamps Output
- For MAME: activate either Windows output system (command line argument :
`-output windows`) or TCP network output system on port 8000 (command
line argument : `-output network`)

- Nebula Model2 and Teknoparrot: use OutputBlaster Plugin from Boomslangnz
 https://github.com/Boomslangnz/OutputBlaster then use either MAME


## Per game help/hints:
- MAME Virtua Racing: to get FFB, set upright cabinet in service menu. See 
http://forum.arcadecontrols.com/index.php?topic=145454.0

- Crazy Taxi 3 PC: change to wheel (see above)


- MAME Outrun: set top upright

