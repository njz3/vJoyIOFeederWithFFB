# Sega driveboard protocol

## List of Board/EPROM

BigPanik maintains an accurate list of Sega's driveboard and their EPROM version.
See here:

![BigPanik's reference](https://hico-srv022.pixhotel.fr/sites/default/files/gamoovernet/20191030170142-BigPanik-E7E552EE-1CF1-4DCB-9EE6-4259324646A0.png)

## Known command codes

Here are good pointers for documentation about the protocols:
- BigPanik's reverse engineering of Daytona 2: http://superusr.free.fr/model3.htm
- SailorSat reverse engineering and translation of most of Sega Model 2/3 command codes: https://github.com/SailorSat/daytona-utils/blob/master/src/src/DriveTranslation.bas

### Daytona 2 protocol (EPR-20985)
See BigPanik's documentation: http://superusr.free.fr/model3.htm

## Indy 500, Touring Car, OverRev, SuperGT 24h, Le Mans 24 (EPR-18261)

## Scud Race, Dirt Devil, Emergency Call Ambulance (EPR-19338A)

Effects sequences are started with a command code 0x0X where X is the sequence id.
To stop any ongoing sequence a 0x00 (or 0x08) command is sent.
Only one sequence may run at a time.
Other effects (0x1X, 0x3X, ...), can be cumulated while a sequence is playing.
Other effects (except constant torque) have a duration limited time, usually 4-5 seconds.
Some commands may be encoded on only 3 bits in the last nible, meaning duplicate value can also be used. For example 0x01 or 0x09 is identical.

The Intensity of an effect is split over 2 nibbles on different command codes. 
The most significant part  (between 0x0..0x7) is used for the effect command code, and a finer value is used with 0x2X command code (0x0 .. 0xF values).
For example, to get the maximum torque effect on the right direction, first send 0x57 + 0x2F.

Turning the steering wheel "right", means the steering wheel turns in the clockwise direction.
Turning the steering wheel "left", means the steering wheel turns in the counter-clockwise direction.


| Command | Description                                                   |  Condition    |
|:-------:|:-----------------------------------------------------------------|:--------------------|
|  0x00   | Stop any sequence                                                                                                     | In game effect command |
|  0x01   | Sequence: light random direction shocks (very short impulse of torque) in the steering wheel, like driving on gravel  | In game effect command |
|  0x02   | Sequence: strong random direction shocks (very short impulse of torque) on the steering wheel, like driving on rocks  | In game effect command |
|  0x03   | Sequence: small random direction pulses of torque (0.5s?) on the steering wheel, like driving on grass                | In game effect command |
|  0x04   | Sequence: medium random direction pulses of torque (0.5s?) on the steering wheel, like driving on clay                | In game effect command |
|  0x05   | Sequence: soft taps on the steering wheel, when turning it only, like feeling the steering rack of the transmission            | In game effect command |
|  0x06   | Sequence: high vibrations pushing the steering wheel to the left, like driving on the left edge of a track            | In game effect command |
|  0x07   | Sequence: high vibrations pushing the steering wheel to the righ, like driving on the right edge of a track           | In game effect command |
|  0x08-0x0F | Identical to the above sequences (only the last 3bits are actually used)                                           | In game effect command |
|  0x1X   | Spring Effect. 0x10 to stop the effect. Intensity range from  0x11 (weak) to 0x17 (strong). Values are duplicated on 0x18-0x1F          | In game effect command |
|  0x2X   | Intensity gain for effects (not sequence!). When a torque effect or a vibration effect is played, allows to modulate its intensity. 0x20: | In game effect command |
|  0x3X   | Vibration effect. 0x30 to stop the effect. Intensity range from  0x31 (weak) to 0x35 (strong)                         | In game effect command |
|  0x4X   | Power-slide effect. Perform a sine of torque that increase the torque and push the steering wheel in one direction, then to the other direction, like driving in a hole. 0x40-0x47 push first right, then left. 0x48-0x4F pushes first left, then right. Intensity is given by the 3bits less significant bits | In game effect command |
|  0x5X   | Constant Torque effect. 0x50 stops any torque. 0x51..0x57 pushes the steering wheel to the right. To stop either set 0x50, or 0x10.           | In game effect command |
|  0x6X   | Constant Torque effect. 0x60 stops any torque. 0x61..0x67 pushes the steering wheel to the left. To stop either set 0x50, or 0x10.           | In game effect command |
|  0x8X   | Test mode commands. Not yet investigated.                                                                          | Initialization command |
|  0x8X   | Game mode commands (see below).                                                                                      | Initialization command |
|  0xC6   | Sequence: Reset effects and sequence                                                                          | Initialization command |
|  0xC7   | Perform initialization                                                                                        | Initialization command  |
