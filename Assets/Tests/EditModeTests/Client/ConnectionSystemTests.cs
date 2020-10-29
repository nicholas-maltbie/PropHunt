using System;
using NUnit.Framework;
using PropHunt.Client.Components;
using PropHunt.Client.Systems;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.NetCode;
using static PropHunt.Game.ClientGameSystem;

namespace PropHunt.EditMode.Tests.Client
{
    /// <summary>
    /// Tests to verify functionality of the ConnectionSystem
    /// </summary>
    [TestFixture]
    public class ConnectionSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Counter for connection events
        /// </summary>
        private int connectionCounts = 0;

        /// <summary>
        /// Counter for disconnection events
        /// </summary>
        private int disconnectionCounts = 0;

        /// <summary>
        /// Connection system tests
        /// </summary>
        private ConnectionSystem connectionSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.connectionSystem = base.World.CreateSystem<ConnectionSystem>();
            // Connection Componenet created as part of Connection System
            // base.m_Manager.CreateEntity(typeof(ConnectionComponent));

            this.connectionCounts = 0;
            this.disconnectionCounts = 0;
        }

        /// <summary>
        /// Verify no changes occur when a test is not requested
        /// </summary>
        [Test]
        public void TestNoRequest()
        {
            var connectionBefore = this.connectionSystem.GetSingleton<ConnectionComponent>();

            this.connectionSystem.Update();

            var connectionAfter = this.connectionSystem.GetSingleton<ConnectionComponent>();

            Assert.IsTrue(connectionBefore.Equals(connectionAfter));
        }

        /// <summary>
        /// Verify a connection/disconnection is requested when requesting connect/disconnect
        /// </summary>
        [Test]
        public void TestRequestConnectDisconnect()
        {
            this.connectionSystem.Update();

            var connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            Assert.IsFalse(connectionData.requestConnect);
            Assert.IsFalse(connectionData.requestDisconnect);

            ConnectionSystem.ConnectToServer();
            this.connectionSystem.Update();

            connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            Assert.IsTrue(connectionData.requestConnect);
            Assert.IsFalse(connectionData.requestDisconnect);

            ConnectionSystem.DisconnectFromServer();
            this.connectionSystem.Update();

            connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            Assert.IsTrue(connectionData.requestConnect);
            Assert.IsTrue(connectionData.requestDisconnect);
        }

        /// <summary>
        /// Counter for counting connect events
        /// </summary>
        /// <param name="source"></param>
        /// <param name="connect"></param>
        private void ConnectCounter(object source, ListenConnect connect)
        {
            this.connectionCounts += 1;
        }

        /// <summary>
        /// Counter for counting disconnect events
        /// </summary>
        /// <param name="source"></param>
        /// <param name="disconnect"></param>
        private void DisconnectCounter(object source, ListenDisconnect disconnect)
        {
            this.disconnectionCounts += 1;
        }

        /// <summary>
        /// Verify a connection/disconnection completion dependent on network stream connection
        /// without attempting to connect or disconnect. 
        /// </summary>
        [Test]
        public void TestVerifyNoAttemptConnectDisconnect()
        {
            var networkStreamEntity = base.m_Manager.CreateEntity(typeof(NetworkStreamConnection));

            // Also verify events
            ConnectionSystem.OnConnect += ConnectCounter;
            // Also verify events
            ConnectionSystem.OnDisconnect += DisconnectCounter;
            // Stream connection component
            Entity streamConn = base.m_Manager.CreateEntity(typeof(NetworkStreamConnection));

            // Test connection while attempting to connect
            this.connectionSystem.SetSingleton<ConnectionComponent>(new ConnectionComponent
            {
                attemptingConnect = false,
                attemptingDisconnect = false
            });
            // Verify connect flow for detecting connect
            this.connectionSystem.Update();
            var connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            // start off disconnected
            Assert.IsFalse(connectionData.isConnected);
            // Simulate connection to server
            base.m_Manager.AddComponent<NetworkStreamInGame>(streamConn);
            this.connectionSystem.Update();
            connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            // detect connection
            Assert.IsFalse(connectionData.isConnected);
            Assert.IsFalse(connectionData.attemptingConnect);
            // Assert connection event
            Assert.IsTrue(this.connectionCounts == 0);
            Assert.IsTrue(this.disconnectionCounts == 0);

            // Simulate disconnecting to server
            this.connectionSystem.SetSingleton<ConnectionComponent>(new ConnectionComponent
            {
                attemptingConnect = false,
                attemptingDisconnect = false,
                isConnected = true,
            });
            base.m_Manager.AddComponent<NetworkStreamDisconnected>(streamConn);
            base.m_Manager.RemoveComponent<NetworkStreamInGame>(streamConn);
            this.connectionSystem.Update();
            connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            // detect connection
            Assert.IsTrue(connectionData.isConnected);
            Assert.IsFalse(connectionData.attemptingDisconnect);
            // Assert connection event
            Assert.IsTrue(this.connectionCounts == 0);
            Assert.IsTrue(this.disconnectionCounts == 0);
        }

        /// <summary>
        /// Verify a connection/disconnection completion dependent on network stream connection
        /// </summary>
        [Test]
        public void TestVerifyFinishConnectDisconnect()
        {
            var networkStreamEntity = base.m_Manager.CreateEntity(typeof(NetworkStreamConnection));

            // Also verify events
            ConnectionSystem.OnConnect += ConnectCounter;
            // Also verify events
            ConnectionSystem.OnDisconnect += DisconnectCounter;
            // Stream connection component
            Entity streamConn = base.m_Manager.CreateEntity(typeof(NetworkStreamConnection));

            // Test connection while attempting to connect
            this.connectionSystem.SetSingleton<ConnectionComponent>(new ConnectionComponent
            {
                attemptingConnect = true,
                attemptingDisconnect = false
            });
            // Verify connect flow for detecting connect
            this.connectionSystem.Update();
            var connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            // start off disconnected
            Assert.IsFalse(connectionData.isConnected);
            // Simulate connection to server
            base.m_Manager.AddComponent<NetworkStreamInGame>(streamConn);
            this.connectionSystem.Update();
            connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            // detect connection
            Assert.IsTrue(connectionData.isConnected);
            Assert.IsFalse(connectionData.attemptingConnect);
            // Assert connection event
            Assert.IsTrue(this.connectionCounts == 1);
            Assert.IsTrue(this.disconnectionCounts == 0);

            // Simulate disconnecting to server
            this.connectionSystem.SetSingleton<ConnectionComponent>(new ConnectionComponent
            {
                attemptingConnect = false,
                attemptingDisconnect = true
            });
            base.m_Manager.AddComponent<NetworkStreamDisconnected>(streamConn);
            base.m_Manager.RemoveComponent<NetworkStreamInGame>(streamConn);
            this.connectionSystem.Update();
            connectionData = this.connectionSystem.GetSingleton<ConnectionComponent>();
            // detect connection
            Assert.IsFalse(connectionData.isConnected);
            Assert.IsFalse(connectionData.attemptingDisconnect);
            // Assert connection event
            Assert.IsTrue(this.connectionCounts == 1);
            Assert.IsTrue(this.disconnectionCounts == 1);
        }
    }

    /// <summary>
    /// Tests to verify functionality of clear ghost entities system
    /// </summary>
    [TestFixture]
    public class ClearClientGhostEntitiesTests : ECSTestsFixture
    {
        /// <summary>
        /// Clear client ghosts system
        /// </summary>
        ClearClientGhostEntities ghostEntitySystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.ghostEntitySystem = base.World.CreateSystem<ClearClientGhostEntities>();
            base.m_Manager.CreateEntity(typeof(ConnectionComponent));
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        /// <summary>
        /// Simple update test without a singleton for clearing ghosts
        /// </summary>
        [Test]
        public void UpdateWithoutEntity()
        {
            this.ghostEntitySystem.Update();
        }

        /// <summary>
        /// Simple update test where a ghost does not exist
        /// </summary>
        [Test]
        public void DestroyNoEntities()
        {
            base.m_Manager.CreateEntity(ComponentType.ReadOnly(typeof(ClearClientGhostEntities.ClientClearGhosts)));
            // Update with no entities in world
            this.ghostEntitySystem.Update();

            EndSimulationEntityCommandBufferSystem buffer = base.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            buffer.Update();
        }

        /// <summary>
        /// Simple update test with multiple entities (ghost and non ghost).
        /// </summary>
        [Test]
        public void DestroyManyEntities()
        {
            base.m_Manager.CreateEntity(ComponentType.ReadOnly(typeof(ClearClientGhostEntities.ClientClearGhosts)));
            Entity testGhost = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<GhostComponent>(testGhost);
            Entity testGhost2 = base.m_Manager.CreateEntity();
            base.m_Manager.AddComponent<GhostComponent>(testGhost2);
            Entity testNonGhost = base.m_Manager.CreateEntity();

            this.ghostEntitySystem.Update();

            EndSimulationEntityCommandBufferSystem buffer = base.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            buffer.Update();

            Assert.IsFalse(base.m_Manager.Exists(testGhost));
            Assert.IsFalse(base.m_Manager.Exists(testGhost2));
            Assert.IsTrue(base.m_Manager.Exists(testNonGhost));
        }
    }

    /// <summary>
    /// Tests to verify functionality of the connect to server system
    /// </summary>
    [TestFixture]
    public class ConnectToServerSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Connect to server system
        /// </summary>
        private ConnectToServerSystem connectToServerSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.connectToServerSystem = base.World.CreateSystem<ConnectToServerSystem>();
            base.m_Manager.CreateEntity(typeof(ConnectionComponent));
        }

        /// <summary>
        /// Verify that the client will not connect when a connection is not requested
        /// </summary>
        [Test]
        public void TestRequestNone()
        {
            // Setup parameters to request connect = false
            this.connectToServerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    requestConnect = false
                });

            this.connectToServerSystem.Update();
            var connection = this.connectToServerSystem.GetSingleton<ConnectionComponent>();
            bool initComponent = this.connectToServerSystem.HasSingleton<InitClientGameComponent>();

            Assert.IsFalse(initComponent);
            Assert.IsFalse(connection.requestConnect);
        }

        /// <summary>
        /// Verify that the client will connect when connection is requested
        /// </summary>
        [Test]
        public void TestRequestConnect()
        {
            // Setup parameters to request connect = true
            this.connectToServerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    requestConnect = true
                });

            this.connectToServerSystem.Update();
            var connection = this.connectToServerSystem.GetSingleton<ConnectionComponent>();
            bool initComponent = this.connectToServerSystem.HasSingleton<InitClientGameComponent>();

            Assert.IsTrue(initComponent);
            Assert.IsFalse(connection.requestConnect);
            Assert.IsTrue(connection.attemptingConnect);
        }
    }

    /// <summary>
    /// Tests to verify the functionality of the disconnect from server systems
    /// </summary>
    [TestFixture]
    public class DisconnectFromServerSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Disconnect from server system tests
        /// </summary>
        DisconnectFromServerSystem disconnectFromServerSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.disconnectFromServerSystem = base.World.CreateSystem<DisconnectFromServerSystem>();
            base.m_Manager.CreateEntity(typeof(ConnectionComponent));
        }

        /// <summary>
        /// Test changes made when not requesting disconnection
        /// </summary>
        [Test]
        public void TestNoRequestDisconnect()
        {
            // Setup parameters to request connect = true
            this.disconnectFromServerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    requestDisconnect = false
                });
            // Create an entity with the NetworkStreamConnection component
            base.m_Manager.CreateEntity(typeof(NetworkStreamConnection));

            this.disconnectFromServerSystem.Update();
            var connection = this.disconnectFromServerSystem.GetSingleton<ConnectionComponent>();
            var networkStreamEntity = base.m_Manager.CreateEntityQuery(typeof(NetworkStreamConnection)).ToEntityArray(Unity.Collections.Allocator.Persistent)[0];
            bool hasDisconnect = base.m_Manager.HasComponent<NetworkStreamRequestDisconnect>(networkStreamEntity);

            Assert.IsFalse(connection.requestDisconnect);
            Assert.IsFalse(connection.attemptingDisconnect);
            Assert.IsFalse(hasDisconnect);
        }

        /// <summary>
        /// Test changes made when requesting disconnect
        /// </summary>
        [Test]
        public void TestWithNetworkStreamComponent()
        {
            // Setup parameters to request connect = true
            this.disconnectFromServerSystem.SetSingleton<ConnectionComponent>(
                new ConnectionComponent
                {
                    requestDisconnect = true
                });
            // Create an entity with the NetworkStreamConnection component
            base.m_Manager.CreateEntity(typeof(NetworkStreamConnection));

            this.disconnectFromServerSystem.Update();
            var connection = this.disconnectFromServerSystem.GetSingleton<ConnectionComponent>();
            var networkStreamEntity = base.m_Manager.CreateEntityQuery(typeof(NetworkStreamConnection)).ToEntityArray(Unity.Collections.Allocator.Persistent)[0];
            bool hasDisconnect = base.m_Manager.HasComponent<NetworkStreamRequestDisconnect>(networkStreamEntity);

            Assert.IsFalse(connection.requestDisconnect);
            Assert.IsTrue(connection.attemptingDisconnect);
            Assert.IsTrue(hasDisconnect);
        }
    }
}