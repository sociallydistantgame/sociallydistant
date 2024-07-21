> [!WARNING]
> Socially Distant's development is being moved to [GitLab](https://gitlab.acidiclight.dev/sociallydistant/sociallydistant). I will be archiving this repository and will no longer accept pull requests or issues here. Please direct any contributions to GitLab. :) 

# Socially Distant
Socially Distant is an up-coming hacking game following the story of a global ransomware attack disrupting a society forced into cyberspace by the spread of a deadly biological threat to humanity.

## Unity
This game is made using Unity, and uses Unity 2021.3.35f1 LTS.

## Running the game from source
It's not the best way to play the game, and more intended for development and modding, but you can absolutely run the game from source. Here's how.

1. Download the version of Unity Editor listed under `Unity` above, for your platform.
2. Clone this repo, or fork and clone that.
3. Open the project in Unity.
4. Navigate to `Scenes` in the asset browser, and open the `Bootstrap` scene.
5. Hit Play at the top and hurry up and wait, while it builds asset bundles the first time.

> [!WARNING]
> On Linux, there is a longstanding bug where Unity will hang during asset import.
> If you run into this, you will need to work around it. See the section below for
> details.

## Working around asset import bugs
Unity has a long-standing bug on Linux that causes asset importing to freeze. To fix this, do the following:

1. Kill Unity if it's open

```bash
killall -9 Unity
```

2. In a Terminal, navigate to `~/Unity/Hub/Editor/<VERSION>/Editor/Data`
3. Rename the `bee_backend` file to `bee_backend_real`

```bash
mv bee_backend bee_backend_real
```

4. Create a new `bee_backend` file and paste the following script into it.

```bash
touch bee_backend
chmod +x bee_backend
nano bee_backend
```

Put this script in:

```bash
#!/bin/bash

args=("$@")
for ((i=0; i<"${#args[@]}"; ++i))
do
    case ${args[i]} in
        --stdin-canary)
            unset args[i];
            break;;
    esac
done
${0}_real "${args[@]}"
```

5. Try opening the game again

## Accepting contributions!
Feel free to submit pull requests to the game. If merged, they will be shipped in the next Steam release of Socially Distant. For more info, see the `CONTRIBUTING.md` and `LICENSE` files in the repo root.

> [!NOTE]
>All contributors must sign the Developer Certificate of Origin before any pull requests are merged. More information is in `/CONTRIBUTING.md`.

## Project structure

- All game scripts are in `/Assets/Scripts`
- Core APIs and packages are in `/Packages`
- Compiled asset bundles are in `/Assets/StreamingAssets`
- To-be-bundled assets are in `/Assets/Assets`, with each subfolder representing a separate asset bundle
- Scenes are in `/Assets/Scenes`. The game's entry-point is `/Assets/Scenes/Bootstrap`

## SDSH scripts
Many parts of the game use `sdsh` (**S**ocially **D**istant **SH**ell) scripts. These are `.sh` files in the bundled assets directory. They use a POSIX-like shell scripting language that the game interprets. When scripting gameplay encounters like missions and NPC interactions, always prefer `sdsh` over C#.
