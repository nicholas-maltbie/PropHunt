using Moq;
using NUnit.Framework;
using PropHunt.Client.Systems;
using PropHunt.UI;
using Unity.Entities.Tests;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.EditMode.Tests.UI
{
    /// <summary>
    /// Tests for various UI Actions such as connect, disconnect, quit game actions
    /// </summary>
    [TestFixture]
    public class UIActionTests : ECSTestsFixture
    {
        private GameObject uiHolder;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.uiHolder = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            GameObject.DestroyImmediate(this.uiHolder);
        }

        [Test]
        public void QuitGameActionTests()
        {
            this.uiHolder.AddComponent<QuitGameAction>();
            QuitGameAction action = this.uiHolder.GetComponent<QuitGameAction>();
            // Just call the method
            action.QuitGame();
        }

        [Test]
        public void DisconnectActionTests()
        {
            this.uiHolder.AddComponent<DisconnectAction>();
            DisconnectAction action = this.uiHolder.GetComponent<DisconnectAction>();
            Mock<ConnectionSystem> systemMock = new Mock<ConnectionSystem>();
            ConnectionSystem.Instance = systemMock.Object;

            int calls = 0;
            systemMock.Setup(e => e.RequestDisconnect()).Callback(() => calls++);

            // Simply invoke the method
            action.DisconnectClient();
            Assert.IsTrue(calls == 1);
        }

        [Test]
        public void ConnectActionTests()
        {
            this.uiHolder.AddComponent<ConnectAction>();
            ConnectAction action = this.uiHolder.GetComponent<ConnectAction>();

            // Create an instance of the connection system
            ConnectionSystem.Instance = new ConnectionSystem();

            // Setup parameters of action
            GameObject serverAddressField = GameObject.Instantiate(new GameObject());
            GameObject serverPortField = GameObject.Instantiate(new GameObject());
            serverAddressField.AddComponent<InputField>();
            serverPortField.AddComponent<InputField>();
            // Connect to the action
            action.serverAddress = serverAddressField.GetComponent<InputField>();
            action.serverPort = serverPortField.GetComponent<InputField>();

            // Verify behaviour of on enable method
            action.OnEnable();
            Assert.IsTrue(action.serverAddress.text == PropHunt.Game.ProphuntClientServerControlSystem.DefaultNetworkAddress);
            Assert.IsTrue(action.serverPort.text == PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort.ToString());

            // Test for failure to parse
            // Set initial parameters
            PropHunt.Game.ProphuntClientServerControlSystem.NetworkAddress = "";
            PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort = 0;
            action.serverAddress.text = "helloworld";
            action.serverPort.text = "i am groot";

            // Attempt to connect with invalid parameters
            action.ConnectToServer();
            // Assert that address and info has not been updated
            Assert.IsTrue(PropHunt.Game.ProphuntClientServerControlSystem.NetworkAddress == "");
            Assert.IsTrue(PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort == 0);

            // Test for correct parse
            // Set initial parameters
            PropHunt.Game.ProphuntClientServerControlSystem.NetworkAddress = "";
            PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort = 0;
            action.serverAddress.text = "127.0.0.1";
            action.serverPort.text = "1234";

            // Attempt to connect with invalid parameters
            action.ConnectToServer();
            // Assert that address and info has not been updated
            Assert.IsTrue(PropHunt.Game.ProphuntClientServerControlSystem.NetworkAddress == "127.0.0.1");
            Assert.IsTrue(PropHunt.Game.ProphuntClientServerControlSystem.NetworkPort == 1234);

            GameObject.DestroyImmediate(serverAddressField);
            GameObject.DestroyImmediate(serverPortField);
        }
    }
}