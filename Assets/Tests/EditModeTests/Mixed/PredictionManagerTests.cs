using NUnit.Framework;
using PropHunt.Mixed.Utilities;
using Unity.Entities.Tests;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.Mixed
{
    [TestFixture]
    public class PredictionManagerTests : ECSTestsFixture
    {
        [Test]
        public void VerifyPredictionManagerInvokeWithoutErrors()
        {
            IPredictionState predManager = new PredictionState();
            base.World.CreateSystem<GhostPredictionSystemGroup>();
            Assert.IsTrue(predManager.GetPredictingTick(base.World) >= 0u);
        }
    }
}