using Unity.Entities;
using UnityEngine;
using PropHunt.Mixed.Components;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Behaviour to create a moving platform component based on settings
    /// </summary>
    public class MovingPlatformAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// Various positions that this platform moves between
        /// </summary>
        public Transform[] positions;

        /// <summary>
        /// Methodology for looping between platforms
        /// </summary>
        public PlatformLooping loopMethod;

        /// <summary>
        /// Speed of movement in meters per second
        /// </summary>
        public float speed = 1;

        /// <summary>
        /// Delay when moving between platforms
        /// </summary>
        public float delayBetweenPlatforms = 1;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            DynamicBuffer<MovingPlatformTarget> targets = dstManager.AddBuffer<MovingPlatformTarget>(entity);

            foreach (Transform pos in positions)
            {
                targets.Add(new MovingPlatformTarget() { target = pos.position });
            }

            dstManager.AddComponentData(entity, new MovingPlatform()
            {
                speed = this.speed,
                direction = 1,
                loopMethod = this.loopMethod,
                current = 0,
                delayBetweenPlatforms = this.delayBetweenPlatforms,
                elapsedWaiting = 0,
            });
        }
    }
}