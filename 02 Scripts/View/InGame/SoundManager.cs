using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace View.InGame {
    using SettingModel = Model.OutGame.SettingModel;

    public class SoundManager : MonoBehaviour
    {
        SettingModel settingModel;

        [Header("BGM")]
        [Space(4)]
        [SerializeField]
        AudioSource bgmAudioSource;
        [SerializeField]
        AudioSource sfxAudioSource;
        [SerializeField]
        AudioClip startWaveSfx;
        [SerializeField]
        AudioClip clickSfx;
        [SerializeField]
        AudioClip spawnSfx;
        [SerializeField]
        AudioClip upgradeSfx;
        [SerializeField]
        AudioClip[] enemyHitSfxList;
        [SerializeField]
        AudioClip[] playerHitSfxList;
        [SerializeField]
        AudioClip castSfx;
        [SerializeField]
        AudioClip gameOverSfx;

        #region Unity Method
        void Awake() {
            this.settingModel = GameObject.FindObjectOfType<SettingModel>();

            this.bgmAudioSource.mute = !this.settingModel.currentSetting.BGM;
            this.bgmAudioSource.playOnAwake = true;
            this.bgmAudioSource.loop = true;
        }
        #endregion

        public void StartWave() {
            this.sfxAudioSource.PlayOneShot(this.startWaveSfx);
        }

        public void Click() {
            this.sfxAudioSource.PlayOneShot(this.clickSfx);
        }

        public void Spawn() {
            this.sfxAudioSource.PlayOneShot(this.spawnSfx);
        }

        public void Upgrade() {
            this.sfxAudioSource.PlayOneShot(this.upgradeSfx);
        }

        public void HitEnenmy() {
            int index = Random.Range(0, this.enemyHitSfxList.Length);
            AudioClip hitSfx = this.enemyHitSfxList[index];
            this.sfxAudioSource.PlayOneShot(hitSfx);
        }

        public void HitPlayer() {
            int index = Random.Range(0, this.playerHitSfxList.Length);
            AudioClip hitSfx = this.playerHitSfxList[index];
            this.sfxAudioSource.PlayOneShot(hitSfx);
        }

        public void Cast() {
            this.sfxAudioSource.PlayOneShot(this.castSfx);
        }

        public void FadeOutBgm() {
            this.bgmAudioSource.DOFade(0, 0.5f).From(1);
        }

        public void GameOver() {
            this.sfxAudioSource.PlayOneShot(this.gameOverSfx);
        }
    }
}
