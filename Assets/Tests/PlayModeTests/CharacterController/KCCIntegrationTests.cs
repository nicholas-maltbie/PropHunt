using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities.Tests;
using PropHunt.Authoring;
using Unity.Entities;
using Unity.Physics.Authoring;
using Unity.Mathematics;
using PropHunt.Mixed.Systems;
using Unity.NetCode;
using PropHunt.Game;
using static PropHunt.Game.ProphuntClientServerControlSystem;
using UnityEditor;
using Unity.Physics.Systems;
using UnityEngine.SceneManagement;
using PropHunt.PlayMode.Tests.Utility;

namespace PropHunt.PlayMode.Tests.CharacterController
{
    [TestFixture]
    public class KinematicCharacterControllerTest
    {
        private Entity playerEntity;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {            
            SceneManager.LoadScene("SampleScene");

            var waitForScene = new WaitForSceneLoaded("SampleScene", 30);
            yield return waitForScene;
            Assert.IsFalse(waitForScene.TimedOut, $"Scene failed to load");

            // Wait for a frame to pass
            yield return null;

            Debug.Log(World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.NumBodies);
            // Spawn a character
            // GameObject player = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);

            // PlayerMovementAuthoringComponent movement = player.AddComponent<PlayerMovementAuthoringComponent>();
            // movement.moveSpeed = 5f;
            // movement.sprintMultiplier = 2f;
            // movement.gravityForce = new Vector3(0, -9.8f, 0);
            // movement.jumpForce = 5.0f;
            // movement.maxWalkAngle = 45;
            // movement.groundCheckDistance = 0.1f;
            // movement.fallAnglePower = 1.2f;
            // movement.moveAnglePower = 2.0f;
            // movement.pushPower = 20.0f;
            // movement.moveMaxBounces = 3;
            // movement.fallMaxBounces = 2;
            // movement.pushDecay = 0.0f;

            // PlayerViewAuthoringComponent playerView = player.AddComponent<PlayerViewAuthoringComponent>();
            // playerView.viewRotationRate = 180f;
            // playerView.offset = Vector3.zero;

            // PhysicsShapeAuthoring physicsShape = player.AddComponent<PhysicsShapeAuthoring>();
            // physicsShape.CollidesWith = PhysicsCategoryTags.Everything;
            // physicsShape.Friction = new PhysicsMaterialCoefficient(){Value = 1};
            // physicsShape.Restitution = new PhysicsMaterialCoefficient(){Value = 1};
            // physicsShape.SetCapsule(new CapsuleGeometryAuthoring(){
            //     Orientation = Quaternion.Euler(-90, 0, -90),
            //     Center = new float3(0, 1, 0),
            //     Height = 2,
            //     Radius = 0.5f
            // });

            // PhysicsBodyAuthoring physicsBody = player.AddComponent<PhysicsBodyAuthoring>();
            // physicsBody.MotionType = BodyMotionType.Kinematic;

            // PlayerIdAuthoringComponent playerId = player.AddComponent<PlayerIdAuthoringComponent>();

            // yield return null;

            // GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            // plane.transform.position = Vector3.zero;
            // var planeShape = plane.AddComponent<PhysicsShapeAuthoring>();
            // planeShape.SetPlane(float3.zero, new float2(10, 10), quaternion.identity);
            // var planeBody = plane.AddComponent<PhysicsBodyAuthoring>();
            // planeBody.MotionType = BodyMotionType.Static;

            // var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
            // this.playerEntity = Unity.Entities.GameObjectConversionUtility.ConvertGameObjectHierarchy(player, settings);
            // var planeEntity = Unity.Entities.GameObjectConversionUtility.ConvertGameObjectHierarchy(plane, settings);
            // Debug.Log(World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentTypes(this.playerEntity));

            // var physicsWorld = buildPhysicsWorld.PhysicsWorld;

            // Debug.Log($"Num bodies: {physicsWorld.NumBodies}");

            // GameObject.Destroy(player);
        }

        private void UpdateKCCSystems()
        {
            // World.GetOrCreateSystem<KCCGroundedSystem>().Update();
            // World.GetOrCreateSystem<KCCGravitySystem>().Update();
            // World.GetOrCreateSystem<KCCJumpSystem>().Update();
            // World.GetOrCreateSystem<KCCMovementSystem>().Update();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ExamplePlaymodeTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
