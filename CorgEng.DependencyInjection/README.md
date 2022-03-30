# CorgEng.DependencyInjection

## Contents

[toc]

## Summary

Dependency injection provides a way of specifying that something wants something that performs a task without specifying the implementation of that task.
It allows us to override and easilly change the functionality of existing systems without needing to rip them out and rewrite them entirely.
It also allows us to inject custom dependency overrides for unit tests, which allow us to force certain behaviours for test cases.
Dependencies are created as singleton instances, so changes a property on 1 will affect the properties of all instances.
Since the injected dependencies are static variables, to create instanced dependencies, a factory class is required. The factory class can be injected into the static dependency and then used to created instanced dependencies.

## Defining dependancies

```csharp=
interface ILogger
{
    void WriteLine(string message, LogType logType);
}
```

```csharp=
[Dependancy(defaultDependency=true)]
class Logger : ILogger
{
    ...
}
```

## Using dependancies

```csharp=
[UsingDependency]
private static ILogger logger;

...
    
logger.WriteLine("test", LogType.Standard);
```

The using dependency tag will automatically fetch the required dependency. The dependency variable must be static to accept the singleton during load time.
