using System.Collections;
using System.Collections.Generic;
using Model.OutGame;
using UnityEngine;

namespace Repository.Setting {
    public class LocalRepository : IRepository
    {
        #region Unity Method
        void Awake() {
            bool BGM = true;
            if (PlayerPrefs.HasKey("SettingBGM")) {
                BGM = PlayerPrefs.GetString("SettingBGM") == "True" ? true : false;
            }
            bool VFX = true;
            if (PlayerPrefs.HasKey("SettingVFX")) {
                VFX = PlayerPrefs.GetString("SettingVFX") == "True" ? true : false;
            }
            this.setting = new Entity(BGM, VFX);
        }
        #endregion

        public override CurrentSetting GetCurrentSetting()
        {
            return new CurrentSetting(this.setting.BGM, this.setting.VFX);
        }

        public override void OnBGM()
        {
            this.setting.BGM = true;
            PlayerPrefs.SetString("SettingBGM", this.setting.BGM.ToString());
        }

        public override void OffBGM()
        {
            this.setting.BGM = false;
            PlayerPrefs.SetString("SettingBGM", this.setting.BGM.ToString());
        }

        public override void OnVFX()
        {
            this.setting.VFX = true;
            PlayerPrefs.SetString("SettingVFX", this.setting.VFX.ToString());
        }

        public override void OffVFX()
        {
            this.setting.VFX = false;
            PlayerPrefs.SetString("SettingVFX", this.setting.VFX.ToString());
        }
    }
}
