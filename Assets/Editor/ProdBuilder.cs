using System.IO;
using UnityEditor;

namespace EditorNamespace
{

    /// <summary>
    /// Production build instructions
    /// </summary>
    public class ProdBuilder
    {
        /// <summary>
        /// Return Building Options for production client
        /// </summary>
        /// <returns>Build options for prod client</returns>
        private static BuildOptions BuildOptionsClient()
        {
            return BuildOptions.None;
        }

        /// <summary>
        /// Return Building Options for production server
        /// </summary>
        /// <returns>Build options for prod server</returns>
        private static BuildOptions BuildOptionsServer()
        {
            return BuildOptions.EnableHeadlessMode;
        }

        /// <summary>
        /// Scenes used in development
        /// </summary>
        private static readonly string[] ProdScenes = {"Assets/Scenes/SampleScene.unity"};

        /// <summary>
        /// Build a Windows 32 bit client
        /// </summary>
        public static void Client_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows-Client/Windows-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a Windows 32 bit server
        /// </summary>
        public static void Server_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows-Server/Windows-Server.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a Windows 64 bit client
        /// </summary>
        public static void Client_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows64-Client/Windows64-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a Windows 64 bit server
        /// </summary>
        public static void Server_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows64-Server/Windows64-Server.exe";
            playerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a Linux 64 bit client
        /// </summary>
        public static void Client_Linux64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Linux64-Client/Linux64-Client.x86_64";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a Linux 64 bit server
        /// </summary>
        public static void Server_Linux64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Linux64-Server/Linux64-Server.x86_64";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build an OSX client
        /// </summary>
        public static void Client_OSX()
        {
            BuildOptions options = BuildOptionsClient();
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = options;
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/OSX-Client/OSX-Client.exe";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build an OSX server
        /// </summary>
        public static void Server_OSX()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/OSX-Server/OSX-Server.exe";
            playerOptions.target = BuildTarget.StandaloneOSX;
            BuildPipeline.BuildPlayer(playerOptions);
        }
    }
}
