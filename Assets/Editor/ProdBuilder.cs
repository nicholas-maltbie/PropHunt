using UnityEditor;

namespace EditorNamespace
{
    public class ProdBuilder
    {
        private static BuildOptions BuildOptionsClient()
        {
            return BuildOptions.None;
        }

        private static BuildOptions BuildOptionsServer()
        {
            return BuildOptions.EnableHeadlessMode;
        }

        private static readonly string[] ProdScenes = {"Assets/Scenes/SampleScene.unity"};

        public static void Client_StandaloneWindows()
        {
            BuildPipeline.BuildPlayer(ProdScenes, "Windows-Client/Windows-Client.exe", BuildTarget.StandaloneWindows, BuildOptionsClient());
        }

        public static void Server_StandaloneWindows()
        {
            BuildPipeline.BuildPlayer(ProdScenes, "Windows-Server/Windows-Server.exe", BuildTarget.StandaloneWindows, BuildOptionsServer());
        }

        public static void Client_StandaloneWindows64()
        {
            BuildPipeline.BuildPlayer(ProdScenes, "Windows64-Client/Windows64-Client.exe", BuildTarget.StandaloneWindows64, BuildOptionsClient());
        }

        public static void Server_StandaloneWindows64()
        {
            BuildPipeline.BuildPlayer(ProdScenes, "Windows64-Server/Windows64-Server.exe", BuildTarget.StandaloneWindows64, BuildOptionsServer());
        }

        public static void Client_Linux64()
        {
            BuildPipeline.BuildPlayer(ProdScenes, "Linux64-Client/Linux64-Client.exe", BuildTarget.StandaloneLinux64, BuildOptionsClient());
        }

        public static void Server_Linux64()
        {
            BuildPipeline.BuildPlayer(ProdScenes, "Linux64-Server/Linux64-Server.exe", BuildTarget.StandaloneLinux64, BuildOptionsServer());
        }

        public static void Client_OSX()
        {
            BuildPipeline.BuildPlayer(ProdScenes, "OSX-Client/OSX-Client.exe", BuildTarget.StandaloneOSX, BuildOptionsClient());
        }
    }
}
