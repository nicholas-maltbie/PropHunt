
using PropHunt.InputManagement;
using PropHunt.Mixed.Utilities;
using Unity.Entities;

namespace PropHunt.Mixed.Utility
{
    public abstract class PredictedStateSystem : SystemBase
    {
        /// <summary>
        /// Prediction manager for determining state update in a testable manner
        /// </summary>
        public IPredictionState predictionManager = new PredictionState();

        /// <summary>
        /// Unity service for making the class testable
        /// </summary>
        public IUnityService unityService = new UnityService();
    }
}