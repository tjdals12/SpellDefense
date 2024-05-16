using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Core.App {
    public class DataTablePatcher : MonoBehaviour
    {
        string dataTableVersionKey = "DataTableVersion";
        StorageClient googleStorage;
        string bucketName;

        #region Unity Method
        void Awake() {
            TextAsset keyFile = Resources.Load<TextAsset>(
                Utils.GetCurrentEnv() == Environment.Production
                    ? "unity-client-service-account-prod"
                    : "unity-client-service-account-dev"
            );
            GoogleCredential credential = GoogleCredential.FromJson(keyFile.text);
            this.googleStorage = StorageClient.Create(credential);
            this.bucketName = Utils.GetCurrentEnv() == Environment.Production
                ? "datatable-bucket-prod"
                : "datatable-bucket-dev";
        }
        #endregion

        public string GetCurrentVersion() {
            string dataTableVersion = PlayerPrefs.GetString(key: this.dataTableVersionKey, defaultValue: null);
            string version = Utils.GetVersion(dataTableVersion);
            return version;
        }

        void SetCurrentVersion(string version)  {
            PlayerPrefs.SetString(key: this.dataTableVersionKey, value: version);
        }

        public bool CheckDownload(VersionInfo versionInfo, out string defaultVersion) {
            string currentDataTableVersion = this.GetCurrentVersion();
            if (currentDataTableVersion == null) {
                defaultVersion = versionInfo.defaultDataTableversion;
                return true;
            }
            defaultVersion = null;
            return false;
        }

        public bool CheckUpdate(VersionInfo versionInfo, out string version) {
            string currentAppVersion = Utils.GetAppVersion();
            string currentDataTableVersion = this.GetCurrentVersion();

            string latestAppVersion = Utils.GetVersion(versionInfo.latestAppVersion);
            string latestDataTableVersion = Utils.GetVersion(versionInfo.latestDataTableVersion);
            if (currentAppVersion == latestAppVersion && currentDataTableVersion != latestDataTableVersion) {
                version = latestDataTableVersion;
                return true;
            }

            string requiredAppVersion = Utils.GetVersion(versionInfo.requiredAppVersion);
            string requiredDataTableVersion = Utils.GetVersion(versionInfo.requiredDataTableVersion);
            if (currentAppVersion == requiredAppVersion && currentDataTableVersion != requiredDataTableVersion) {
                version = requiredDataTableVersion;
                return true;
            }

            string defaultDataTableversion = Utils.GetVersion(versionInfo.defaultDataTableversion);
            if (currentDataTableVersion == null) {
                version = defaultDataTableversion;
                return true;
            }

            version = null;
            return false;
        }

        public IEnumerator Patch(string version, Action onStart, Action<int, int> onStepComplete, Action onComplete) {
            yield return null;
            #if UNITY_EDITOR
                string dirPath = Path.Combine(new string[] { Application.dataPath, "Resources", "DataTable" });
            #else
                string dirPath = Path.Combine(new string[] { Application.persistentDataPath, "DataTable" });
            #endif
            if (Directory.Exists(dirPath) == false) {
                Directory.CreateDirectory(dirPath);
            }
            var files = this.googleStorage.ListObjects(this.bucketName, prefix: version, options: new ListObjectsOptions() { PageSize = 50 });
            int currentCount = 0;
            int totalCount = files.Count();
            onStart?.Invoke();
            yield return null;
            foreach (var file in files) {
                string fileName = file.Name.Split("/").Last();
                string filePath = Path.Combine(new string[] { dirPath, fileName });
                using (var stream = File.Open(filePath, FileMode.OpenOrCreate)) {
                    this.googleStorage.DownloadObject(this.bucketName, file.Name, stream);
                }
                currentCount++;
                onStepComplete?.Invoke(currentCount, totalCount);
                yield return null;
            }
            #if UNITY_EDITOR
                AssetDatabase.Refresh();
            #endif
            onComplete?.Invoke();
            yield return null;
            this.SetCurrentVersion(version);
        }

    }
}
