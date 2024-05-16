using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace MyEditor {
    public class FirebaseProjectSwitcher : MonoBehaviour
    {
        const string developmentJsonFile = "google-services-dev.json";
        const string productionJsonFile = "google-services-prod.json";
        const string desktopJsonFile = "StreamingAssets/google-services-desktop.json";
        const string projectJsonFile = "google-services.json";

        [MenuItem("CustomMenu/Firebase/Switch To Development")]
        public static void SwitchToDevelopment() {
            try {
                bool isConfirm = Confirm();
                if (isConfirm == false) return;
                CopyFile(developmentJsonFile, desktopJsonFile);
                CopyFile(developmentJsonFile, projectJsonFile);
                EditorUtility.DisplayDialog("Success", "Switched to development", "Close");
                AssetDatabase.Refresh();
            } catch (Exception e) {
                EditorUtility.DisplayDialog("Error", e.Message, "Close");
            }
        }

        [MenuItem("CustomMenu/Firebase/Switch To Production")]
        public static void SwitchToProduction() {
            try {
                bool isConfirm = Confirm();
                if (isConfirm == false) return;
                CopyFile(productionJsonFile, desktopJsonFile);
                CopyFile(productionJsonFile, projectJsonFile);
                EditorUtility.DisplayDialog("Success", "Switched to production", "Close");
                AssetDatabase.Refresh();
            } catch (Exception e) {
                EditorUtility.DisplayDialog("Error", e.Message, "Close");
            }
        }

        static bool Confirm() {
            return EditorUtility.DisplayDialog(
                "Warning!", "This action switches only the projects to be accessed from Editor. This does not apply to builds.",
                "Confirm",
                "Cancel"
            );
        }

        static void CopyFile(string readPath, string writePath) {
            string directoryPath = Application.dataPath;
            string from = Path.Combine(new string[] { directoryPath, readPath });
            if (File.Exists(from) == false) throw new Exception($"[Assets/{readPath}] does not exists.");
            string to = Path.Combine(new string[] { directoryPath, writePath });
            File.Copy(from, to, overwrite: true);
        }
    }
}
