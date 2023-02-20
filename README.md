## Socially Distant
This is the open-source base framework for Socially Distant, an up-coming hacking game for PC on Steam. This repository doesn't contain the game's Career mode or any assets related to it.

You should not treat this version of the game as a trial/demo of Socially Distant, it does not contain enough of the actual game to be used as such. You can, however, use it as a launchpad for building community mods and Steam Workshop assets.

### Getting started

To get the game running:

1. Install Unity 2021.3.12f1.
2. Install a .NET IDE
3. Clone the repository and open it in Unity Hub.
4. Wait.

When Unity Editor launches, navigate to "Quick Actions" -> "Enter Bootstrap Scene" to load the initial bootstrap scene. This will boot you into the in-game OS with a fresh, temporary world for testing. Only the filesystem of the player's computer will be saved, as well as the game's global settings in the Registry.

### Career mode?

If you are a core maintainer of the Steam version of Socially Distant, you should be both a member of this organization and the game's Steamworks app. You should have access to a private repository containing the game's Career Mode data.

To set up a Career-enabled development environment:

1. Follow the Getting Started section above to get Unity running.
2. Open a terminal to the game's repository, then `/Assets/GameModes`.
3. Run: `git clone https://github.com/alkalinethunder/SociallyDistantCareer Career`.
4. Wait for Unity to see the new `/Assets/GameModes/Career` folder and import its assets.

You will then see a new entry in the Quick Actions menu to "Enter Career Bootstrap". This will load the Career Bootstrap scene, which will bootstrap Steamworks integration and other core systems needed for a paid version of Socially Distant. You will need to be signed into Steam and own the game.

### Programming conventions

All code in Socially Distant must follow these conventions, or I will crush you.

#### Prefer properties over public fields

Unless you are working in a `struct`, use public properties in place of public fields - i.e:

```csharp
// Bad
public class MyClass
{
    public int RamUsage;
}

// Good
public class MyClass
{
    public int RamUsage { get; set; }
}
```

Public properties allow for explicit access control, allowing writes to be restricted and validated.

#### Nullable reference types

Use them.

#### Public Inspector fields

**Do not** use them. Use private fields marked with `[SerializeField]` instead. If needed, expose the serialized value as a public get-only property.

```csharp
// Bad
public class GameBootstrap : MonoBehaviour
{
    public GameManagerHolder gameManager = null1;
}

// Good
public class GameBootstrap : MonoBehaviour
{
    [SerializeField]
    private GameManagerHolder gameManager = null!;
}
```

This prevents unintentional modification of serialized values, you almost will never intentionally do it.

#### Getting components

Use `MustGetComponent` instead of `GetComponent()` when the component is mandatory for the script's functionality. This will retrieve the required component and assert in the console if no such component could be found. Saves you a bunch of repetitive null-checks.

This, and other helper functions, are provided by our lovely friends at Trixel Creative as part of their core libraries. We're lucky to have them, they make our lives easier, so use them.

#### Non-nullable serialized references

Use `this.AssertAllFieldsAreSerialized()` to check all mandatory serialized references and assert on any that are missing or null. It will print an error to the console saying which serialized references are invalid.

Example:

```cs
public class GameBootstrap : MonoBehaviour
{
    [SerializedField]
    private GameManagerHolder gameManager = null!; // Mandatory
    
    [SerializeField]
    private string? gameName; // Optional
    
    private void Awake()
    {
        this.AssertAllFieldsAreSerialized(typeof(GameBootstrap));
    }
}
```

This will skip over any fields marked as nullable references.