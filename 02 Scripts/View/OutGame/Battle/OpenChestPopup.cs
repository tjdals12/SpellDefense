using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Battle {
    using ChestController = Controller.OutGame.ChestController;
    using UserController = Controller.OutGame.UserController;
    using UserModel = Model.OutGame.IUserModel;
    using Model.OutGame;
    using Model.Common;
    using ItemType = Repository.Item.ItemType;
    using ToggleButton = Common.ToggleButton;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;
    using RewardPopup = Common.RewardPopup;

    public class OpenChestPopup : MonoBehaviour
    {
        ChestController chestController;
        UserController userController;
        UserModel userModel;
        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        [Header("Audio")]
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
        GameObject chestItemsPanel;
        [SerializeField]
        Button increaseButton;
        [SerializeField]
        Button decreaseButton;
        [SerializeField]
        Button maxButton;
        [SerializeField]
        TextMeshProUGUI keyCountText;
        [SerializeField]
        TextMeshProUGUI chestCountText;
        [SerializeField]
        Image sliderBarImage;
        [SerializeField]
        TextMeshProUGUI sliderBarText;
        [SerializeField]
        ToggleButton openButton;
        [SerializeField]
        Button closeButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        ChestOpeningPopup chestOpeningPopup;
        [SerializeField]
        RewardPopup rewardPopup;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject chestItemInfoPrefab;

        Chest chest;
        readonly int onceMaxOpenCount = 10;
        int currentOpenCount = 0;

        #region Unity Method
        void Awake() {
            this.chestController = GameObject.FindObjectOfType<ChestController>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.increaseButton.onClick.AddListener(this.Increase);
            this.decreaseButton.onClick.AddListener(this.Decrease);
            this.maxButton.onClick.AddListener(this.IncreaseToMax);
            this.openButton.Initialize(onClick: this.OpenChest);
            this.closeButton.onClick.AddListener(this.Close);
            Chest chest = this.chestController.GetSilverChest();
            foreach (var chestItem in chest.chestItems) {
                GameObject clone = Instantiate(this.chestItemInfoPrefab, this.chestItemsPanel.transform);
                ChestItemInfo chestItemInfo = clone.GetComponent<ChestItemInfo>();
                chestItemInfo.Initialize(chestItem);
            }
            this.chest = chest;
            this.UpdateView();
        }
        void OnEnable() {
            this.userModel.OnOpenChest += this.OnOpenChest;
        }
        void OnDisable() {
            this.userModel.OnOpenChest -= this.OnOpenChest;
        }
        #endregion

        #region Event Listeners
        void OnOpenChest(Chest chest, int count, List<RewardItem> rewardItems) {
            this.chestOpeningPopup.Open(
                chest: chest,
                count: count,
                onClick: () => {
                    this.rewardPopup.Open(rewardItems);
                }
            );
            this.Close();
        }
        #endregion

        public void Open() {
            this.openButton.Disable();
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        public void Close() {
            this.soundManager.Close();
            this.popup.SetActive(false);
            this.keyCountText.text = "40 / 0";
            this.chestCountText.text = "0";
            this.sliderBarImage.transform.localScale = Vector3.zero;
            this.sliderBarText.text = "0 / 10";
            this.currentOpenCount = 0;
        }

        void Increase() {
            if (this.currentOpenCount >= this.onceMaxOpenCount) return;
            int nextOpenCount = this.currentOpenCount + 1;
            bool isEnoughCost = this.CheckIsEnoughCost(nextOpenCount, chest.cost);
            if (isEnoughCost == false) return;
            this.soundManager.Click();
            this.openButton.Enable();
            this.currentOpenCount += 1;
            this.UpdateView();
        }

        void Decrease() {
            if (0 >= this.currentOpenCount) return;
            this.soundManager.Click();
            this.currentOpenCount -= 1;
            if (this.currentOpenCount == 0) {
                this.openButton.Disable();
            }
            this.UpdateView();
        }

        void IncreaseToMax() {
            if (this.currentOpenCount >= this.onceMaxOpenCount) return;
            int maxOpenCount = this.GetMaxOpenCount(chest.cost);
            if (maxOpenCount == 0) return;
            this.soundManager.Click();
            this.openButton.Enable();
            this.currentOpenCount = maxOpenCount;
            this.UpdateView();
        }

        void OpenChest() {
            bool isEnoughCost = this.CheckIsEnoughCost(this.currentOpenCount, chest.cost);
            if (isEnoughCost == false || this.currentOpenCount == 0 || this.currentOpenCount > this.onceMaxOpenCount) return;
            this.soundManager.Click();
            this.loadingPopup.Open();
            this.userController.OpenSilverChest(this.currentOpenCount);
        }

        bool CheckIsEnoughCost(int openCount, Cost cost) {
            bool isEnough = false;
            if (cost.item.type == ItemType.Currency) {
                switch (cost.item.id) {
                    // Gold
                    case 1:
                        isEnough = this.userModel.user.gold >= (cost.amount * openCount);
                        break;
                }
            } else if (cost.item.type == ItemType.Key) {
                switch (cost.item.id) {
                    // Silver Key
                    case 10:
                        isEnough = this.userModel.user.key >= (cost.amount * openCount);
                        break;
                }
            }
            return isEnough;
        }

        int GetMaxOpenCount(Cost cost) {
            int maxOpenCount = 0;
            if (cost.item.type == ItemType.Currency) {
                switch (cost.item.id) {
                    // Gold
                    case 1:
                        maxOpenCount = Mathf.Min((this.userModel.user.gold / cost.amount), this.onceMaxOpenCount);
                        break;
                }
            } else if (cost.item.type == ItemType.Key) {
                switch (cost.item.id) {
                    // Silver Key
                    case 10:
                        maxOpenCount = Mathf.Min((this.userModel.user.key / cost.amount), this.onceMaxOpenCount);
                        break;
                }
            }
            return maxOpenCount;
        }

        void UpdateView() {
            this.keyCountText.text = $"{chest.cost.amount * this.currentOpenCount} / {chest.cost.amount}";
            this.chestCountText.text = this.currentOpenCount.ToString();
            if (this.currentOpenCount == this.onceMaxOpenCount) {
                this.sliderBarImage.transform.localScale = Vector3.one;
            } else if (this.currentOpenCount == 0) {
                this.sliderBarImage.transform.localScale = Vector3.zero;
            } else {
                this.sliderBarImage.transform.localScale = new Vector3((this.currentOpenCount / (this.onceMaxOpenCount * 1f)), 1, 1);
            }
            this.sliderBarText.text = $"{this.currentOpenCount} / {this.onceMaxOpenCount}";
        }
    }
}
