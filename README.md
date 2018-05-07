# AVS - Auto Volume Switcher for Creative Sound Blaster Z

This little tool automatically switches Windows master volume between two values (for headphones and speakers) when headphones are plugged into / unplugged from the Creative Sound Blaster Z front panel.

Current volume is saved and bound to the relative device on switch. Default values are 100 for speakers and 10 for headphones.

Based on NAudio: https://github.com/naudio/NAudio.

## How to "install"

Compile the project or download the [archive](https://github.com/DSAureli/AVS/releases/download/v1.0.1/AVS.v1.0.1.rar) to get the three essential files:
- AVS.exe
- AVS.exe.config
- NAudio.dll

Put them together in a directory you won't touch ever again, I suggest something like
> C:\Program Files\AVS

Then create a shortcut to AVS.exe and place it into
> C:\Users\\\<Your username>\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup

AVS will be fired up at system startup and will show up in the taskbar with an S or H icon, according to the current active audio device.
