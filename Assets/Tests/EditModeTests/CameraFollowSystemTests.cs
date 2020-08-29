using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Tests
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
                this.camera.AddComponent<Camera>();
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
                Assert.IsTrue(position == this.camera.transform.position);
            }
            if (rotation != null)
            {
                Assert.IsTrue(rotation == this.camera.transform.rotation);
            }
        }

        /// <summary>
        /// Creates the network id component singleton and EnableProphuntGhostReceiveSystemComponent singleton
        /// for updating the player camera
        /// </summary>
        /// <param name="networkId">Player id to give the created NetworkIDComponent</param>
        private void CreateNetworkSingletons(int networkId)
        {
            // System for creating and editing singletons
            var system = World.GetExistingSystem<ComponentSystemBase>();

            // Setup singleton data
            var networkIDEntity = World.EntityManager.CreateEntity(ComponentType.ReadOnly<NetworkIdComponent>());
            World.EntityManager.CreateEntity(ComponentType.ReadOnly<EnableProphuntGhostReceiveSystemComponent>());

            // Set singleton data
            system.SetSingleton<NetworkIdComponent>(new NetworkIdComponent {Value = networkId});
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
            World.Update();

            // Ensure camera has not moved
            this.AssertCameraTransform(CameraFollowSystemTests.StartingPosition, CameraFollowSystemTests.StartingRotation);
        }

        /// <summary>
        /// Ensure camera follow system does not update without singletons
        /// </summary>
        [Test]
        public void CameraFollowSystem_NoSingletons()
        {
            // Create target entity
            var entity = this.CreateTargetEntity();

            // Move starting entity
            Vector3 position = new float3(CameraFollowSystemTests.StartingPosition) + new float3(1, 1, 1);
            m_Manager.SetComponentData(entity, new Translation {Value = position});

            // Ensure that camera doesn't move without required singletons
            World.Update();
            
            // Ensure camera has not moved
            this.AssertCameraTransform(CameraFollowSystemTests.StartingPosition, CameraFollowSystemTests.StartingRotation);
        }

        /// <summary>
        /// Test to ensure that Update method is called with singletons
        /// </summary>
        [Test]
        public void CameraFollowSystem_WithSingletons()
        {
            // Create target entity
            var cameraTarget = this.CreateTargetEntity();
            int playerId = 1;

            // Move starting entity
            Vector3 targetPos = new float3(CameraFollowSystemTests.StartingPosition) + new float3(1, 1, 1);
            m_Manager.SetComponentData(cameraTarget, new Translation {Value = targetPos});
            m_Manager.SetComponentData(cameraTarget, new PlayerId {playerId = playerId});
            this.CreateNetworkSingletons(playerId);

            // Ensure that camera doesn't move without required singletons
            World.Update();
            
            // Ensure camera has not moved
            this.AssertCameraTransform(targetPos, CameraFollowSystemTests.StartingRotation);
        }
    }
}
