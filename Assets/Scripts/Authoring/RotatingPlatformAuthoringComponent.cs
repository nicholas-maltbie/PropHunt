using Unity.Entities;
using UnityEngine;
using PropHunt.Mixed.Components;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Behaviour to create a moving platform component based on settings
    /// </summary>
    public class RotatingPlatformAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// Various translations that this platform moves between
        /// </summary>
        public Transform[] translations;

        /// <summary>
        /// Methodology for looping between platforms
        /// </summary>
        public PlatformLooping loopMethod;

        /// <summary>
        /// Speed of movement in meters per second
        /// </summary>
        public float speed = 1;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            DynamicBuffer<RotatingPlatformTarget> targets = dstManager.AddBuffer<RotatingPlatformTarget>(entity);

            foreach (Transform trans in translations)
            {
                targets.Add(new RotatingPlatformTarget() { target = trans.rotation.eulerAngles });
            }

            dstManager.AddComponentData(entity, new RotatingPlatform()
            {
                speed = this.speed,
                currentAngle = transform.rotation.eulerAngles,
                direction = 1,
                loopMethod = this.loopMethod,
                current = 0
            });
        }
    }
}