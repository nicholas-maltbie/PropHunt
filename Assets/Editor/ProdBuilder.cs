using System.IO;
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

        public static void Client_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows-Client/Windows-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Server_Windows()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows-Server/Windows-Server.exe";
            playerOptions.target = BuildTarget.StandaloneWindows;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Client_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows64-Client/Windows64-Client.exe";
            playerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Server_Windows64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Windows64-Server/Windows64-Server.exe";
            playerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Client_Linux64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsClient();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Linux64-Client/Linux64-Client.exe";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Server_Linux64()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/Linux64-Server/Linux64-Server.exe";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);
        }

        public static void Client_OSX()
        {
            BuildOptions options = BuildOptionsClient();
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = options;
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/OSX-Client/OSX-Client.exe";
            playerOptions.target = BuildTarget.StandaloneLinux64;
            BuildPipeline.BuildPlayer(playerOptions);

            // move contents of Builds.app folder
            string sourceFolder = "Builds.app";
            string destFolder = "Builds";
            foreach (string filePath in Directory.GetFiles(sourceFolder))
            {
                File.Copy(Path.Combine(sourceFolder, filePath), Path.Combine(destFolder, filePath), true);
            }
        }

        public static void Server_OSX()
        {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.options = BuildOptionsServer();
            playerOptions.scenes = ProdScenes;
            playerOptions.locationPathName = "Builds/OSX-Server/OSX-Server.exe";
            playerOptions.target = BuildTarget.StandaloneOSX;
            BuildPipeline.BuildPlayer(playerOptions);

            // move contents of Builds.app folder
            string sourceFolder = "Builds.app";
            string destFolder = "Builds";
            foreach (string filePath in Directory.GetFiles(sourceFolder))
            {
                File.Copy(Path.Combine(sourceFolder, filePath), Path.Combine(destFolder, filePath), true);
            }
        }
    }
}
