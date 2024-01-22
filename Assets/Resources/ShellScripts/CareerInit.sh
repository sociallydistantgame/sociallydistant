#!WorldScript

# This script runs every time a save file is loaded or after it's been saved.
# The purpose of the script is to update the game's world based on the player's progression through
# Socially Distant's story.

# Mod hook for injecting code to run before core world flags
hookexec BeforeWorldStateUpdate

# World flags
#
# We will inevitably load a save file created before features or narrative elements were
# added to the game. World flags let us update the save file with new content. To do this,
# add a new 'requireflag' command here. The first parameter is the flag you want to check for,
# and the second argument is the command you want to run should that flag not be set in the save.
#
# World flags won't be set automatically when the command finishes. This allows you to also update
# the save file based on quest completion states, by setting a world flag on completion of a quest. 
worldflag run-if-unset CREATED_CITY_ISPS ShellScripts/Career/CreateISPs
worldflag run-if-unset LIFEPATH_QUEST_COMPLETED ShellScripts/Career/SetupLifepathQuest

# We're done. Let mods run code after we've updated the core world state.
hookexec AfterWorldStateUpdate

# Save changes to the world without triggering the script again.
if [ worldflag get DEBUG ]
then
  echo "Debug save. World will not be saved."
else
  savegame -s
fi

echo End of world update script

# Execute the player update script
# commented out because of missing parser features
#ShellScripts/DeviceScripts/PlayerEnvironmentUpdate