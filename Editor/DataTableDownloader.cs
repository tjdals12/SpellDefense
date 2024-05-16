using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Download;
using Google.Apis.Drive.v3;

namespace MyEditor {
    using Environment = Core.App.Environment;
    using Utils = Core.App.Utils;

    public class DataTableDownloader : EditorWindow
    {
        const string projectName = "PROJECT_NAME";
        const string clientName = "Unity Editor";
        const string clientId = "CLIENT_ID";
        const string clientSecret = "CLIENT_SECRET";

        static Environment currentEnvironment;
        string inputVersion;

        #region Unity Method
        void OnGUI() {
            EditorGUILayout.BeginVertical();

            GUIStyle descriptionLabelStyle = new GUIStyle(GUI.skin.label);
            descriptionLabelStyle.margin = new RectOffset(10, 10, 10, 10);
            descriptionLabelStyle.wordWrap = true;
            GUILayout.Label("1) 구글 드라이브에 있는 파일을 다운로드 합니다.", descriptionLabelStyle);
            GUILayout.Label("2) 다운로드 후 현재 데이터 테이블 버전을 수정하지 않기 때문에 데이터 패치를 무시합니다.", descriptionLabelStyle);
            GUILayout.Label("3) 데이터 패치를 진행하려면 저장된 데이터 테이블 버전을 삭제해 주세요.", descriptionLabelStyle);

            GUIStyle versionLabelStyle = new GUIStyle(GUI.skin.label);
            versionLabelStyle.margin = new RectOffset(10, 10, 30, 10);
            GUILayout.Label("다운로드할 버전을 입력해 주세요.", versionLabelStyle);

            EditorGUILayout.BeginHorizontal();
            GUIStyle textFieldLabelStyle = new GUIStyle(GUI.skin.label);
            textFieldLabelStyle.margin = new RectOffset(10, 10, 10, 10);
            textFieldLabelStyle.fixedHeight = 30f;
            GUILayout.Label("Version: ", textFieldLabelStyle);
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.margin = new RectOffset(10, 10, 10, 5);
            textFieldStyle.fixedWidth = 300f;
            textFieldStyle.fixedHeight = 30f;
            textFieldStyle.padding = new RectOffset(5, 5, 0, 0);
            textFieldStyle.alignment = TextAnchor.MiddleLeft;
            inputVersion = GUILayout.TextField(inputVersion, textFieldStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle textFieldHintLabelStyle = new GUIStyle(GUI.skin.label);
            textFieldHintLabelStyle.margin = new RectOffset(10, 10, 10, 10);
            textFieldHintLabelStyle.fixedWidth = 300f;
            textFieldHintLabelStyle.margin = new RectOffset(10, 10, 0, 10);
            GUILayout.Label("ex) 1.0.0", textFieldHintLabelStyle);
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.margin = new RectOffset(10, 10, 10, 10);
            buttonStyle.fixedWidth = 200f;
            buttonStyle.fixedHeight = 30f;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("다운로드", buttonStyle) == true && String.IsNullOrEmpty(inputVersion) == false) {
                this.Download(currentEnvironment, inputVersion);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        #endregion

        static DriveService GetDriveService() {
            ClientSecrets clientSecrets = new ClientSecrets() {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
            string[] scopes = new string[] {
                DriveService.Scope.DriveReadonly
            };
            UserCredential userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets: clientSecrets,
                scopes: scopes,
                user: clientName,
                CancellationToken.None
            ).Result;
            DriveService driveService = new DriveService(
                new BaseClientService.Initializer() {
                    HttpClientInitializer = userCredential,
                    ApplicationName = projectName
                }
            );
            return driveService;
        }

        static string GetFolderId(DriveService driveService, string targetFolderName, string parentFolderId = null) {
            var folderListRequest = driveService.Files.List();
            if (parentFolderId == null) {
                folderListRequest.Q = $"mimeType='application/vnd.google-apps.folder' and trashed = false";
            } else {
                folderListRequest.Q = $"mimeType='application/vnd.google-apps.folder' and trashed = false and '{parentFolderId}' in parents";
            }
            folderListRequest.Fields = "nextPageToken, files(id, name, size, mimeType)";
            var folderListResult = folderListRequest.Execute();
            string folderId = null;
            targetFolderName = targetFolderName.ToLower();
            foreach (var folder in folderListResult.Files) {
                if (folder.Name.ToLower() == targetFolderName) {
                    folderId = folder.Id;
                    break;
                }
            }
            return folderId;
        }

        public void Download(Environment environment, string version) {
            try {
                if (Utils.CheckIsValidVersion(version) == false)
                    throw new Exception($"버전 형식이 올바르지 않습니다. ({version})");

                EditorUtility.DisplayProgressBar("잠시만 기다려주세요...", "파일을 찾고 있습니다.", 0);

                DriveService driveService = GetDriveService();

                string rootFolderName = environment == Environment.Production ? "prod" : "dev";
                string rootFolderId = GetFolderId(
                    driveService: driveService,
                    targetFolderName: rootFolderName,
                    parentFolderId: "FOLDER_ID"
                );
                if (rootFolderId == null)
                    throw new Exception($"파일을 찾을 수 없습니다. [Spell War/{rootFolderName}] 폴더가 있는지 확인해 주세요.");

                string dataTableFolderId = GetFolderId(
                    driveService: driveService,
                    targetFolderName: "DataTable",
                    parentFolderId: rootFolderId
                );
                if (dataTableFolderId == null)
                    throw new Exception($"파일을 찾을 수 없습니다. [Spell War/{rootFolderName}/DataTable] 폴더가 있는지 확인해 주세요.");

                string versionFolderId = GetFolderId(
                    driveService: driveService,
                    targetFolderName: version,
                    parentFolderId: dataTableFolderId
                );
                if (versionFolderId == null)
                    throw new Exception($"파일을 찾을 수 없습니다. [Spell War/{rootFolderName}/DataTable/{version}]");

                EditorUtility.DisplayProgressBar("잠시만 기다려주세요...", "다운로드를 시작합니다.", 0);

                var fileListRequest = driveService.Files.List();
                fileListRequest.Q = $"mimeType!='application/vnd.google-apps.folder' and trashed = false and '{versionFolderId}' in parents";
                fileListRequest.Fields = "nextPageToken, files(id, name, size, mimeType)";
                string nextPageToken = null;
                do {
                    var fileListResult = fileListRequest.Execute();
                    string directoryPath = Path.Combine(new string[] { Application.dataPath, "Resources", "DataTable" });
                    for (int i = 0; i < fileListResult.Files.Count; i++) {
                        var file = fileListResult.Files[i];
                        EditorUtility.DisplayProgressBar("잠시만 기다려주세요...", $"다운로드 중 ({file.Name})", (i + 1) / (fileListResult.Files.Count * 1f));
                        if (Directory.Exists(directoryPath) == false) {
                            Directory.CreateDirectory(directoryPath);
                        }
                        string filePath = Path.Join(directoryPath, file.Name);
                        var fileRequest = driveService.Files.Get(file.Id);
                        var stream = new MemoryStream();
                        fileRequest.MediaDownloader.ProgressChanged += (progress) => {
                            if (progress.Status == DownloadStatus.Completed) {
                                Debug.Log($"Download Complete: {file.Name}");
                            } else if (progress.Status == DownloadStatus.Failed) {
                                Debug.LogError($"Download Fail: {file.Name}");
                            }
                        };
                        fileRequest.Download(stream);
                        File.Create(filePath).Write(stream.GetBuffer());
                    }
                    fileListRequest.PageToken = fileListResult.NextPageToken;
                } while (nextPageToken != null);

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                CompilationPipeline.RequestScriptCompilation();
                DataTableDownloader window = EditorWindow.GetWindow<DataTableDownloader>();
                window.Close();
                EditorUtility.DisplayDialog("완료", "다운로드를 완료하였습니다. Resources/DataTable에서 파일을 확인해 주세요.", "닫기");
            } catch (Exception e) {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("오류", e.Message, "닫기");
            }
        }

        [MenuItem("CustomMenu/DataTable/Download (Development)")]
        public static void DownloadDevelopment() {
            currentEnvironment = Environment.Development;
            DataTableDownloader window = EditorWindow.GetWindowWithRect<DataTableDownloader>(new Rect(0, 0, 400, 300), utility: true, title: "데이터 테이블 다운로드 (Development)");
            window.Show();
        }

        [MenuItem("CustomMenu/DataTable/Download (Production)")]
        public static void DownloadProduction() {
            currentEnvironment = Environment.Production;
            DataTableDownloader window = EditorWindow.GetWindowWithRect<DataTableDownloader>(new Rect(0, 0, 400, 300), utility: true, title: "데이터 테이블 다운로드 (Production)");
            window.Show();
        }
    }
}
