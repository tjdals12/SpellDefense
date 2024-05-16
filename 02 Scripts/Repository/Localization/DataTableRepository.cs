using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Repository.Localization {
    public class DataTableRepository : IRepository
    {
        string currentLanguage;
        string[] allowLanguages;

        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IRepository>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            this.localizedTexts = this.Load();
            this.isLoaded = true;
        }
        #endregion

        Dictionary<string, Dictionary<string, string>> Load() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/Localization");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "Localization.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            this.allowLanguages = Array.FindAll(names, (e) => e != "id");
            Dictionary<string, Dictionary<string, string>> items = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] values = rows[i].Split(",");
                List<string> columns = new();
                foreach (string value in values) {
                    string column = value;
                    if (column.StartsWith("\"") == false && column.EndsWith("\"")) {
                        string prevColumn = columns[columns.Count - 1];
                        columns[columns.Count - 1] = new Regex("^\"|\"$").Replace(prevColumn + "," + column, "").Replace("\"\"", "\"");
                    } else {
                        columns.Add(new Regex("^\"|\"$").Replace(column, "").Replace("\"\"", "\""));
                    }
                }
                if (columns.Count != names.Length) continue;
                string id = null;
                Dictionary<string, string> localizedText = new();
                for (int j = 0; j < columns.Count; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        id = value;
                    } else {
                        localizedText[name] = value;
                    }
                }
                items[id] = localizedText;
            }
            if (PlayerPrefs.HasKey("SettingLanguage")) {
                this.currentLanguage = PlayerPrefs.GetString("SettingLanguage");
            } else {
                if (Application.systemLanguage == SystemLanguage.Korean) {
                    this.currentLanguage = "ko";
                } else {
                    this.currentLanguage = "en";
                }
            }
            bool isAllowedLanguage = Array.Exists(this.allowLanguages, (e) => e == currentLanguage);
            if (isAllowedLanguage == false) {
                this.currentLanguage = "en";
            }
            return items;
        }

        public override string GetCurrentLanguage()
        {
            return this.currentLanguage;
        }

        public override void ChangeCurrentLanguage(string language)
        {
            this.currentLanguage = language;
            PlayerPrefs.SetString("SettingLanguage", language);
        }

        public override string GetLocalizedText(string id)
        {
            string localizedText = id;
            if (this.localizedTexts.ContainsKey(id)) {
                if (this.localizedTexts[id].ContainsKey(this.currentLanguage)) {
                    localizedText = this.localizedTexts[id][this.currentLanguage];
                }
            }
            return localizedText;
        }
    }
}
