using System.Collections.Generic;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace PropHunt.Tests.Client
{
    [TestFixture]
    public class CameraFollowSystemTests : ECSTestsFixture
    {
        /// <summary>
        /// Main camera in the scene
        /// </summary>
        private GameObject camera;

        /// <summary>
        /// System for camera follow system
        /// </summary>
        private CameraFollowSystem cameraFollow;

        /// <summary>
        /// Starting position of camera
        /// </summary>
        public static readonly Vector3 StartingPosition = Vector3.zero;

        /// <summary>
        /// Starting attitude of camera
        /// </summary>
        public static readonly Quaternion StartingRotation = Quaternion.identity;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Ensure that camera exists
            if (Camera.main == null)
            {
                this.camera = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                var cameraComponent = this.camera.AddComponent<Camera>();
                this.camera.tag = "MainCamera";
            }
            else
            {
                this.camera = Camera.main.gameObject;
                this.camera.transform.position = StartingPosition;
                this.camera.transform.rotation = StartingRotation;
            }

            // Setup camera follow system
            this.cameraFollow = World.CreateSystem<CameraFollowSystem>();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Clean up camera object
            GameObject.DestroyImmediate(this.camera);
        }

        /// <summary>
        /// Assert that the transform of the camera matches some set of values
        /// </summary>
        /// <param name="position">position of the camera, if null will skip</param>
        /// <param name="rotation">rotation of the camera, if null will skip</param>
        private void AssertCameraTransform(Vector3 position, Quaternion rotation)
        {
            if (position != null)
            {
                Assert.IsTrue(position == this.camera.transform.position,
                    $"Expected translation {position} but found translation {this.camera.transform.position}");
            }
            if (rotation != null)
            {
                Assert.IsTrue(rotation == this.camera.transform.rotation,
                    $"Expected rotation {rotation} but found rotation {this.camera.transform.rotation}");
            }
        }

        /// <summary>
        /// Creates the network id component singleton and EnableProphuntGhostReceiveSystemComponent singleton
        /// for updating the player camera
        /// </summary>
        /// <param name="networkId">Player id to give the created NetworkIDComponent</param>
        private void CreateNetworkSingletons(int networkId)
        {
            var system = World.GetExistingSystem<CameraFollowSystem>();

            // Setup singleton data
            if (!system.HasSingleton<NetworkIdComponent>())
            {
                World.EntityManager.CreateEntity(ComponentType.ReadOnly<NetworkIdComponent>());
            }
            if (!system.HasSingleton<EnableProphuntGhostReceiveSystemComponent>())
            {
                World.EntityManager.CreateEntity(ComponentType.ReadOnly<EnableProphuntGhostReceiveSystemComponent>());
            }
            var networkIDEntity = system.GetSingleton<NetworkIdComponent>(); 

            // Set singleton data
            system.SetSingleton<NetworkIdComponent>(new NetworkIdComponent {Value = networkId});
            system.SetSingleton<EnableProphuntGhostReceiveSystemComponent>(new EnableProphuntGhostReceiveSystemComponent());
        }

        /// <summary>
        /// Creates a target entity for camera system
        /// </summary>
        /// <returns></returns>
        private Entity CreateTargetEntity()
        {
            return this.m_Manager.CreateEntity(
                typeof(Translation),
                typeof(Rotation),
                typeof(PlayerId),
                typeof(PlayerView)
            );
        }

        /// <summary>
        /// Assert that the camera exists
        /// </summary>
        [Test]
        public void CheckCameraExists()
        {
            Assert.IsTrue(this.camera.GetComponent<Camera>() != null);
        }

        /// <summary>
        /// Evaluate Camera Follow System with no target
        /// </summary>
        [Test]
        public void CameraFollowSystem_NoTarget()
        {
            // Do not create a target entity

            // Do an update step for the system
            this.cameraFollow.Update();

            // Ensure camera has not moved
            this.AssertCameraTransform(CameraFollowSystemTests.StartingPosition, CameraFollowSystemTests.StartingRotation);
        }

        /// <summary>
        /// Ensure camera follow system does not update without singletons
        /// </summary>
        [Test]
        public void CameraFollowSystem_NoSingletons()
        {
            // Setup camera follow system mock for update function
            Mock<CameraFollowSystem> cameraFollowMock = new Mock<CameraFollowSystem>();
            this.cameraFollow = World.AddSystem<CameraFollowSystem>(cameraFollowMock.Object);
            cameraFollowMock.Protected().Setup("OnUpdate");

            // Destroy the netwok singletons
            if (this.cameraFollow.HasSingleton<NetworkIdComponent>())
            {
                World.EntityManager.DestroyEntity(World.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<NetworkIdComponent>()));
            }
            if (this.cameraFollow.HasSingleton<EnableProphuntGhostReceiveSystemComponent>())
            {
                World.EntityManager.DestroyEntity(World.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<EnableProphuntGhostReceiveSystemComponent>()));
            }

            // Ensure that camera doesn't move without required singletons
            this.cameraFollow.Update();

            // Ensure update function has not been invoked
            cameraFollowMock.Protected().Verify("OnUpdate", Times.Never());;
        }

        /// <summary>
        /// Test to ensure that Update method is called with singletons
        /// </summary>
        [Test]
        public void CameraFollowSystem_WithSingletons()
        {
            // Setup camera follow system mock for update function
            Mock<CameraFollowSystem> cameraFollowMock = new Mock<CameraFollowSystem>();
            this.cameraFollow = World.AddSystem<CameraFollowSystem>(cameraFollowMock.Object);
            cameraFollowMock.Protected().Setup("OnUpdate");

            // Create target entity
            var cameraTarget = this.CreateTargetEntity();
            int playerId = 1;

            // Move starting entity
            Vector3 targetPos = new float3(CameraFollowSystemTests.StartingPosition) + new float3(1, 1, 1);
            m_Manager.SetComponentData(cameraTarget, new Translation {Value = targetPos});
            m_Manager.SetComponentData(cameraTarget, new PlayerId {playerId = playerId});
            this.CreateNetworkSingletons(playerId);

            // Ensure that camera doesn't move without required singletons
            this.cameraFollow.Update();

            // Ensure update function has been invoked
            cameraFollowMock.Protected().Verify("OnUpdate", Times.Once());
        }

        /// <summary>
        /// Ensure that the camera follow system will follow a moving object
        /// </summary>
        [Test]
        public void CameraFollowSystem_MultipleTarget()
        {
            // Create target entity1
            var cameraTarget1 = this.CreateTargetEntity();
            var cameraTarget2 = this.CreateTargetEntity();

            int playerId = 1;
            int notPlayerId = playerId - 1;
            this.CreateNetworkSingletons(playerId);

            Vector3 pos1 = new Vector3(1, 1, 1);
            Vector3 pos2 = new Vector3(-1, -1, -1);

            m_Manager.SetComponentData(cameraTarget1, new Translation {Value = pos1});
            m_Manager.SetComponentData(cameraTarget2, new Translation {Value = pos2});

            m_Manager.SetComponentData(cameraTarget1, new PlayerId {playerId = playerId});
            m_Manager.SetComponentData(cameraTarget2, new PlayerId {playerId = notPlayerId});
            
            // Ensure that camera moved to follow target 1
            this.cameraFollow.Update();
            this.AssertCameraTransform(pos1, CameraFollowSystemTests.StartingRotation);

            // Change target camera
            m_Manager.SetComponentData(cameraTarget1, new PlayerId {playerId = notPlayerId});
            m_Manager.SetComponentData(cameraTarget2, new PlayerId {playerId = playerId});

            // Ensure that camera moved to follow target 2
            this.cameraFollow.Update();
            this.AssertCameraTransform(pos2, CameraFollowSystemTests.StartingRotation);
        }

        /// <summary>
        /// Ensure that the camera follow system will follow a moving object
        /// </summary>
        [Test]
        public void CameraFollowSystem_MoveWithTarget()
        {
            // Create target entity
            var cameraTarget = this.CreateTargetEntity();
            int playerId = 1;

            // Move starting entity
            Vector3 changePos = new Vector3(1, 1, 1);
            float changePitch = 1f;
            float changeYaw = 2f;

            // Setup track for target position and rotation
            Vector3 targetPos = CameraFollowSystemTests.StartingPosition;
            PlayerView targetView = new PlayerView() {pitch = 0, yaw = 0};

            // Setup singletons
            this.CreateNetworkSingletons(playerId);

            int updates = 10;
            for (int i = 0; i < updates; i++)
            {
                // Move target positions
                targetPos += changePos;
                targetView.pitch += changePitch;
                targetView.yaw += changeYaw;
                m_Manager.SetComponentData(cameraTarget, new Translation {Value = targetPos});
                m_Manager.SetComponentData(cameraTarget, targetView);
                m_Manager.SetComponentData(cameraTarget, new PlayerId {playerId = playerId});

                // Ensure that camera doesn't move without required singletons
                this.cameraFollow.Update();

                // Ensure position and rotation is correct
                this.AssertCameraTransform(targetPos, Quaternion.Euler(targetView.pitch, targetView.yaw, 0));
            }
        }
    }
}
