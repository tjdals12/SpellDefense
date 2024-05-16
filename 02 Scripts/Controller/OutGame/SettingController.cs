using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using SettingService = Service.OutGame.SettingService;

    public class SettingController : MonoBehaviour
    {
        SettingService settingService;

        #region Unity Method
        void Awake() {
            this.settingService = GameObject.FindObjectOfType<SettingService>();
        }
        #endregion

        public void ToggleBGM(bool isOn) {
            this.settingService.ToggleBGM(isOn);
        }

        public void ToggleSFX(bool isOn) {
            this.settingService.ToggleSFX(isOn);
        }

        public void ChangeLanguage(string language) {
            this.settingService.ChangeLanguage(language);
        }
    }
}
