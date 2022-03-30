# CorgEng.Rendering

## Contents

[toc]

## Dependencies

[CorgEng.EntityComponentSystem](https://hackmd.io/UBpdUuDuRXaOBRYwGO40Jw)
[CorgEng.RenderSystem](https://hackmd.io/aZsJGr7gSM2d6f01AmRcKA)

## Summary

Provides a component based implementation of the rendering system, allowing entities to have a renderable component applied to them, allowing them to be rendered.

CorgEng.Core provides an interface `ITextRenderer` with some methods that define the intention of an implementation. The default implementation of this does nothing, however by using dependency injection, when the rendering module is included with a project, a propert text renderer can be injected.

## Documentation

### IModel

```csharp=
public interface IModel
```

IModel defines something with vertices and UVs that can be sent to the renderer for rendering.

```csharp=
int VertexCount { get; }
```

The number of vertices that the model has. A square made up of 3 triangles has 6 vertices (3 for each triangle).

```csharp=
uint VertexBuffer { get; }
```

The ID of the buffer that represents the vertex buffer for this model.

```csharp=
uint UvBuffer { get; }
```

The ID of the buffer that represents the UV buffer for this model.

### IModelFactory

```csharp=
public interface IModelFactory
```

IModelFactory is a factory class that can be used to create Models. When creating models based off of a dependency type, a factory dependency can be created and used to produce models without knowing the specific implementation of IModel.

```csharp=
IModel CreateModel(float[] vertices, float[] uvs);
```

Creates a model with the specified vertices and UVs.

### IRenderer

```csharp=
public interface IRenderer
```

IRenderer defines something that can render objects. It is used for generic abstract classes, however.
