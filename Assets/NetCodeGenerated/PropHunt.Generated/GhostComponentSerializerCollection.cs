//THIS FILE IS AUTOGENERATED BY GHOSTCOMPILER. DON'T MODIFY OR ALTER.
using Unity.Entities;
using Unity.NetCode;
using PropHunt.Generated;

namespace PropHunt.Generated
{
    [UpdateInGroup(typeof(ClientAndServerInitializationSystemGroup))]
    public class GhostComponentSerializerRegistrationSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var ghostCollectionSystem = World.GetOrCreateSystem<GhostCollectionSystem>();
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsKCCMovementSettingsGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsKCCJumpingGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsKCCGroundedGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsKCCVelocityGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsKCCGravityGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsMaterialIdComponentDataGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsMovementTrackingGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsMovingPlatformGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsPlayerIdGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsPlayerViewGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(PropHuntMixedComponentsRotatingPlatformGhostComponentSerializer.State);
        }

        protected override void OnUpdate()
        {
            var parentGroup = World.GetExistingSystem<InitializationSystemGroup>();
            if (parentGroup != null)
            {
                parentGroup.RemoveSystemFromUpdateList(this);
            }
        }
    }
}