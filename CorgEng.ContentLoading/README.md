
## ContentLoading V2 Documentation

## Summary

Version 2 of the content loading system for CorgEng will incorporate better XML design and standardise some of the things we were doing weirdly before.

## Nodes

### All Nodes

#### Properties

 - key: May assign the instance a key identifier, which can then be used by children.
If an entity is given a key, then the created entity instance can be used as a parameter.

### Entities

The parent node of the entity file.

### Entity

Specifies the definition of a new entity.

#### Attributes:

 - name: Specifies the unique name to use for this entity.
 - abstract: A boolean value representing if this entity can be spawned in the world or not.
 - parent: The name of the parent entity. Values from the parent will be inhereted.

#### Content:

 - Can contain 'Component' nodes.

---

### Component

Adds a component to an entity.

#### Attributes:

 - type: The type of the component to add.

#### Content:

 - Can contain 'Property' nodes. This will update the properties of the component.

---

### Property

Updates the property of the parent type.

#### Attributes:

 - name: The name of the property to set.
 - value (optional): If this is set then it will apply the value specified in the attribute to the property.

#### Content:

 - Can contain 'Property' nodes. This will affect the assigned value of this property, which will usually be the default value, unless already specified.
 - Can contain 'Object' nodes. This will create a new object and replace the default value of the property.

### Object

Creates a new object from a specified type. Can be used to create entities if the name attribute is used instead of the type.

#### Attributes:

 - name: If set, will locate and spawn the entity with this name.
 - type: If set, will create an object of the specified type.

#### Content:

 - Can contain 'Property' nodes.

### Instance

Fetches the created instance based on a key.

## Sample File

```xml=
<?xml version="1.0" encoding="utf-8" ?> 
<Entities>

  <Entity name="Pawn">

    <Component type="TransformComponent" />
    
    <Component type="SpriteRenderComponent">
      <Property name="Sprite">
        <Dependency type="IIconFactory" method="CreateIcon">
          <Parameter>human_body</Parameter>
        </Dependency>
      </Property>
    </Component>
    
    <Component type="MobHealthComponent">
      <Property name="MaxHealth">100</Property>
      <Property name="Health">100</Property>
    </Component>

    <Component type="PawnControlComponent" />
    
  </Entity>
  
</Entities>
```
