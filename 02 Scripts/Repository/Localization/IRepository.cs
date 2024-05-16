using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Localization {
    public abstract class IRepository : MonoBehaviour
    {
        public bool isLoaded { get; protected set; }
        protected Dictionary<string, Dictionary<string, string>> localizedTexts;
        public abstract string GetCurrentLanguage();
        public abstract void ChangeCurrentLanguage(string language);
        public abstract string GetLocalizedText(string id);
    }
}
