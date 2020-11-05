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
    public class JoinGameServerSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// JoinGameServerSystem for testing the system
        /// </summary>
        private JoinGameServerSystem joinGameServerSystem;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.joinGameServerSystem = base.World.CreateSystem<JoinGameServerSystem>();
        }
    }
}