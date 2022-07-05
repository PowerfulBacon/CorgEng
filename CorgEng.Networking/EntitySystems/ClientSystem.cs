using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.World;
using CorgEng.Networking.Components;
using CorgEng.Networking.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.EntitySystems
{
    /// <summary>
    /// Responsible for sending information about entities in view
    /// when a client moves.
    /// </summary>
    internal class ClientSystem : EntitySystem
    {

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static IEntityCommunicator EntityCommunicator;

        [UsingDependency]
        private static IWorld WorldAccess;

        public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup()
        {
            RegisterLocalEvent<ClientComponent, ClientConnectedEvent>(OnClientConnected);
            RegisterLocalEvent<ClientComponent, MoveEvent>(OnClientMoved);
            RegisterLocalEvent<ClientComponent, AttachClientEvent>(OnAttachClient);
            RegisterLocalEvent<ClientComponent, DeleteEntityEvent>(OnEntityDeleted);
        }

        /// <summary>
        /// When the entity is deleted, detach the client from it to ensure proper deletion
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="clientComponent"></param>
        /// <param name="entityDeletedEvent"></param>
        private void OnEntityDeleted(IEntity entity, ClientComponent clientComponent, DeleteEntityEvent entityDeletedEvent)
        {
            if (clientComponent.AttachedClient != null)
            {
                clientComponent.AttachedClient.AttachedEntity = null;
                clientComponent.AttachedClient = null;
            }
        }

        private void OnAttachClient(IEntity entity, ClientComponent clientComponent, AttachClientEvent attachClientEvent)
        {
            //Attach the client
            clientComponent.AttachedClient = attachClientEvent.Client;
            clientComponent.AttachedClient.AttachedEntity = entity;
        }

        /// <summary>
        /// Called when a client connects to the server.
        /// Transmits information about objects near them.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="clientComponent"></param>
        /// <param name=""></param>
        /// <param name="clientConnectedEvent"></param>
        private void OnClientConnected(IEntity entity, ClientComponent clientComponent, ClientConnectedEvent clientConnectedEvent)
        {
            //Transmit the information about the entities nearby

            //Calculate the new bounds
            int left = (int)Math.Floor(clientComponent.AttachedClient.View.ViewOffsetX - clientComponent.AttachedClient.View.ViewWidth);
            int right = (int)Math.Floor(clientComponent.AttachedClient.View.ViewOffsetX + clientComponent.AttachedClient.View.ViewWidth);
            int top = (int)Math.Floor(clientComponent.AttachedClient.View.ViewOffsetY + clientComponent.AttachedClient.View.ViewHeight);
            int bottom = (int)Math.Floor(clientComponent.AttachedClient.View.ViewOffsetY - clientComponent.AttachedClient.View.ViewHeight);

            for (int x = left; x <= right; x++)
            {
                for (int y = bottom; y <= top; y++)
                {
                    //Get information about the world tile we want to transmit
                    //TODO: Z-Levels
                    IContentsHolder contentsHolder = WorldAccess.GetContentsAt(x, y, 0);
                    if (contentsHolder == null)
                        continue;
                    //Get a list of all entities that need to be sent
                    //Painfully expensive
                    foreach (IEntity entityToTransmit in contentsHolder.GetContents())
                    {
                        EntityCommunicator.CommunicateEntity(entityToTransmit, clientComponent.AttachedClient);
                    }
                }
            }

            Logger?.WriteLine("A client connected and was sent information about their view.", LogType.DEBUG);
        }

        /// <summary>
        /// Called when the client entity is moved.
        /// Will transmit information about new items in view to the client.
        /// Anything thats no longer in view will just be left idle in memory, and
        /// will be recompared against prototypes when it comes back into view.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="clientComponent"></param>
        /// <param name="moveEvent"></param>
        private void OnClientMoved(IEntity entity, ClientComponent clientComponent, MoveEvent moveEvent)
        {
            //Calculate what should be added to view.
            //Send this information across to the client.

            //Calculate the old bounds
            int oldLeft = (int)Math.Floor(moveEvent.OldPosition.X + clientComponent.AttachedClient.View.ViewOffsetX - clientComponent.AttachedClient.View.ViewWidth);
            int oldRight = (int)Math.Floor(moveEvent.OldPosition.X + clientComponent.AttachedClient.View.ViewOffsetX + clientComponent.AttachedClient.View.ViewWidth);
            int oldTop = (int)Math.Floor(moveEvent.OldPosition.Y + clientComponent.AttachedClient.View.ViewOffsetY - clientComponent.AttachedClient.View.ViewHeight);
            int oldBottom = (int)Math.Floor(moveEvent.OldPosition.Y + clientComponent.AttachedClient.View.ViewOffsetY + clientComponent.AttachedClient.View.ViewHeight);
            //Calculate the new bounds
            int newLeft = (int)Math.Floor(moveEvent.NewPosition.X + clientComponent.AttachedClient.View.ViewOffsetX - clientComponent.AttachedClient.View.ViewWidth);
            int newRight = (int)Math.Floor(moveEvent.NewPosition.X + clientComponent.AttachedClient.View.ViewOffsetX + clientComponent.AttachedClient.View.ViewWidth);
            int newTop = (int)Math.Floor(moveEvent.NewPosition.Y + clientComponent.AttachedClient.View.ViewOffsetY - clientComponent.AttachedClient.View.ViewHeight);
            int newBottom = (int)Math.Floor(moveEvent.NewPosition.Y + clientComponent.AttachedClient.View.ViewOffsetY + clientComponent.AttachedClient.View.ViewHeight);

            //Locate all tiles within the delta
            int lowerX;
            int upperX;
            if (newLeft < oldLeft)
            {
                lowerX = newLeft;
                upperX = oldLeft;
            }
            else
            {
                lowerX = oldRight;
                upperX = newRight;
            }

            //Process the vertical delta
            for (int x = lowerX; x <= upperX; x++)
            {
                for (int y = newBottom; y <= newTop; y++)
                {
                    //Get information about the world tile we want to transmit
                    //TODO: Z-Levels
                    IContentsHolder contentsHolder = WorldAccess.GetContentsAt(x, y, 0);
                    //Get a list of all entities that need to be sent
                    //Painfully expensive
                    foreach (IEntity entityToTransmit in contentsHolder.GetContents())
                    {
                        EntityCommunicator.CommunicateEntity(entityToTransmit, clientComponent.AttachedClient);
                    }
                }
            }

            //Horizontal stuff too
            int lowerY;
            int upperY;
            if (newBottom < oldBottom)
            {
                lowerY = newBottom;
                upperY = oldBottom;
            }
            else
            {
                lowerY = oldTop;
                upperY = newTop;
            }

            //Process the vertical delta
            for (int x = newLeft; x <= newRight; x++)
            {
                //TODO: Refactor me
                if (x >= lowerX && x <= upperX)
                    continue;
                for (int y = lowerY; y <= upperY; y++)
                {
                    //Get information about the world tile we want to transmit
                    //TODO: Z-Levels
                    IContentsHolder contentsHolder = WorldAccess.GetContentsAt(x, y, 0);
                    //Get a list of all entities that need to be sent
                    //Painfully expensive
                    foreach (IEntity entityToTransmit in contentsHolder.GetContents())
                    {
                        EntityCommunicator.CommunicateEntity(entityToTransmit, clientComponent.AttachedClient);
                    }
                }
            }

        }

    }
}
