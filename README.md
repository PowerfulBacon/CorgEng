//TODO: Rewrite me to a proper README

https://discord.gg/6hgfjRrVJP

# CorgEng

A modularised, multi-threaded ECS based game engine.

[CorgEng Documentation Contents Page](https://hackmd.io/UnZmbyzhR3GR7silkiitig)

## Example

See CorgEng.Example for examples of how to create CorgEng applications that utilize the important features.

https://github.com/PowerfulBacon/CorgEng/tree/main/CorgEng.Example

## Getting Started

See CorgEng.Example for an example project of how to get started. A more complex example with implementations of ECS will be coming soon.

A quick summary of the important features and how to use them are listed below.

## Dependency Injection

### Fetching Dependencies

```csharp
[UsingDependency]
private static IDependency Dependency;
```

Apply the UsingDependency parameter to have a static dependency instance injected upon load.

**Note:**
The variable must be static.
This does not work for generic typed classes. A wrapper class is required for generic typed classes.

### Creating Dependencies

To create a dependency, you require an interface in some area that is in a common location. In the case of CorgEng engine internals, all of the dependency definitions are defined in CorgEng.GenericInterfaces which is a dependency to all CorgEng modules.

To create your dependency, create a class that implements the interface you want to represent the dependency with and add the `[Dependency]` attribute.
You can define the priority of a dependency to have your dependency override existing dependencies which is useful if you want to rework or refactor a large portion of code.

```csharp
[Dependency]
internal DependencyImplementation : IDependency
{
  //Code here...
}
```

Created dependencies will always be static. If you want an instanced reference, create a factory dependency that can create new instances of the dependency you wish to be instanced.

### Overriding Dependencies (Testing)

//TODO

## Entity Component System / Signals

### Systems

Systems are responsible for processing events and changing the states of components. They can listen for signals raised locally on entities, or listen to signals globally.

```csharp
void InitializeSystem()
{
  RegisterLocalEvent<GComponent, GEvent>(HandleEvent);
}

void HandleEvent(Entity e, GComponent component, GEvent signal)
{
  ...
}
```

### Components

A component simply stores data and nothing else. All processing and changing of the data is handled by the entity system.

### Entities

An entity stores components. It has no functionality without its components.

### Signals/Events

Signals store data and can be raised against an entity.
When a signal is raised, any systems listening for the signal will be notified and can then perform some action.

## Render Cores

Render Cores are the way that CorgEng handles rendering. A render core is a framebuffer that can be bound and drawn to. It has a width and height that can be dynamically modified.
To display a render core, you can render it to any other render core or the main screen.
CorgEng will automatically draw the main render core to the screen every frame.
This allows for the simple application of post-processing effects and culling which will be useful for parts of the game such as UI.
