using NUnit.Framework;
using PropHunt.Mixed.Components;
using PropHunt.Server.Systems;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Server
{
    /// <summary>
    /// Tests to verify functionality of the ConnectionSystem
    /// </summary>
    [TestFixture]
    public class HandleDisconnectSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Connection system tests
        /// </summary>
        private HandleDisconnectSystem handleDisconnectSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.handleDisconnectSystem = base.World.CreateSystem<HandleDisconnectSystem>();
        }

        /// <summary>
        /// Craete a disconnect for a given Id
        /// </summary>
        /// <param name="id">Id of player to disconnect</param>
        /// <returns>Entity that contains the disconnect event</returns>
        public Entity CreateDisconnectEvent(int id)
        {
            Entity disconnectEvent = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<NetworkStreamDisconnected>(disconnectEvent);
            base.m_Manager.AddComponentData<NetworkIdComponent>(disconnectEvent, new NetworkIdComponent { Value = id });
            return disconnectEvent;
        }


        /// <summary>
        /// Craete a player with a given Id
        /// </summary>
        /// <param name="id">Id to assign to player</param>
        /// <returns>Entity that contains the player</returns>
        public Entity CreatePlayer(int id)
        {
            Entity player = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<PlayerId>(player, new PlayerId { playerId = id });
            return player;
        }

        /// <summary>
        /// Verify behaviour of handle disconnect system when there is one playerID object and 
        /// no disconnect event
        /// </summary>
        [Test]
        public void OnePlayerNoDisconnect()
        {
            // Create a player with id = 0
            Entity player0 = this.CreatePlayer(0);

            // Update the handle disconnect system
            this.handleDisconnectSystem.Update();

            // Verify player still exists
            Assert.IsTrue(base.m_Manager.Exists(player0));
        }

        /// <summary>
        /// Verify behaviour of handle disconnect system when there is one playerID object and 
        /// one disconnect event
        /// </summary>
        [Test]
        public void OnePlayerOneDisconnect()
        {
            // Create a disconnect event with id = 0
            Entity disconnect = this.CreateDisconnectEvent(0);
            // Create a player with id = 0
            Entity player0 = this.CreatePlayer(0);

            // Update the handle disconnect system
            this.handleDisconnectSystem.Update();

            // Verify player still exists
            Assert.IsFalse(base.m_Manager.Exists(player0));

            // Verify player does not exist when the ids are not the same
            // Create a disconnect event with id = 0
            base.m_Manager.DestroyEntity(disconnect);
            this.CreateDisconnectEvent(0);
            // Create a player with id = 0
            Entity player1 = this.CreatePlayer(1);

            // Update the handle disconnect system
            this.handleDisconnectSystem.Update();

            // Verify player still exists
            Assert.IsTrue(base.m_Manager.Exists(player1));
        }

        /// <summary>
        /// Verify behaviour of handle disconnect system when there is many playerID objects and 
        /// one disconnect event
        /// </summary>
        [Test]
        public void ManyPlayerOneDisconnect()
        {
            // Create a disconnect event with id = 0
            this.CreateDisconnectEvent(0);
            // Create a player with id = 0, 1, 2
            Entity player0 = this.CreatePlayer(0);
            Entity player0_2 = this.CreatePlayer(0);
            Entity player1 = this.CreatePlayer(1);
            Entity player2 = this.CreatePlayer(2);

            // Update the handle disconnect system
            this.handleDisconnectSystem.Update();

            // Verify player 1 and 2 exist while player 0 and 0_2 do not
            Assert.IsFalse(base.m_Manager.Exists(player0));
            Assert.IsFalse(base.m_Manager.Exists(player0_2));
            Assert.IsTrue(base.m_Manager.Exists(player1));
            Assert.IsTrue(base.m_Manager.Exists(player2));

        }

        /// <summary>
        /// Verify behaviour of handle disconnect system when there is many playerID objects and 
        /// many disconnect events
        /// </summary>
        [Test]
        public void ManyPlayerManyDisconnect()
        {
            // Create a disconnect event with id = 1
            this.CreateDisconnectEvent(1);
            this.CreateDisconnectEvent(2);
            // Create a player with id = 0, 1, 2
            Entity player0 = this.CreatePlayer(0);
            Entity player0_2 = this.CreatePlayer(0);
            Entity player1 = this.CreatePlayer(1);
            Entity player2 = this.CreatePlayer(2);

            // Update the handle disconnect system
            this.handleDisconnectSystem.Update();

            // Verify player 1 and 2 exist while player 0 and 0_2 do not
            Assert.IsTrue(base.m_Manager.Exists(player0));
            Assert.IsTrue(base.m_Manager.Exists(player0_2));
            Assert.IsFalse(base.m_Manager.Exists(player1));
            Assert.IsFalse(base.m_Manager.Exists(player2));
        }
    }
}