
using PropHunt.Mixed.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PropHunt.Authoring
{
    /// <summary>
    /// Behaviour to create a Door based on user provided settings
    /// </summary>
    public class DoorAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        /// <summary>
        /// Postion and rotation of the door when opened
        /// </summary>
        public Transform openedState;

        /// <summary>
        /// Postion and rotation of the door when closed
        /// </summary>
        public Transform closedState;

        /// <summary>
        /// Time it takes to translate between opened and closed state
        /// </summary>
        public float transitionTime = 1.0f;

        /// <summary>
        /// Starting state of the door
        /// </summary>
        public DoorState startingState = DoorState.Closed;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<Door>(entity, new Door
            {
                state = this.startingState,
                transitionTime = this.transitionTime,
                openedPosition = openedState.position,
                closedPosition = closedState.position,
                openedRotation = math.radians(openedState.rotation.eulerAngles),
                closedRotation = math.radians(closedState.rotation.eulerAngles),
            });
        }
    }
}