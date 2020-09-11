// MIT License
//
// Copyright (c) 2019-present Webber Takken <webber@takken.io>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using UnityBuilderAction.Input;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace EditorNamespace
{
    static class Builder
    {
        public static void BuildProject()
        {
            // Gather values from args
            var options = ArgumentsParser.GetValidatedOptions();

            // Gather values from project
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(s => s.path).ToArray();

            BuildOptions selectedOptions = BuildOptions.None;
            if (options.ContainsKey("buildType") && options["buildType"].Equals("server", StringComparison.OrdinalIgnoreCase))
            {
                selectedOptions |= BuildOptions.EnableHeadlessMode;
                Console.WriteLine("Creating Server Build");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "UNITY_SERVER");
            }
            else
            {
                Console.WriteLine("Creating Client Build");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");
            }
            if (options.ContainsKey("development"))
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.allowDebugging = true;
                EditorUserBuildSettings.connectProfiler = true;

                selectedOptions |= BuildOptions.Development |
                    BuildOptions.AllowDebugging |
                    BuildOptions.ConnectWithProfiler |
                    BuildOptions.EnableHeadlessMode;
            }

            // Define BuildPlayer Options
            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = options["customBuildPath"],
                target = (BuildTarget)Enum.Parse(typeof(BuildTarget), options["buildTarget"]),
                options = selectedOptions,
            };

            // Perform build
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildOptions);
        }
    }
}