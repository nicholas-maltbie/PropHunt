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
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace UnityBuilderAction.Reporting
{
  public class StdOutReporter
  {
    static string EOL = Environment.NewLine;

    public static void ReportSummary(BuildSummary summary)
    {
      Console.WriteLine(
        $"{EOL}" +
        $"###########################{EOL}" +
        $"#      Build results      #{EOL}" +
        $"###########################{EOL}" +
        $"{EOL}" +
        $"Duration: {summary.totalTime.ToString()}{EOL}" +
        $"Warnings: {summary.totalWarnings.ToString()}{EOL}" +
        $"Errors: {summary.totalErrors.ToString()}{EOL}" +
        $"Size: {summary.totalSize.ToString()} bytes{EOL}" +
        $"{EOL}"
      );
    }

    public static void ExitWithResult(BuildResult result)
    {
      if (result == BuildResult.Succeeded) {
        Console.WriteLine("Build succeeded!");
        EditorApplication.Exit(0);
      }

      if (result == BuildResult.Failed) {
        Console.WriteLine("Build failed!");
        EditorApplication.Exit(101);
      }

      if (result == BuildResult.Cancelled) {
        Console.WriteLine("Build cancelled!");
        EditorApplication.Exit(102);
      }

      if (result == BuildResult.Unknown) {
        Console.WriteLine("Build result is unknown!");
        EditorApplication.Exit(103);
      }
    }
  }
}
