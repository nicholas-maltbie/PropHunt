using UnityEditor;

namespace EditorNamespace
{

    /// <summary>
    /// Development build instructions
    /// </summary>
    public class DevelopmentBuilder
    {
        /// <summary>
        /// Return Building Options for development client
        /// </summary>
        /// <returns>Build options for dev client</returns>
        private static BuildOptions BuildOptionsClient()
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.allowDebugging = true;
            EditorUserBuildSettings.connectProfiler = true;

            return BuildOptions.Development |
                BuildOptions.ConnectWithProfiler |
                BuildOptions.AllowDebugging;
        }

        /// <summary>
        /// Return Building Options for development client
        /// </summary>
        /// <returns>Build options for dev client</returns>
        private static BuildOptions BuildOptionsServer()
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.allowDebugging = true;
            EditorUserBuildSettings.connectProfiler = true;

            return BuildOptions.Development |
                BuildOptions.AllowDebugging |
                BuildOptions.ConnectWithProfiler |
                BuildOptions.EnableHeadlessMode;
        }

        /// <summary>
        /// Scenes used in development
        /// </summary>
        private static readonly string[] DevelopmentScenes = {"Assets/Scenes/SampleScene.unity"};

        /// <summary>
        /// Build a windows 32 bit client
        /// </summary>
        public static void Client_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Windows-Client/Windows-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a windows 32 bit server
        /// </summary>
        public static void Server_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Windows-Server/Windows-Server.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a windows 64 bit client
        /// </summary>
        public static void Client_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Windows64-Client/Windows64-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        /// <summary>
        /// Build a windows 64 bit server
        /// </summary>
        public static void Server_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = DevelopmentScenes;
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
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Linux64-Client/Linux64-Client.exe";
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
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Linux64-Server/Linux64-Server.exe";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);
        }
    }
}
