using PropHunt.InputManagement;
using Unity.Entities;

namespace PropHunt.Mixed.Utilities
{
    public abstract class PredictionStateSystem : SystemBase
    {
        /// <summary>
        /// Unity service for making the class testable
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Prediction manager for determining state update in a testable manner
        /// </summary>
        public IPredictionState predictionManager = new PredictionState();
    }
}
