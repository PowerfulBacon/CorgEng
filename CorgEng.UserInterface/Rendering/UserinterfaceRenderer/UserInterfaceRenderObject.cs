﻿using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer
{
    public sealed class UserInterfaceRenderObject : IUserInterfaceRenderObject
    {

        public IBindableProperty<uint> TextureFile { get; set; }

        public IBindableProperty<float> TextureFileX { get; set; }

        public IBindableProperty<float> TextureFileY { get; set; }

        public IBindableProperty<float> TextureFileWidth { get; set; }

        public IBindableProperty<float> TextureFileHeight { get; set; }

        public IBindableProperty<IVector<float>> WorldPosition { get; } = new BindableProperty<IVector<float>>(new Vector<float>(0, 0, 0));

        public IBindablePropertyGroup TextureDetails { get; }

        public IEntityDef TypeDef { get; set; }

        public UserInterfaceRenderObject(uint textureUint, float textureX, float textureY, float textureWidth, float textureHeight)
        {
            //When the vector changes, trigger change on the bindable property.
            WorldPosition.Value.OnChange += (object src, EventArgs arg) => { WorldPosition.TriggerChanged(); };
            //Set the bindable properties
            TextureFile = new BindableProperty<uint>(textureUint);
            TextureFileX = new BindableProperty<float>(textureX);
            TextureFileY = new BindableProperty<float>(textureY);
            TextureFileWidth = new BindableProperty<float>(textureWidth);
            TextureFileHeight = new BindableProperty<float>(textureHeight);
            TextureDetails = new BindablePropertyGroup(TextureFileX, TextureFileY, TextureFileWidth, TextureFileHeight);
        }

        private object batchContained;

        public IBatchElement<BatchType> GetBelongingBatchElement<BatchType>() where BatchType : IBatch<BatchType>
        {
            return (IBatchElement<BatchType>)batchContained;
        }

        public void SetBelongingBatchElement<BatchType>(IBatchElement<BatchType> heldBatch) where BatchType : IBatch<BatchType>
        {
            batchContained = heldBatch;
        }

        public ISharedRenderAttributes GetSharedRenderAttributes()
        {
            return new UserInterfaceSharedRenderAttributes(TextureFile.Value);
        }

        public void PreInitialize(IVector<float> initializePosition)
        { }

        public void Initialize(IVector<float> initializePosition)
        { }

        public bool SetProperty(string name, IPropertyDef property)
        {
            //TODO: Textures can be loaded from the def file
            return false;
        }

    }
}
