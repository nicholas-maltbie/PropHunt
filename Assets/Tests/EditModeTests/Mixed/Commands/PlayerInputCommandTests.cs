using NUnit.Framework;
using Unity.Entities.Tests;
using PropHunt.Mixed.Commands;
using Unity.Networking.Transport;
using Unity.Collections;

namespace PropHunt.EditMode.Tests.Mixed.Commands
{
    /// <summary>
    /// Tests for the Player Input Commands
    /// </summary>
    [TestFixture]
    public class PlayerInputCommandTests : ECSTestsFixture
    {
        [Test]
        public void TestPlayerInputSerializeDeserialize()
        {
            PlayerInput playerInput = new PlayerInput{
                tick = 0u,
                horizMove = -1.0f,
                vertMove = 1.0f,
                targetYaw = 33.3f,
                targetPitch = 33.3f,
                jump = 1,
                interact = 1,
                sprint = 1
            };

            Assert.IsTrue(playerInput.IsSprinting);
            Assert.IsTrue(playerInput.IsJumping);
            Assert.IsTrue(playerInput.IsInteracting);

            DataStreamWriter writer = new DataStreamWriter(100, Allocator.Temp);
            
            // serialize data
            playerInput.Serialize(ref writer);
            NativeArray<byte> bytes = writer.AsNativeArray();

            // put data into reader
            DataStreamReader reader = new DataStreamReader(bytes);

            // Get the output.
            PlayerInput playerInput2 = new PlayerInput();
            playerInput2.Deserialize(0u, ref reader);

            // Assert data is passed correctely.
            Assert.IsTrue(playerInput.tick == playerInput2.tick);
            Assert.IsTrue(playerInput.horizMove == playerInput2.horizMove);
            Assert.IsTrue(playerInput.vertMove == playerInput2.vertMove);
            Assert.IsTrue(playerInput.targetYaw == playerInput2.targetYaw);
            Assert.IsTrue(playerInput.targetPitch == playerInput2.targetPitch);
            Assert.IsTrue(playerInput.jump == playerInput2.jump);
            Assert.IsTrue(playerInput.interact == playerInput2.interact);
            Assert.IsTrue(playerInput.sprint == playerInput2.sprint);
        }
    }
}