using Unity.Entities;
using Unity.NetCode;

namespace PropHunt.Mixed.Utilities
{
    /// <summary>
    /// Structure for contains the static decision of predicting at a given tick
    /// </summary>
    public interface IPredictionState
    {
        /// <summary>
        /// Should a prediction be made at a given tick for a given component
        /// </summary>
        /// <param name="tick">Time of event</param>
        /// <param name="prediction">Prediction component of the object</param>
        /// <returns>True if a prediction should be made, false otherwise</returns>
        bool ShouldPredict(uint tick, in PredictedGhostComponent prediction);

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
        public bool ShouldPredict(uint tick, in PredictedGhostComponent prediction)
        {
            return GhostPredictionSystemGroup.ShouldPredict(tick, prediction);
        }

        /// <inheritdoc/>
        public uint GetPredictingTick(World world)
        {
            return world.GetExistingSystem<GhostPredictionSystemGroup>().PredictingTick;
        }
    }
}