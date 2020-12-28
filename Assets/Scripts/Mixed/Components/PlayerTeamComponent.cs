using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Mixed.Components
{
    /// <summary>
    /// Enum that indicates the type of team a player can be on
    /// </summary>
    public enum TeamType
    {
        Prop,
        Hunter,
        Spectator
    };

    /// <summary>
    /// Component for identifying a player team.
    /// </summary>
    [GenerateAuthoringComponent]
    [GhostComponent]
    public struct PlayerTeam: IComponentData
    {
        /// <summary>
        /// Number identifying which team any given player is on.
        /// </summary>
        [GhostField]
        public TeamType TeamValue;
    }
}