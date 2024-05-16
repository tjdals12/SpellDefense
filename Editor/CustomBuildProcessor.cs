using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace MyEditor {
    public class CustomBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            string fromFirebaseJsonFile = "google-services-dev.json";
            if (Application.version.EndsWith("prod")) {
                fromFirebaseJsonFile = "google-services-prod.json";
            } 
            string toFirebaseJsonFile = "google-services.json";
            string directoryPath = Application.dataPath;
            string from = Path.Combine(new string[] { directoryPath, fromFirebaseJsonFile });
            if (File.Exists(from) == false) throw new Exception($"[Assets/{fromFirebaseJsonFile}] does not exists.");
            string to = Path.Combine(new string[] { directoryPath, toFirebaseJsonFile });
            File.Copy(from, to, overwrite: true);
        }
    }
}
