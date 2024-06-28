![TrixelAudio](/Branding/TrixelAudio_Logo_Dark.png)

TrixelAudio is an open-source, robust and feature-rich sound effects and background music system for Unity. It uses a ScriptableObject-based architecture and can be easily brought into any existing Unity project. TrixelAudio is the core framework for all sound-related features in the [Restitched](https://trixelcreative.com/Restitched) game by Trixel Creative.

## Features

 - **Scene-independent:** The TrixelAudio Core can be initialized during game startup and will not destroy itself until the game is shut down.
 - **Built-in Audio Source pool**: We've implemented an AudioSource pool for you so you don't have to. That way, you do not need to manage activating and deactivating Unity audio sources.## Contributing
 - **Sound Banks:** Group related sound effects together into Sound Bank assets to create music playlists, random sound cues, and more.

If you feel TrixelAudio is missing a feature, feel free to fork it and submit a pull request! This project is maintained with <3 by [Michael (acidiclight)](https://github.com/acidiclight).

To set up a development environment for TrixelAudio:

1. Create an empty Unity project
2. Create a `Packages` directory in your Assets folder
3. Clone this repository into that directory using Git.
4. Make any changes you'd like, and push them back into your fork!

## Programming Guidelines

To ensure consistency and stability, this project follows the same guidelines as Restitched.

### Singletons
**Singletons are banned.** Singletons are extremely difficult to work with and refactor, and should not be used inside TrixelAudio.

### Nullable Reference Types
This project uses Nullable Reference Types in C#. By default, all reference types are treated as non-nullable and the compiler will warn you of possible null reference errors in your code.

You must explicitly declare any nullable references with the `Type?` syntax. This tells the compiler and other programmers that you intend to accept and handle `null` values properly.

### Serialized Fields
Serialized Fields are members of an object that are serialized by Unity and thus appear in the Inspector. It is good practice never to directly expose a serialized field to any other class in the code, for example:

```cs
// Bad
public class Stuffy : MonoBehaviour
{
    public Craftbook craftbook;
}

// Good
public class Stuffy : MonoBehaviour
{
    
    private Craftbook craftbook;
}
```

If you intend to give other classes access to a serialized field, you may do so with a property.

### Nullable Serialized Fields
By default, all serialized fields are treated as non-nullable reference types. However, the C# compiler has no control or knowledge of how Unity Objects work behind the scenes. Unity is not a C# game engine. Therefore, the below code may produce a nullability warning.

```csharp
public class Stuffy : MonoBehaviour
{
    
    private Craftbook craftbook;
    
    private void Awake()
    {
        this.MustGetComponent(out craftbook);
    }
}
```

You must explicitly initialize the field as `null!` to suppress the warning. It is on you to assert the non-nullability of the field during `Awake()`.

```csharp
private Craftbook craftbook = null!;
```

Alternatively, you may mark the field as nullable. This will require a null-check every time you intend to use this serialized reference.

```csharp
private Craftbook? craftbook;
```

### Documentation
There is nothing wrong with explaining what your code does and why it's there. You can do this using XML documentation comments.

When overriding an inherited or interface member, you must inherit its documentation as well. This can be done like so.

```csharp
public abstract class AbstractThing
{
    /// <summary>
    ///     Restitches the given Stuffy object
    /// </summary>
    public abstract void Restitch(Stuffy stuff);
}

public class OverridingThing : AbstractThing
{
    /// <inheritdoc />
    public override void Restitch(Stuffy stuffy)
    {
        stuffy.IsRestitched = true;
    }
}
```

Your IDE should assist you in writing properly-formatted XML documentation.
