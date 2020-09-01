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
            BuildPipeline.BuildPlayer(DevelopmentScenes, "Windows-Client/Windows-Client.exe", BuildTarget.StandaloneWindows, BuildOptionsClient());
        }

        public static void Server_Windows()
        {
            BuildPipeline.BuildPlayer(DevelopmentScenes, "Windows-Server/Windows-Server.exe", BuildTarget.StandaloneWindows, BuildOptionsServer());
        }

        public static void Client_Windows64()
        {
            BuildPipeline.BuildPlayer(DevelopmentScenes, "Windows64-Client/Windows64-Client.exe", BuildTarget.StandaloneWindows64, BuildOptionsClient());
        }

        public static void Server_Windows64()
        {
            BuildPipeline.BuildPlayer(DevelopmentScenes, "Windows64-Server/Windows64-Server.exe", BuildTarget.StandaloneWindows64, BuildOptionsServer());
        }

        public static void Client_Linux64()
        {
            BuildPipeline.BuildPlayer(DevelopmentScenes, "Linux64-Client/Linux64-Client.exe", BuildTarget.StandaloneLinux64, BuildOptionsClient());
        }

        public static void Server_Linux64()
        {
            BuildPipeline.BuildPlayer(DevelopmentScenes, "Linux64-Server/Linux64-Server.exe", BuildTarget.StandaloneLinux64, BuildOptionsServer());
        }
    }
}
