using NUnit.Framework;
using PropHunt.Game;
using PropHunt.InputManagement;
using Unity.Entities.Tests;
using Unity.NetCode;

namespace PropHunt.EditMode.Tests.InputManagement
{
    [TestFixture]
    public class UnityServiceTests : ECSTestsFixture
    {
        [Test]
        public void VerifyUnityServiceInvokeWithoutErrors()
        {
            IUnityService unityService = new UnityService();
            unityService.GetAxis("Horizontal");
            unityService.GetButton("Jump");
            unityService.GetButtonDown("Jump");
            Assert.IsTrue(unityService.GetDeltaTime(new Unity.Core.TimeData(0.1f, 0.1f)) == 0.1f);
        }
    }
}