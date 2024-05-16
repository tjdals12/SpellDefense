using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.Battle {
    using UserModel = Model.OutGame.UserModel;
    using UserController = Controller.OutGame.UserController;
    using LoadingView = Loading.LoadingView;
    using LoadingPopup = Common.LoadingPopup;

    public class GameStartButton : MonoBehaviour
    {
        UserModel userModel;
        UserController userController;

        [Header("Audio")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button gameStartButton;
        [SerializeField]
        GameObject gameStartPopup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        RectTransform usedEnergy;
        [SerializeField]
        CanvasGroup usedEnergyCanvasGroup;

        LoadingView loadingView;
        LoadingPopup loadingPopup;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.loadingView = GameObject.FindObjectOfType<LoadingView>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.gameStartButton.onClick.AddListener(this.GameStart);
        }
        void OnEnable() {
            this.userModel.OnChangeEnergy += this.OnChangeEnergy;
            this.userModel.OnGameStart += this.OnGameStart;
        }
        void OnDisable() {
            this.userModel.OnChangeEnergy -= this.OnChangeEnergy;
            this.userModel.OnGameStart -= this.OnGameStart;
        }
        #endregion

        #region Event Listeners
        void OnChangeEnergy() {
            if (this.userModel.user.energy.amount >= 5) {
                this.gameStartButton.enabled = true;
            } else {
                this.gameStartButton.enabled = false;
            }
        }
        void OnGameStart() {
            this.gameStartPopup.SetActive(true);
            this.soundManager.FadeOutBgm();
            this.backgroundImage.DOFade(1, 0.5f).From(0);
            DOTween.Sequence()
                .Join(this.usedEnergy.DOAnchorPosY(200, 0.5f).From(Vector3.zero, isRelative: true).SetEase(Ease.OutCirc))
                .Join(this.usedEnergyCanvasGroup.DOFade(0, 0.5f).From(1).SetEase(Ease.InExpo))
                .OnComplete(() => {
                    LoadingView.LoadScene("InGameScene");
                });
        }
        #endregion

        void GameStart() {
            this.soundManager.Click();
            this.gameStartButton.enabled = false;
            this.loadingPopup.Open();
            this.userController.GameStart();
        }
    }
}
