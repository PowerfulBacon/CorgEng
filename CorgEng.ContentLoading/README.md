
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

---

### Object

Creates a new object from a specified type. Can be used to create entities if the name attribute is used instead of the type.

An object node can also contain direct values, such as intengers.

#### Attributes:

 - name: If set, will locate and spawn the entity with this name.
 - createdType (Required): If set, will create an object of the specified type.
 - returnType (Optional):

#### Content:

 - Can contain 'Property' nodes.
 - Can contain direct values

---

### Instance

Fetches the created instance based on a key.

---

### Array

Defines a collection of elements stored in an Array object.
Requires a type to be specified - The type is not automatically inferred.

#### Content:

 - Can contain Element nodes

#### Example

An array of value types:

```xml
<Array type="Int32">
    <Element>4</Element>
    <Element>8</Element>
    <Element>30</Element>
    ...
</Array>
```

An array of complex types:

```xml
<Array type="ComplexType">
    <Element>
        <Object type="ComplexType">
            ...
        </Object>
    </Element>
    ...
</Array>
```

---

### Element

Defines an element of a collection type.
Can either be a value type, or a complex type.

---

### Dictionary

Creates a dictionary, which associates keys and values.
Note that the type of the dictionary can differ to the types created by the 
Key and Value pairs, as long as the type created by the key/value nodes
are a child of the corresponding dictionary type.

A dictionary with 'Object' keys can have Int32s inserted as keys.

#### Attributes

 - keyType (Required): The type of the dictionary key.
 - valueType (Required): The type of the dictionary value.

#### Content

 - Must contain element nodes, which contain key and value nodes.

#### Example

```xml
<Dictionary id="test_basic_dictionary" keyType="Int32" valueType="Int32">
    <Element>
        <Key createdType="Int32">1</Key>
        <Value createdType="Int32">4</Value>
    </Element>
    <Element>
        <Key createdType="Int32">2</Key>
        <Value createdType="Int32">5</Value>
    </Element>
</Dictionary>
```

---

### Key

Defines the key of a KeyValuePair represented by an Element.

See object node for more details.

---

### Value

Defines the value of a KeyValuePair represented by an Element.

See object node for more details.

---

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
