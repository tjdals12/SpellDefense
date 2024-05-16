using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Battle {
    using ChestController = Controller.OutGame.ChestController;
    using UserModel = Model.OutGame.IUserModel;
    using Model.OutGame;
    using Model.Common;
    using ItemType = Repository.Item.ItemType;

    public class ChestIcon : MonoBehaviour
    {
        ChestController chestController;
        UserModel userModel;

        [Header("Audio")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI amountText;
        [SerializeField]
        OpenChestPopup openChestPopup;

        Chest chest;

        #region Unity Method
        void Awake() {
            this.chestController = GameObject.FindObjectOfType<ChestController>();
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.button.onClick.AddListener(this.Open);
            this.chest = this.chestController.GetSilverChest();
            this.UpdateUI();
        }
        void OnEnable() {
            this.userModel.OnChangeGold += this.OnChangeGold;
            this.userModel.OnChangeKey += this.OnChangeKey;
            this.userModel.OnOpenChest += this.OnOpenChest;
        }
        void OnDisable() {
            this.userModel.OnChangeGold -= this.OnChangeGold;
            this.userModel.OnChangeKey -= this.OnChangeKey;
            this.userModel.OnOpenChest -= this.OnOpenChest;
        }
        #endregion

        #region Event Listener
        void OnChangeGold() {
            this.UpdateUI();
        }
        void OnChangeKey() {
            this.UpdateUI();
        }
        void OnOpenChest(Chest chest, int count, List<RewardItem> rewardItems) {
            this.UpdateUI();
        }
        #endregion

        void UpdateUI() {
            Cost cost = this.chest.cost;
            this.iconImage.sprite = cost.item.image;
            int currentAmount = 0;
            if (cost.item.type == ItemType.Currency) {
                switch (cost.item.id) {
                    // Gold
                    case 1:
                        currentAmount = this.userModel.user.gold;
                        break;
                }
            } else if (cost.item.type == ItemType.Key) {
                switch (cost.item.id) {
                    // Silver Key:
                    case 10:
                        currentAmount = this.userModel.user.key;
                        break;
                }
            }
            this.amountText.text = currentAmount.ToString();
        }

        void Open() {
            this.soundManager.Click();
            this.openChestPopup.Open();
        }
    }
}
