//TODO: Rewrite me to a proper README

# CorgEng
 A modularised, multi-threaded ECS based game engine.

[CorgEng Documentation Contents Page](https://hackmd.io/UnZmbyzhR3GR7silkiitig)

# CorgEng.Core

## Contents

[toc]

## Summary

The core module of the CorgEng game engine. Where the execution of CorgEng based programs begins.

CorgEng is a module based game engine with a focus on optimisation and performance.

If you want to make a menu, you can create a game for the menu and easilly add in a background by starting another game process with some different logic and rules entirely.

CorgEng uses an Entity Component Based system.

By itself, this module will display a black screen and module loading alerts.

## CorgEng

```csharp=
static void Initialize();
```

Initializes the CorgEng game engine.
Will call initialization on all CorgEng modules.

```csharp=
static void Shutdown();
```

Shuts down the CorgEng game engine and cleans up any resources being used. Terminates used threads so the game can be closed properly.

### CorgEng game flow

```sequence
Program -> CorgEng: Initialize
CorgEng -> CorgEngGame: Load game
CorgEngGame -> RenderCore: Load render core
RenderCore -> CorgEng: Render world
note left of CorgEng: Display render on screen
```

### Modules

CorgEng works with a module based system. This means that each part of CorgEng is heavilly modularised and can be included or not included as depenancies to a project as needed.

### Threads

CorgEng makes good use of multi-threading, however when developing games using the framework it is important to understand how to communicate between threads safely. (Use the Entity Component System).

Rendering Thread:
 - The main thread
 - System threads

The entity component system should handle most of the communication between threads.

## Logging

CorgEng.Core provides a logging system.

```csharp=
using CorgEng.Core.Logging;

...
    
Log.WriteLine(message, logType);
```

Can be used to print a message to the Console. The Log manager can be extended to provide additional functionality, such as logging to files or logging to a place in the game rather than the console if needed.

## Game Management

Game management is a submodule of CorgEng.Core that implements the ability for external programs to interact with the core parts of CorgEng.

Games that run on CorgEng will have a main game class that extends the CorgEngGame abstract class and implement the necessary methods, and then call GameManager.LoadGame() somewhere in their game's initialization.

### CorgEngGame

```csharp=
abstract class CorgEngGame
```

CorgEngGames specifiy the logic that make up a game. This is where most of the stuff that is done on the user end needs to be performed.

```csharp=
void StartGameProcess();
```

Starts the specified game process.
Aadds this game into the rendering queue.

```csharp=
void StopGameProcess();
```

Stops the current game process.

```csharp=
abstract int TickRate { get; }
```

Specifies the tick rate that the game is currently using. When set to 60, there will be a 1/60 second delay between each tick update.

```csharp=
abstract void OnGameTick(double deltaTime);
```

*Calling Thread: GameThread*

 - deltaTime - The time in milliseconds between the completion of the last frame and the beginning of the current one. Allows the game to account for lag if using a realtime system instead of a tick based system.

Called every time the game tick is called. The functionality should be mostly provided by systems, however this is provided so that it can be used for debugging or things that systems cannot handle.
This is called internally by the CorgEng engine and shouldn't be called manually.

```csharp=
abstract RenderCore GameRenderCore { get; }
```

The game's render core. CorgEng will execute this render core to generate an image of the game when rendering is required.
 - How do we handle rendering and the rendering being changed at the same time? (Lock the part of the array that we are writing to, and read the rest to minimize slowdown as much as possible.)

## Rendering

CorgEng rendering works by passing a render texture to the rendering engine. The base renderer provided in the Core module will not be able to do much, since the main bulk of advanced rendering functionality will be provided by CorgEng.Rendering.

### RenderCore

The render core manages render systems, gets everything rendered to a texture and then passes that texture on to be used elsewhere.
Render cores can be used for a variety of purposes, for example embedding a rendered world in a UI component, but primarilly just passes its texture on to the CorgEng core module to be rendered directly to the screen.

Custom render cores can be written to override or extend default render core functionalities, such as to add post-processing effects like bloom, or just apply general post-processing shaders.

Multiple render cores can be used and have their output images stacked on top of each other to create layered rendering, where post-processing effects can be applied to each layer individually.

```csharp=
public virtual int Width { get; }
public virtual int Height { get; }
```

Height and width of the renderable area of the RenderCore. Will be set to the size of the window if not specified.

```csharp=
public void RenderToFramebuffer();
```

Renders the render core to the framebuffer attached to this RenderCore. Can be called by the user, for example if a render core needs to render other render cores (such as a UI core).

```csharp=
protected abstract void PerformRender();
```

Called when rendering is required.
The internal engine cleans the background and sets the render framebuffer target, so this can just execute glDraw calls.

```csharp=
protected abstract void Initialize();
```

Called when the render core is created.

### GenericRenderCore

GenericRenderCore is a render core that renders other rendering cores. This is pretty inefficient, especially when rendering large numbers of items that could be drawn using OpenGL's instanced rendering system.
This render core exists to provide a default loading display without depending on more advanced modules.

 - How do we render the loading game without a proper rendering system?

We could give render cores a list of sub-render cores that get drawn and then drawn to the main render core, which can then be drawn to the screen.
The disadvantage of this is that we are drawing to a middle render core, and then drawing that middle render core to the screen which adds another draw call. (This should be fine).

## Example Code Plan

```csharp=
class Program
{
    
    public static ExampleRenderCore RenderCore { get; private set; }
    
    public static void Main(string[] args)
    {
        //Initialize CorgEng
        //This creates the window and loads all
        //modules that are dependencies
        CorgEng.Initialize();
        //Create our game
        //The game supplies the logic
        ExampleGame exampleGame = new ExampleGame();
        exampleGame.StartGameProcess();
        //Transfer control of the main thread to the CorgEng
        //rendering thread
        CorgEng.TransferToRenderingThread();
        //Shut down the program once it has been closed
        //and clean everything up.
        CorgEng.Shutdown();
    }
}
```

```csharp=
//Custom render cores can be used for global rendering effects
//such as post-processing
class ExampleRenderCore : CorgEngRenderCore
{

    //Renders stuff to a render texture,
    //which then gets returned to CorgEng
    //for displaying on the screen.
    //For example purposes this rendering engine
    //assumes the super type to be an instanced rendering core
    //that has already been implemented.
    public override void PerformRender()
    {
        return base.PerformRender();
    }
}
```

```csharp=
class ExampleGame : CorgEngGame
{

    //Create and set the render core for this game in object construction
    public RenderCore GameRenderCore { get; } = new ExampleRenderCore();

    protected override void OnGameInitialization()
    {
        //Create an example entity
        Entity entity = new Entity();
        //Create the renderable component
        //Renderable component is an implementation of IRenderable
        //supplied by the component renderable module.
        ExampleRenderableComponent renderableComponent = new ExampleRenderableComponent();
        //Add the component to the entity, so that the renderable
        //can be updated when the entity is updated
        entity.AddComponent(renderableComponent);
        //Start rendering the renderable component
        GameRenderCore.StartRendering<ExampleRenderSystem>(renderableComponent);
    }
}
```
