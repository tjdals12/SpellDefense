using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using LocalizationRepository = Repository.Localization.IRepository;

    public class LocalizationController : MonoBehaviour
    {
        LocalizationRepository localizationRepository;

        #region Unity Method
        void Awake() {
            this.localizationRepository = GameObject.FindObjectOfType<LocalizationRepository>();
        }
        #endregion

        public string GetCurrentLanguage() {
            return this.localizationRepository.GetCurrentLanguage();
        }

        public string GetText(string id) {
            return this.localizationRepository.GetLocalizedText(id);
        }
    }
}
