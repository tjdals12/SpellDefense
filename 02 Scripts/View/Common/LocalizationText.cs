using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace View.Common {
    using LocalizationController = Controller.OutGame.LocalizationController;

    public class LocalizationText : MonoBehaviour
    {
        [Header("Controller")]
        [Space(4)]
        [SerializeField]
        LocalizationController localizationController;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        TextMeshProUGUI text;

        Regex regex;

        bool isInitialized;

        string format;
        object[] values;

        #region Unity Method
        void Awake() {
            if (this.localizationController == null) {
                this.localizationController = GameObject.FindObjectOfType<LocalizationController>();
            }
            this.regex = new Regex("(?<={)[A-Z_]+(?=})");
        }
        void Start() {
            string txt = this.text.text;
            MatchCollection mc = regex.Matches(txt);
            Dictionary<string, string> dict = new();
            foreach (Match m in mc) {
                string id = m.Value;
                string localizedText = this.localizationController.GetText(id);
                txt = txt.Replace("{" + id + "}", localizedText);
            }
            this.text.text = txt;
            this.format = txt;
            this.isInitialized = true;
            this.UpdateView();
        }
        #endregion

        public void UpdateView(object value) {
            this.values = new object[] { value };
            if (this.isInitialized) {
                this.UpdateView();
            }
        }

        public void UpdateView(object[] values) {
            this.values = values;
            if (this.isInitialized) {
                this.UpdateView();
            }
        }

        void UpdateView() {
            if (this.values == null || this.values.Length == 0) return;
            string txt = this.format;
            Regex regex = new Regex("(?<={)[0-9]+(?=})");
            MatchCollection mc = regex.Matches(txt);
            Dictionary<string, string> dict = new();
            int index = 0;
            foreach (Match m in mc) {
                if (index >= values.Length) break;
                string id = m.Value;
                txt = txt.Replace("{" + id + "}", values[index++].ToString());
            }
            this.text.text = txt;
        }
    }
}
