using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace View.OutGame {
    using SettingModel = Model.OutGame.SettingModel;

    public class SoundManager : MonoBehaviour
    {
        SettingModel settingModel;

        [Header("BGM")]
        [Space(4)]
        [SerializeField]
        AudioSource bgmAudioSource;

        [Header("SFX")]
        [Space(4)]
        [SerializeField]
        AudioSource sfxAudioSource;
        [SerializeField]
        AudioClip clickSfx;
        [SerializeField]
        AudioClip closeSfx;

        #region Unity Method
        void Awake() {
            this.settingModel = GameObject.FindObjectOfType<SettingModel>();

            this.bgmAudioSource.mute = !this.settingModel.currentSetting.BGM;
            this.bgmAudioSource.playOnAwake = true;
            this.bgmAudioSource.loop = true;

            this.sfxAudioSource.mute = !this.settingModel.currentSetting.SFX;
            this.sfxAudioSource.playOnAwake = false;
            this.sfxAudioSource.loop = false;
        }
        void OnEnable() {
            this.settingModel.OnToggleBGM += this.OnToggleBGM;
            this.settingModel.OnToggleSFX += this.OnToggleSFX;
        }
        void OnDisable() {
            this.settingModel.OnToggleBGM -= this.OnToggleBGM;
            this.settingModel.OnToggleSFX -= this.OnToggleSFX;
        }
        #endregion
        
        #region Event Listeners
        void OnToggleBGM() {
            this.bgmAudioSource.mute = !this.settingModel.currentSetting.BGM;
        }
        void OnToggleSFX() {
            this.sfxAudioSource.mute = !this.settingModel.currentSetting.SFX;
        }
        #endregion

        public void Click() {
            this.sfxAudioSource.clip = this.clickSfx;
            this.sfxAudioSource.Play();
        }

        public void Close() {
            this.sfxAudioSource.clip = this.closeSfx;
            this.sfxAudioSource.Play();
        }

        public void FadeOutBgm() {
            this.bgmAudioSource.DOFade(0, 1f).From(1);
        }
    }
}