#!DeviceUpdateScript

# Player environment update script
#
# This script executes as part of game initialization and world
# updates. So it executes every time a game is loaded or saved.
#
# It also runs during special player state changes, namely:
# - skill tree updates
# - after "Skip Time" events
# - world flag updates
#
# This is a place where you can update the state of the player's computer
# as well as set and unset player flags.
#
# Player flags are just like world flags, but mainly affect parts of Socially
# Diistant's user interface. They're used to control what the player is allowed
# to do in the virtual environment. Unlike world flags, they can also store numeric
# values. 
#
# Note: PlayerUpdateScript runs in the same context as WorldScript, so you have access
# to all commands you would in a world update script.

# binds this script to the player, this will fail if a game isn't active.
device player

# if we're in a debug world, give a shit ton of money to the player and
# and other useful points for testing
if worldflag get DEBUG
then
  playerflag BANK_BALANCE 1000000
  playerflag EXPERIENCE 50
  playerflag REPUTATION 0
fi

# if we're in a debug save file or the "Skip Tutorials" setting is set in the hypervisor,
# make sure the tutorial daemon shuts off. Otherwise, turn it on.
if worldflag get DEBUG || registryctl get-bool com.sociallydistant.gameplay.SkipTutorials
then
  daemon stop Daemons/TutorialManager
else
  daemon start Daemons/TutorialManager
fi