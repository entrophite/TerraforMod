TerraforMod
===========

A BepInEx-based gameplay mod for Terraformers


## Included mods

* add resource
* add specific card
* prevent sending game status analytics


## Build info

Tested with game version 1.0.70 (steam) and BepInEx 5.4.21 (downloadable from GitHub)


# How to use

1. Install BepInEx
2. Compile mod source, copy the result TerraforMod.dll into &lt;game root&gt;\BepInEx\plugins (&lt;game root&gt; means the directory with the game's executable `Terraformers.exe`)
3. Launch the game, press <kbd>F2</kbd> (default) to show/hide the mod window


## Q&A

### Pressing F2 had no results; it seems the mod is not loaded?

Some games (seemingly including this one) delete the BepInEx node as an anti-cheating/anti-modding method; try following:

1. check the config file &lt;game root&gt;\BepInEx\configs\BepInEx.cfg (it should be generated with the first game launch after BepInEx installation)
2. set the config `HideManagerGameObject` to true
3. save and re-launch the game


### What does the mod `prevent seding game status analytics` do?

The game automatically sends the game's status to dev's remote server at the end of each turn.
This mod option is by default on to intercept this sending; my main purpose is to help the devs reduce contaminated/cheating statitics.
However I do believe that the devs should run a sanity check before analyzing their data anyway.

