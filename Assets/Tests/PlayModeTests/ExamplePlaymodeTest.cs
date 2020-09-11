using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities.Tests;

namespace PropHunt.PlayMode.Tests
{
    [TestFixture]
    public class ExamplePlaymodeTest : ECSTestsFixture
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        // A Test behaves as an ordinary method
        [Test]
        public void ExamplePlaymodeTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ExamplePlaymodeTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return new WaitForEndOfFrame();
        }
    }
}
