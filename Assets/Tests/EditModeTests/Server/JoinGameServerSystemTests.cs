using System;
using NUnit.Framework;
using PropHunt.Mixed.Commands;
using PropHunt.Mixed.Components;
using PropHunt.Server.Systems;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.NetCode;
using Unity.Transforms;

namespace PropHunt.EditMode.Tests.Server
{
    /// <summary>
    /// Tests to verify functionality of the ConnectionSystem
    /// </summary>
    [TestFixture]
    public class JoinGameServerSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// JoinGameServerSystem for testing the system
        /// </summary>
        private JoinGameServerSystem joinGameServerSystem;

        /// <summary>
        /// Ghost collection entity
        /// </summary>
        private Entity ghostCollection;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.joinGameServerSystem = base.World.CreateSystem<JoinGameServerSystem>();

            // Setup the character avatar and ghost collection
            this.ghostCollection = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<GhostPrefabCollectionComponent>(ghostCollection);
            base.m_Manager.AddBuffer<GhostPrefabBuffer>(ghostCollection);
            // Setup player entity
            Entity playerEntity = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<PlayerId>(playerEntity);
            // Add a player entity and an empty entity to the prefab buffer
            Entity emptyPrefab = base.m_Manager.CreateEntity();
            DynamicBuffer<GhostPrefabBuffer> prefabBuffer = base.m_Manager.GetBuffer<GhostPrefabBuffer>(ghostCollection);
            prefabBuffer.Add(new GhostPrefabBuffer { Value = emptyPrefab });
            prefabBuffer.Add(new GhostPrefabBuffer { Value = playerEntity });
        }

        /// <summary>
        /// Verify that a player object is created when a player joins the game through request
        /// </summary>
        [Test]
        public void TestPlayerJoin()
        {
            // Player Id
            int playerId = 42;
            // Setup source connection
            Entity sourceConnection = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<NetworkIdComponent>(sourceConnection, new NetworkIdComponent { Value = playerId });
            // Setup request command
            Entity requestEntity = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<JoinGameRequest>(requestEntity);
            base.m_Manager.AddComponentData<ReceiveRpcCommandRequestComponent>(requestEntity,
                new ReceiveRpcCommandRequestComponent { SourceConnection = sourceConnection });
            
            // Process the request
            this.joinGameServerSystem.Update();

            // Verify actions
            // Verify that the source connection now has a NetworkStreamInGame component
            Assert.IsTrue(base.m_Manager.HasComponent<NetworkStreamInGame>(sourceConnection));
            Assert.IsTrue(base.m_Manager.HasComponent<CommandTargetComponent>(sourceConnection));

            // Assert that a player now exists in the world
            var playerQuery = base.m_Manager.CreateEntityQuery(typeof(PlayerId), typeof(Translation), typeof(GhostOwnerComponent));
            Assert.IsTrue(playerQuery.CalculateEntityCount() == 1);
            var players = playerQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            Entity player = players[0];
            Assert.IsTrue(base.m_Manager.GetComponentData<PlayerId>(player).playerId == playerId);
            // Cleanup memory
            players.Dispose();
        }

        private void UpdateJoinSystem()
        {
            this.joinGameServerSystem.Update();
        }

        [Test]
        public void TestPlayerJoinNoCollection()
        {
            // Clear the prefab buffer
            base.m_Manager.GetBuffer<GhostPrefabBuffer>(this.ghostCollection).Clear();
            // Player Id
            int playerId = 0;
            // Setup source connection
            Entity sourceConnection = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponentData<NetworkIdComponent>(sourceConnection, new NetworkIdComponent { Value = playerId });
            // Setup request command
            Entity requestEntity = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<JoinGameRequest>(requestEntity);
            base.m_Manager.AddComponentData<ReceiveRpcCommandRequestComponent>(requestEntity,
                new ReceiveRpcCommandRequestComponent { SourceConnection = sourceConnection });
            
            // Process the request and assert that an exception is thrown
            Assert.Throws<IndexOutOfRangeException>(new TestDelegate(UpdateJoinSystem));
        }
    }
}