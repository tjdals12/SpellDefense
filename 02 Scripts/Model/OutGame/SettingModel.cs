using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.OutGame {
    public abstract class ISettingModel : MonoBehaviour {
        protected CurrentSetting _currentSetting;
        public ICurrentSetting currentSetting { get => this._currentSetting; }
        protected string _currentLanguage;
        public string currentLanguage { get => this._currentLanguage; }

        public Action OnToggleBGM;
        public Action OnToggleSFX;
        public Action OnChangeLanguage;
    }

    public class SettingModel : ISettingModel {
        public void Initialize(CurrentSetting currentSetting, string currentLanguage) {
            this._currentSetting = currentSetting;
            this._currentLanguage = currentLanguage;
        }
        public void ToggleBGM(bool isOn) {
            this._currentSetting.ToggleBGM(isOn);
            this.OnToggleBGM?.Invoke();
        }
        public void ToggleSFX(bool isOn) {
            this._currentSetting.ToggleVFX(isOn);
            this.OnToggleSFX?.Invoke();
        }
        public void ChangeLanguage(string language) {
            this._currentLanguage = language;
            this.OnChangeLanguage?.Invoke();
        }
    }
}
