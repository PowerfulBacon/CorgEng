//TODO: Rewrite me to a proper README

https://discord.gg/6hgfjRrVJP

# CorgEng

A modularised, multi-threaded ECS based game engine.

[CorgEng Documentation Contents Page](https://hackmd.io/UnZmbyzhR3GR7silkiitig)

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

### Components

### Entities

### Signals

## Render Cores
