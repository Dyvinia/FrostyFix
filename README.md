# FrostyFix
[![Latest Release](https://img.shields.io/github/v/release/Dyvinia/FrostyFix?style=for-the-badge&labelColor=270943&color=8f35e3&label=Release&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAAsTAAALEwEAmpwYAAAA9klEQVQ4jaXTsSuFYRTH8c/LzSL/h0yUlMUiFkpZLJdF/gIjZbiSwWAWgygmFhMyWA1sSjFK2QymW6/hPurEe9/73u5Znt9zzvl9e55TJ8vzXC9Rg6P5j6LaNDaTvsUZ3v429ZXAczQxhW28Yq0bwB1m0I/9lDvAelVAjJeg9zDcDWA8GlLUMYTBToA5PGgNdBEZdrCBL2yVARZwlfQALpO+TmcDu7U25iWcJ/2MkVC7xxieKJ5BPZg/tWYQI/81FwFWcJJ0E6P4bvPKf4BVHIf7BN7LzBHQwGHIz+KxkzkCYvMybqqYScuEC63/T+K0qhmyXtf5Bx+4LodbDvzpAAAAAElFTkSuQmCC)](https://github.com/Dyvinia/FrostyFix/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/Dyvinia/FrostyFix/total?style=for-the-badge&labelColor=270943&color=8f35e3&label=Downloads&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAhUlEQVQ4jWP8//8/AyWAiSLdDAwMLMicuT4vkLnoTmOEMZK3SFDPBYPPAC0GBoZXDJj+Z4CKvYKqwWnANQYGBgcGBobXWAx4DZW7hs8AXIZg1YzLAHRDcGpmYEBLBzgMgbGxAnwG4NUIA+heWMAACW18eAE+FyxgYGA4QMDSB8gcxgHPjQC6HSiHjNpftAAAAABJRU5ErkJggm9ucyAoQ29weXJpZ2h0IEdvb2dsZSBJbmMuKfG0k74AAABXdEVYdExpY2Vuc2UATGljZW5zZWQgdW5kZXIgQXBhY2hlIExpY2Vuc2UgdjIuMCAoaHR0cDovL3d3dy5hcGFjaGUub3JnL2xpY2Vuc2VzL0xJQ0VOU0UtMi4wKePXdBsAAAAASUVORK5CYII=)](https://github.com/Dyvinia/FrostyFix/releases)

FrostyFix is a tool to fix an issue with modding games on platforms other than Origin (EA Desktop, Epic Games Store, Steam), where mods do not appear in the game.

![image](https://i.imgur.com/u2FzcIp.png)

#### New Alternative [DatapathFix](https://github.com/Dyvinia/DatapathFixPlugin) with full game support

## Instructions

### [Download the latest release](https://github.com/Dyvinia/FrostyFix/releases)

1. Download FrostyFix
2. Apply your chosen mods normally through Frosty Mod Manager and launch the game through it.
   <br>**Your mods will not show up in game, don't worry**
3. Exit the Game and Launch FrostyFix
4. Select Game and Profile
5. Select the Platform you intend to use
6. Click Enable/Launch
7. To Disable FrostyFix, click **Disable Mods**

## .NET 6 Runtime
This application requires [.NET 6 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)<br>
![image](https://i.imgur.com/KDpUsFc.png)

## Reminder
#### **You must disable this whenever you play any other Frostbite game or if you encounter issues with other games.**
**You must run this program again every time you want to play another Frostbite game**, so you can either disable mods or select the other game.

## FAQ

> **Q:** What issues does FrostyFix solve?
> 
> **A:** If you own a Frosty supported game on Steam, Epic Games Store, or use EA Desktop, **Frosty may launch the game without any mods applied**. This has something to do with how command line arguments are handled in platforms other than Origin. FrostyFix sets an environment variable instead of using commandline arguments which makes the platform a game on irrelevent.

> **Q:** Frosty is giving me an error, how do I use FrostyFix to solve this?
> 
> **A:** If Frosty is giving you an error, then it is an issue with your mod setup or how your Frosty is configured. *FrostyFix was not created to solve those issue.*

> **Q:** When I try to launch game other than the one I chose in FrostyFix, I get an error
> 
> **A:** You **MUST** disable mods before playing any other Frostbite game 

> **Q:** Why are some games missing?
> 
> **A:** If FrostyFix cannot locate a game, it hides the game in the dropdown. ***You can add a custom game to the dropdown in the Settings window.***

## Credits
Created by [Dyvinia](https://twitter.com/Dyvinia)

[![Donate-Kofi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/J3J63UBHG)
