using UnityEditor;

namespace EditorNamespace
{
    public class DevelopmentBuilder
    {
        private static BuildOptions BuildOptionsClient()
        {
            return BuildOptions.Development;
        }

        private static BuildOptions BuildOptionsServer()
        {
            return BuildOptions.Development | BuildOptions.EnableHeadlessMode;
        }

        private static readonly string[] DevelopmentScenes = {"Assets/Scenes/SampleScene.unity"};

        public static void Client_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Windows-Client/Windows-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Server_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Windows-Server/Windows-Server.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Client_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Windows64-Client/Windows64-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Server_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Windows64-Server/Windows64-Server.exe";
            playerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Client_Linux64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = DevelopmentScenes;
            playerOptions.locationPathName = "Builds/Linux64-Client/Linux64-Client.exe";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

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
