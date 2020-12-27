using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Mixed.Utils
{
    /// <summary>
    /// Structure for contains the static decision of predicting at a given tick
    /// </summary>
    public interface IPredictionState
    {
        /// <summary>
        /// Get the current predicting tick of the world
        /// </summary>
        /// <param name="world">World for current entity predictions</param>
        /// <returns>The current prediction tick</returns>
        uint GetPredictingTick(World world);
    }

    public class PredictionState : IPredictionState
    {
        /// <inheritdoc/>
        public uint GetPredictingTick(World world)
        {
            return world.GetExistingSystem<GhostPredictionSystemGroup>().PredictingTick;
        }
    }
}