using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Service.OutGame {
    using ISettingRepository = Repository.Setting.IRepository;
    using ILocalizationRepository = Repository.Localization.IRepository;
    using Model.OutGame;

    public class SettingService : MonoBehaviour
    {
        ISettingRepository settingRepository;
        ILocalizationRepository localizationRepository;
        SettingModel settingModel;

        #region Unity Method
        void Awake() {
            this.settingRepository = GameObject.FindObjectOfType<ISettingRepository>();
            this.localizationRepository = GameObject.FindObjectOfType<ILocalizationRepository>();
            this.settingModel = GameObject.FindObjectOfType<SettingModel>();
        }
        #endregion

        public void ToggleBGM(bool isOn) {
            if (isOn) {
                this.settingRepository.OnBGM();
            } else {
                this.settingRepository.OffBGM();
            }
            this.settingModel.ToggleBGM(isOn);
        }

        public void ToggleSFX(bool isOn) {
            if (isOn) {
                this.settingRepository.OnVFX();
            } else {
                this.settingRepository.OffVFX();
            }
            this.settingModel.ToggleSFX(isOn);
        }

        public void ChangeLanguage(string language) {
            this.localizationRepository.ChangeCurrentLanguage(language);
            this.settingModel.ChangeLanguage(language);
        }
    }
}
