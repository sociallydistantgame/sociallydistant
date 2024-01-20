#!WorldScript
hookexec BeforeCreateCareerISPs

# Spawn Internet Service Providers here.
#
# To spawn a new ISP, use the spawnisp command. The first parameter is the
# narrative identifier for the ISP, which will be stored in the world data. If an existing
# ISP with the same narrative ID exists, it will be updated instead of created.
#
# The second parameter is a fallback display name for the ISP. If the game can't find an ISP asset
# or modded ISP definition with a matching narrative ID, then the fallback name will be used.
# 
# This allows one to quickly add new ISPs to the game without needing to fill out a full definition
# for it until you're ready to.
spawnisp district_fairview_isp "Lunartel Communications, Inc."

hookexec AfterCreateCareerISPs

# Setting the player ISP
#
# In order for the player to have access to the Internet, they need
# to connect to an ISP. How this is done depends on a world flag.
#
# If we're in a debug save file (the DEBUG flag is set), then we
# hardcode the player's ISP. Otherwise, we check if the player has
# completed their lifepath quest. 
#
# In the case where the lifepath quest hasn't been started, we do nothing.
# The quest itself determines the ISP the player connects to.
# 
# If the lifepath quest has been completed, then we check if the player already
# has an ISP set up. If not, then we set the starting ISP for Act 1 of the narrative.
function set_debug_isp() {
  setplayerisp district_fairview_isp
  exit 0
}

function set_act1_isp() {
  setplayerisp district_fairview_isp
  exit 0
}

worldflag run-if-set DEBUG set_debug_isp
worldflag run-if-set LIFEPATH_QUEST_COMPLETED set_act1_isp





