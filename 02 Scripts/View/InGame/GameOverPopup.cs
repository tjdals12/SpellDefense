using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace View.InGame {
    using LoadingView = Loading.LoadingView;
    using RewardedAdView = Core.Ads.Admob.RewardedAdView;
    using PlayModel = Model.InGame.PlayModel;
    using InGameController = Controller.InGame.InGameController;
    using Common;

    public class GameOverPopup : MonoBehaviour
    {
        PlayModel playModel;
        InGameController inGameController;
        RewardedAdView rewardedAdView;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        GameObject window;
        [SerializeField]
        TextMeshProUGUI waveText;
        [SerializeField]
        LocalizationText waveLocalizationText;
        [SerializeField]
        TextMeshProUGUI rewardGoldText;
        [SerializeField]
        TextMeshProUGUI rewardSilverKeyText;
        [SerializeField]
        TextMeshProUGUI playTimeText;
        [SerializeField]
        TextMeshProUGUI depeatedEnemyText;
        [SerializeField]
        Button lobbyButton;
        [SerializeField]
        ToggleButton adsButton;

        [Header("Effect")]
        [Space(4)]
        [SerializeField]
        GameObject goldAttractionPrefab;
        [SerializeField]
        GameObject keyAttractionPrefab;

        LoadingPopup loadingPopup;

        #region Unity Method
        void Awake() {
            this.playModel = GameObject.FindObjectOfType<PlayModel>();
            this.inGameController = GameObject.FindObjectOfType<InGameController>();
            this.rewardedAdView = GameObject.FindObjectOfType<RewardedAdView>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.lobbyButton.onClick.AddListener(this.GoToLobby);
            this.adsButton.Initialize(onClick: this.WatchAds);
        }
        void OnEnable() {
            this.playModel.OnWatchAds += this.OnWatchAds;
        }
        void OnDisable() {
            this.playModel.OnWatchAds -= this.OnWatchAds;
        }
        #endregion

        #region Event Listeners
        void OnWatchAds() {
            this.adsButton.Disable();
            this.loadingPopup.Close();
            GameObject goldAttraction = Instantiate(this.goldAttractionPrefab, this.popup.transform.parent);
            Destroy(goldAttraction, 5);
            StartCoroutine(this.IncreaseRewardGold());
            GameObject keyAttraction = Instantiate(this.keyAttractionPrefab, this.popup.transform.parent);
            Destroy(keyAttraction, 5);
            StartCoroutine(this.IncreaseRewardSilverKey());
        }
        #endregion


        public void Open(int wave, int rewardGold, int rewardSilverKey, int playSeconds, int depeatedEnemy) {
            this.waveLocalizationText.UpdateView(wave);
            this.rewardGoldText.text = rewardGold.ToString();
            this.rewardSilverKeyText.text = rewardSilverKey.ToString();
            string minutes = (playSeconds / 60).ToString();
            string seconds = (playSeconds % 60).ToString().PadLeft(2, '0');
            this.playTimeText.text = $"{minutes}:{seconds}";
            this.depeatedEnemyText.text = depeatedEnemy.ToString();
            this.popup.SetActive(true);
            DOTween.Sequence()
                .OnStart(() => {
                    this.soundManager.FadeOutBgm();
                })
                .Join(this.backgroundImage.DOFade(0.99f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .Append(this.waveText.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .Append(this.rewardGoldText.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .Append(this.rewardSilverKeyText.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .Append(this.playTimeText.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .Append(this.depeatedEnemyText.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .OnComplete(() => {
                    this.soundManager.GameOver();
                });
        }

        void GoToLobby() {
            this.soundManager.Click();
            LoadingView.LoadScene("OutGameScene");
        }

        void WatchAds() {
            this.rewardedAdView.Show(onReward: () => {
                this.loadingPopup.Open();
                this.inGameController.WatchAds();
            });
        }

        IEnumerator IncreaseRewardGold() {
            yield return new WaitForSeconds(1f);
            int beforeGold = this.playModel.rewardGold;
            int afterGold = beforeGold * 2;
            int currentGold = beforeGold;
            float elapsedTime = 0f;
            float duration = 50 >= beforeGold ? 0.5f : 1f;
            while (duration > elapsedTime) {
                elapsedTime += Time.deltaTime;
                currentGold = Mathf.FloorToInt(Mathf.Lerp(beforeGold, afterGold, elapsedTime / duration));
                this.rewardGoldText.text = currentGold.ToString();
                yield return null;
            }
        }

        IEnumerator IncreaseRewardSilverKey() {
            yield return new WaitForSeconds(1f);
            int beforeSilverKey = this.playModel.rewardSilverKey;
            int afterSilverKey = beforeSilverKey * 2;
            int currentSilverKey = beforeSilverKey;
            float elapsedTime = 0f;
            float duration = 50 >= beforeSilverKey ? 0.5f : 1f;
            while (duration > elapsedTime) {
                elapsedTime += Time.deltaTime;
                currentSilverKey = Mathf.FloorToInt(Mathf.Lerp(beforeSilverKey, afterSilverKey, elapsedTime / duration));
                this.rewardSilverKeyText.text = currentSilverKey.ToString();
                yield return null;
            }
        }
    }
}