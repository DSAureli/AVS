# AVS - Auto Volume Switcher for Creative Sound Blaster Z

This little tool automatically switches Windows master volume between two values (for headphones and speakers) when heaphones are plugged/unplugged into the front panel of the Creative Sound Blaster Z sound card. Default values are 100 for speakers and 10 for headphones. Current volume is saved and binded to the appropriate device when plugging or unplugging headphones.

Based on NAudio: https://github.com/naudio/NAudio.

## How to "install"

Compile with Visual Studio or download the [archive](https://github.com/DSAureli/AVS/releases/download/v1.0.0/AVS.v1.0.0.rar) to get the three essential files:
- AVS.exe
- AVS.exe.config
- NAudio.dll

Put them together in a directory you won't touch ever again, I suggest something like
> C:\Program Files\AVS

Then create a shortcut to AVS.exe and place it into
> C:\Users\\\<Your username>\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup

AVS will be fired up at system startup. It shows up in the taskbar with an H or an S icon, according to the current active audio device.
