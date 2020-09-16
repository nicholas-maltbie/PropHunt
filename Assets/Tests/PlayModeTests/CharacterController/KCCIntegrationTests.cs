using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PropHunt.Authoring;
using Unity.Entities;
using Unity.Physics.Authoring;
using Unity.Mathematics;
using PropHunt.Mixed.Systems;
using Unity.Physics.Systems;
using PropHunt.Client.Systems;

namespace PropHunt.PlayMode.Tests.CharacterController
{
    public class TestSystem : ComponentSystem
    {
        public Entity entity;

        protected override void OnUpdate()
        {
            if (entity != Entity.Null)
            {
                PostUpdateCommands.Instantiate(entity);
                entity = Entity.Null;
            }
        }
    }

    [TestFixture]
    public class KinematicCharacterControllerTest
    {
        private Entity playerEntity;

        private GameObject player;

        private BuildPhysicsWorld buildPhysicsWorld;

        private GameObject environment;

        private World world;

        [SetUp]
        public void Setup()
        {
            var settings = GameObjectConversionSettings.FromWorld(this.world, new BlobAssetStore());
            this.playerEntity = Unity.Entities.GameObjectConversionUtility.ConvertGameObjectHierarchy(this.player, settings);
            var environmentEntity = Unity.Entities.GameObjectConversionUtility.ConvertGameObjectHierarchy(this.environment, settings);
            Debug.Log(this.world.EntityManager.GetComponentTypes(this.playerEntity));

            // var physicsWorld = buildPhysicsWorld.PhysicsWorld;

            // Debug.Log($"Num bodies: {physicsWorld.NumBodies}");
            var test = this.world.GetOrCreateSystem<TestSystem>();
            test.entity = this.playerEntity;
            test.Update();

            GameObject.Destroy(this.player);
            GameObject.Destroy(this.environment);
        }

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            this.world = World.DefaultGameObjectInjectionWorld;
            this.buildPhysicsWorld = this.world.GetOrCreateSystem<BuildPhysicsWorld>();

            // Spawn a character
            this.player = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            
            PlayerMovementAuthoringComponent movement = this.player.AddComponent<PlayerMovementAuthoringComponent>();
            movement.moveSpeed = 5f;
            movement.sprintMultiplier = 2f;
            movement.gravityForce = new Vector3(0, -9.8f, 0);
            movement.jumpForce = 5.0f;
            movement.maxWalkAngle = 45;
            movement.groundCheckDistance = 0.1f;
            movement.fallAnglePower = 1.2f;
            movement.moveAnglePower = 2.0f;
            movement.pushPower = 20.0f;
            movement.moveMaxBounces = 3;
            movement.fallMaxBounces = 2;
            movement.pushDecay = 0.0f;

            PlayerViewAuthoringComponent playerView = this.player.AddComponent<PlayerViewAuthoringComponent>();
            playerView.viewRotationRate = 180f;
            playerView.offset = Vector3.zero;

            PhysicsShapeAuthoring physicsShape = this.player.AddComponent<PhysicsShapeAuthoring>();
            physicsShape.CollidesWith = PhysicsCategoryTags.Everything;
            physicsShape.Friction = new PhysicsMaterialCoefficient(){Value = 1};
            physicsShape.Restitution = new PhysicsMaterialCoefficient(){Value = 1};
            physicsShape.SetCapsule(new CapsuleGeometryAuthoring(){
                Orientation = Quaternion.Euler(-90, 0, -90),
                Center = new float3(0, 1, 0),
                Height = 2,
                Radius = 0.5f
            });

            PhysicsBodyAuthoring physicsBody = this.player.AddComponent<PhysicsBodyAuthoring>();
            physicsBody.MotionType = BodyMotionType.Kinematic;

            PlayerIdAuthoringComponent playerId = this.player.AddComponent<PlayerIdAuthoringComponent>();

            this.environment = GameObject.CreatePrimitive(PrimitiveType.Plane);
            this.environment.transform.position = Vector3.zero;
            var planeShape = this.environment.AddComponent<PhysicsShapeAuthoring>();
            planeShape.SetPlane(float3.zero, new float2(10, 10), quaternion.identity);
            var planeBody = this.environment.AddComponent<PhysicsBodyAuthoring>();
            planeBody.MotionType = BodyMotionType.Static;

            // Wait for a frame to pass
            yield return null;
        }

        private void UpdateKCCSystems()
        {
            this.world.GetOrCreateSystem<KCCGroundedSystem>().Update();
            this.world.GetOrCreateSystem<KCCGravitySystem>().Update();
            this.world.GetOrCreateSystem<KCCJumpSystem>().Update();
            this.world.GetOrCreateSystem<KCCMoveWithGroundSystem>().Update();
            this.world.GetOrCreateSystem<KCCMovementSystem>().Update();
            this.world.GetOrCreateSystem<CameraFollowSystem>().Update();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ExamplePlaymodeTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            yield return null;

            // this.world.

            this.UpdateKCCSystems();
            this.world.Update();
            // this.world.EntityManager.CompleteAllJobs();
            // yield return new WaitForFixedUpdate();
            // world.EntityManager.CreateEntity(typeof(FixedClientTickRate));

            Debug.Log(this.buildPhysicsWorld.PhysicsWorld.NumBodies);
        }
    }
}
