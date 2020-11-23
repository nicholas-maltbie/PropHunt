using NUnit.Framework;
using PropHunt.Game;
using Unity.Entities.Tests;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Flow
{
    [TestFixture]
    public class ServerGameSystemTests : ECSTestsFixture
    {
        private ServerGameSystem serverGameSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.serverGameSystem = base.World.CreateSystem<ServerGameSystem>();
            base.World.CreateSystem<NetworkStreamReceiveSystem>();
            base.World.CreateSystem<NetworkControlSettingsSystem>();
        }

        [Test]
        public void VerifyServerSystemSetupWithSeverSystem()
        {
            base.World.CreateSystem<ServerSimulationSystemGroup>();
            this.serverGameSystem.Update();
            Assert.IsFalse(this.serverGameSystem.HasSingleton<ServerGameSystem.InitServerGameComponent>());
        }

        [Test]
        public void VerifyServerSystemSetupWithoutSeverSystem()
        {
            base.World.DestroySystem(base.World.GetOrCreateSystem<ServerSimulationSystemGroup>());
            this.serverGameSystem.Update();
        }
    }

    [TestFixture]
    public class ClientGameSystemTests : ECSTestsFixture
    {
        private ClientGameSystem clientGameSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.clientGameSystem = base.World.CreateSystem<ClientGameSystem>();
            base.World.CreateSystem<NetworkControlSettingsSystem>();
            base.World.CreateSystem<NetworkStreamReceiveSystem>();
            base.m_Manager.CreateEntity(typeof(ClientGameSystem.InitClientGameComponent));
        }

        [Test]
        public void VerifyClientSystemSetupWithClientSystem()
        {
            base.World.CreateSystem<ClientSimulationSystemGroup>();
            this.clientGameSystem.Update();
            Assert.IsFalse(this.clientGameSystem.HasSingleton<ClientGameSystem.InitClientGameComponent>());
        }

        [Test]
        public void VerifyClientSystemSetupWithoutSeverSystem()
        {
            base.World.DestroySystem(base.World.GetOrCreateSystem<ClientSimulationSystemGroup>());
            this.clientGameSystem.Update();
            Assert.IsFalse(this.clientGameSystem.HasSingleton<ClientGameSystem.InitClientGameComponent>());
        }
    }

    [TestFixture]
    public class NetworkControlSettingsSystemTests : ECSTestsFixture
    {
        private NetworkControlSettingsSystem networkControlSettingsSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.networkControlSettingsSystem = base.World.CreateSystem<NetworkControlSettingsSystem>();
            base.World.CreateSystem<NetworkStreamReceiveSystem>();
            base.m_Manager.CreateEntity(typeof(ClientGameSystem.InitClientGameComponent));
        }

        [Test]
        public void VerifyUpdateFunctionality()
        {
            this.networkControlSettingsSystem.Update();
        }
    }
}