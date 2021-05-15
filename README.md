# FrostyFix 2
A tool to fix Frosty Support with games on platforms other than Origin (EA Desktop, Epic Games Store, Steam)
#### v2.3.0 and above are Required if you use the Frosty Alpha

![image](https://i.imgur.com/6h3ngWV.png)

## Instructions

### [Download the latest version here](https://github.com/Dulana57/FrostyFix/releases)

1. Launch FrostyFix
2. Pick game (or choose custom)
3. Press Enable Mods or Disable Mods
4. IMPORTANT: Choose one of the **After Patch Options**
5. Launch game through Frosty

[Video Tutorial](https://www.youtube.com/watch?v=-oUOCvFBGLs)

## Info
#### **You must disable this whenever you play any other Frostbite game or if you encounter issues with other games.**
It is recommended to launch the game with Frosty after forcing mods.
It works directly from Origin/EA Desktop but it's better to launch Battlefront from Frosty Mod Manager/Editor to guarantee everything is working fine and to refresh your mod list.
**You must run this program again every time you want to play another Frostbite game**, so you can either disable mods or select the other game.

## FAQ

> **Q:** What issues does FrostyFix solve?
> 
> **A:** If you own a Frosty supported game on Steam, Epic Games Store, or use EA Desktop, **Frosty may launch the game without any mods applied**. This has something to do with how command line arguments are handled in platforms other than Origin. FrostyFix sets an environment variable instead of using commandline arguments which makes the platform a game on irrelevent.

> **Q:** Frosty is giving me an error, how do I use FrostyFix to solve this?
> 
> **A:** If Frosty is giving you an error, then it is an issue with your mod setup or how your Frosty is configured. FrostyFix was not created to solve those issue.

> **Q:** Mods are still not working after FrostyFix
> 
> **A:** Make sure to click on either **Restart Origin** or **Restart EA Desktop** in the After Patch Options (Depending on the platform you use. If that does not work, try restarting Origin or EADesk again. If it still does not work, it is recommended to restart your PC and launch the game after that.

> **Q:** When I try to launch game other than the one I chose in FrostyFix, I get an error
> 
> **A:** You **MUST** disable mods before playing any other Frostbite game 

> **Q:** Why are some games greyed out?
> 
> **A:** If FrostyFix cannot locate a game, it greys out the option making it unselectable to prevent an empty variable to be set.

## Unsupported Games
These games are confirmed to be unsupported because they DO NOT USE a environment variable unlike most other Frostbite Games. They will not be able to be fixed through FrostyFix
- FIFA 20
- Madden 20
- Need For Speed: Heat

## FrostyFix 1.0
The old FrostyFix [Github](https://github.com/Dulana57/FrostyFix-Old)

## Support
For more information and/or support about FrostyFix, join [my Discord Server](https://discord.gg/57sJ6fj) or join the [BattlefrontModding Discord Server](https://discord.gg/EzXSJfUDmq)

## Credits
Created by [Dulana57](https://Dulana57.com), based off the batch file by [BattleDash](https://forums.nexusmods.com/index.php?/topic/9737483-a-permanent-solution-to-epic-games-modding/), [Dulana57](https://Dulana57.com), and [VictorPLopes](https://twitter.com/VictorPL2003/)
