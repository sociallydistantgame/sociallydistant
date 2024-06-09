# Socially Distant
Socially Distant is an upcoming hacking game following the story of a global ransomware attack disrupting a society forced into cyberspace by the spread of a deadly biological threat to humanity.

## Unity
This game is made using Unity, and uses Unity 2021.3.35f1 LTS.

## Accepting contributions!
Feel free to submit pull requests to the game. If merged, they will be shipped in the next Steam release of Socially Distant. For more info, see the `CONTRIBUTING` and `LICENSE` files in the repo root.

## Project structure

- All game scripts are in `/Assets/Scripts`
- Core APIs and packages are in `/Packages`
- Compiled asset bundles are in `/Assets/StreamingAssets`
- To-be-bundled assets are in `/Assets/Assets`, with each subfolder representing a separate asset bundle
- Scenes are in `/Assets/Scenes`. The game's entry-point is `/Assets/Scenes/Bootstrap`

## SDSH scripts
Many parts of the game use `sdsh` (**S**ocially **D**istant **Sh**ell) scripts. These are `.sh` files in the bundled assets directory. They use a POSIX-like shell scripting language that the game interprets. When scripting gameplay encounters like missions and NPC interactions, always prefer `sdsh` over C#.
