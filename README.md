//TODO: Rewrite me to a proper README

https://discord.gg/6hgfjRrVJP

# CorgEng

A modularised, ECS-based game engine with high performance, scalability and collaberation built in mind.

[CorgEng Documentation Contents Page](https://hackmd.io/UnZmbyzhR3GR7silkiitig)

CorgEng relies heavilly on multi-threading and a good understanding of concurrency is recommened in order to prevent issues arising from concurrent modification.
The entity component system does a good job at forcing code to work asynchronously, however event handling is performed out of order, so if an event depends on the result of another event, the first event should be executed synchronously. See the ECS section for more details.

## Building

In order to build the project, you may need to nuget restore.
The nuget packages can be obtained from https://api.nuget.org/v3/index.json.
Visual studio should do this automatically, however when building on my laptop I had to add the above link as a package source in nuget before calling dotnet restore.

## Example

See CorgEng.Example for examples of how to create CorgEng applications that utilize the important features.

https://github.com/PowerfulBacon/CorgEng/tree/main/CorgEng.Example

## Getting Started

See CorgEng.Example for an example project of how to get started. A more complex example with implementations of ECS will be coming soon.

A quick summary of the important features and how to use them are listed below.

## General Concepts / Notes

Events in CorgEng are all asynchronous and cannot return the results of that event call. Events cannot return, as this would require them to be synchronous which would result in subsystems depending on other subsystems creating potential bottlenecks when scaling games up to a large number of entities/users.
A solution to this issue is by using event chains. The result of one event call could be to call another event given some conditions on that system. If the conditions to the event are not met, then the event chain is broken and the actions from the event will not occur.
For example:
TranslateEvent (X, Y) -> TransformSystem (Gets current position and adds on (X, Y)) -> MoveEvent (newPosition) -> _Other Listeners_
The top is used in transforms, since the translate event has no information about the entities current position, so need to have it injected by the transform system first.

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

## Content Loading

CorgEng has a built in module for loading content dynamically.

### Creating Entities

IEntityCreator provides a method `CreateEntity` that can produce entities defined by dynamic content files. This takes in 2 parameters, the name of the object to be instantiated, and a nullable function that executes events on an entity before initialisation is called. All events in this function should be called synchronously.

```c#
EntityCreator.CreateEntity(objectToSpawn, (createdEntity) => {
    IVector<float> vector = CorgEngMain.MainCamera.ScreenToWorldCoordinates(0, 0, 1000, 1000);
    new SetPositionEvent(new Vector<float>(vector.X, vector.Y)).Raise(createdEntity, true);
    new SetSpriteRendererEvent(ExampleRenderCore.GetEntityRenderer(createdEntity)).Raise(createdEntity, true);
});
```

## Render Cores

Render Cores are the way that CorgEng handles rendering. A render core is a framebuffer that can be bound and drawn to. It has a width and height that can be dynamically modified.
To display a render core, you can render it to any other render core or the main screen.
CorgEng will automatically draw the main render core to the screen every frame.
This allows for the simple application of post-processing effects and culling which will be useful for parts of the game such as UI.
