using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;
    using UserModel = Model.OutGame.IUserModel;
    using UserController = Controller.OutGame.UserController;
    using ToggleButton = Common.ToggleButton;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class SkillBookPopup : MonoBehaviour
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
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        GameObject window;
        [SerializeField]
        TextMeshProUGUI skillBookNameText;
        [SerializeField]
        TextMeshProUGUI skillBookGradeText;
        [SerializeField]
        GameObject skillBookPanel;
        [SerializeField]
        TextMeshProUGUI descriptionText;
        [SerializeField]
        Button skillPreviewButton;
        [SerializeField]
        ToggleButton[] tabMenus;
        [SerializeField]
        GameObject[] tabContents;
        [SerializeField]
        UpgradeButton upgradeButton;
        [SerializeField]
        EquipButton equipButton;
        [SerializeField]
        Button closeButton;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        SkillBookUpgradePopup skillBookUpgradePopup;
        [SerializeField]
        SkillBookEquipPopup skillBookEquipPopup;
        [SerializeField]
        SkillPreviewPopup skillPreviewPopup;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject commonSkillBookPrefab;
        [SerializeField]
        GameObject uncommonSkillBookPrefab;
        [SerializeField]
        GameObject rareSkillBookPrefab;
        [SerializeField]
        GameObject epicSkillBookPrefab;
        [SerializeField]
        GameObject damageStatPrefab;
        [SerializeField]
        GameObject coolTimeStatPrefab;

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        SkillBook currentSkillBook;
        Dictionary<string, Stat>[] stats;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.closeButton.onClick.AddListener(() => {
                this.soundManager.Close();
                this.Close();
            });
            foreach (int index in Enumerable.Range(0, this.tabMenus.Length)) {
                this.tabMenus[index].Initialize(onClick: () => this.SelectSkill(index));
            }
        }
        void OnEnable() {
            this.userModel.OnUpgradeSkillBook += this.OnUpgradeSkillBook;
            this.userModel.OnEquipSkillBook += this.OnEquipSkillBook;
        }
        void OnDisable() {
            this.userModel.OnUpgradeSkillBook -= this.OnUpgradeSkillBook;
            this.userModel.OnEquipSkillBook -= this.OnEquipSkillBook;
        }
        #endregion

        #region Event Listeners
        void OnUpgradeSkillBook(int skillBookId) {
            InventorySkillBook inventorySkillBook = this.userModel.user.GetSkillBook(skillBookId);
            this.skillBookUpgradePopup.Open(inventorySkillBook);
            if (this.currentSkillBook != null) {
                this.UpdateSkillBook(inventorySkillBook);
                this.UpdateSkills(inventorySkillBook);
                this.UpdateUpgradeButton(inventorySkillBook);
            }
        }
        void OnEquipSkillBook(int index) {
            this.Close();
        }
        #endregion

        public void Open(InventorySkillBook inventorySkillBook) {
            this.SetTitle(inventorySkillBook);
            this.SetSkillBook(inventorySkillBook);
            this.SetDescription(inventorySkillBook);
            this.SetSkillPreview(inventorySkillBook);
            this.SetSkills(inventorySkillBook);
            this.SetUpgradeButton(inventorySkillBook);
            this.SetEquipButton(inventorySkillBook);
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.99f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Close() {
            this.popup.SetActive(false);
        }

        void SetTitle(InventorySkillBook inventorySkillBook) {
            this.skillBookNameText.text = inventorySkillBook.skillBook.name;
            this.skillBookGradeText.text = inventorySkillBook.skillBook.gradeName;
            switch (inventorySkillBook.skillBook.grade) {
                case SkillBookGrade.Common:
                    this.skillBookGradeText.color = new Color32(19, 123, 255, 255);
                    break;
                case SkillBookGrade.Uncommon:
                    this.skillBookGradeText.color = new Color32(67, 161, 37, 255);
                    break;
                case SkillBookGrade.Rare:
                    this.skillBookGradeText.color = new Color32(184, 63, 234, 255);
                    break;
                case SkillBookGrade.Epic:
                    this.skillBookGradeText.color = new Color32(252, 23, 0, 255);
                    break;
            }
        }

        void SetSkillBook(InventorySkillBook inventorySkillBook) {
            for (int i = 0; i < this.skillBookPanel.transform.childCount; i++) {
                GameObject child = this.skillBookPanel.transform.GetChild(i).gameObject;
                Destroy(child);
            }
            GameObject prefab = null;
            switch (inventorySkillBook.skillBook.grade) {
                case SkillBookGrade.Common:
                    prefab = this.commonSkillBookPrefab;
                    break;
                case SkillBookGrade.Uncommon:
                    prefab = this.uncommonSkillBookPrefab;
                    break;
                case SkillBookGrade.Rare:
                    prefab = this.rareSkillBookPrefab;
                    break;
                case SkillBookGrade.Epic:
                    prefab = this.epicSkillBookPrefab;
                    break;
                default:
                    prefab = this.commonSkillBookPrefab;
                    break;
            }
            GameObject clone = Instantiate(prefab, this.skillBookPanel.transform);
            SkillBook skillBook = clone.GetComponent<SkillBook>();
            skillBook.Initialize(inventorySkillBook);
            this.currentSkillBook = skillBook;
        }

        void UpdateSkillBook(InventorySkillBook inventorySkillBook) {
            this.currentSkillBook.UpdateView(inventorySkillBook);
        }

        void SetDescription(InventorySkillBook inventorySkillBook) {
            this.descriptionText.text = inventorySkillBook.skillBook.description;
        }

        void SetSkillPreview(InventorySkillBook inventorySkillBook) {
            this.skillPreviewButton.onClick.RemoveAllListeners();
            this.skillPreviewButton.onClick.AddListener(() => {
                this.soundManager.Click();
                this.skillPreviewPopup.Open(inventorySkillBook);
            });
        }

        void SetSkills(InventorySkillBook inventorySkillBook) {
            var skills = inventorySkillBook.skillBook.skills;
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            bool canUpgrade = currentUpgradeSpec == null ? false : inventorySkillBook.amount >= currentUpgradeSpec.requiredAmount;
            this.stats = new Dictionary<string, Stat>[skills.Length];
            for (int i = 0; i < skills.Length; i++) {
                Dictionary<string, Stat> dict = new();

                var skill = skills[i];

                GameObject tabContent = this.tabContents[i];
                for (int j = 0; j < tabContent.transform.childCount; j++) {
                    GameObject child = tabContent.transform.GetChild(j).gameObject;
                    Destroy(child);
                }

                GameObject damageStatClone = Instantiate(this.damageStatPrefab, tabContent.transform);
                Stat damageStat = damageStatClone.GetComponent<Stat>();
                float damage = skill.spec.damage + (skill.spec.damagePerLevel * (inventorySkillBook.level - 1));
                if (canUpgrade) {
                    damageStat.Initialize($"ATK * {damage} <size=24><color=green>(+{skill.spec.damagePerLevel})</size></color>");
                } else {
                    damageStat.Initialize($"ATK * {damage}");
                }
                dict["damage"] = damageStat;

                GameObject coolTimeStatClone = Instantiate(this.coolTimeStatPrefab, tabContent.transform);
                Stat coolTimeStat = coolTimeStatClone.GetComponent<Stat>();
                coolTimeStat.Initialize($"{skill.spec.coolTime}s");
                dict["coolTime"] = coolTimeStat;

                this.stats[i] = dict;
            }
        }

        void UpdateSkills(InventorySkillBook inventorySkillBook) {
            var skills = inventorySkillBook.skillBook.skills;
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            bool canUpgrade = currentUpgradeSpec == null ? false : inventorySkillBook.amount >= currentUpgradeSpec.requiredAmount;
            for (int i = 0; i < skills.Length; i++) {
                Dictionary<string, Stat> dict = this.stats[i];
                var skill = skills[i];
                float damage = skill.spec.damage + (skill.spec.damagePerLevel * inventorySkillBook.level);
                if (canUpgrade) {
                    dict["damage"].Initialize($"ATK * {damage} <size=24><color=green>(+{skill.spec.damagePerLevel})</size></color>");
                } else {
                    dict["damage"].Initialize($"ATK * {damage}");
                }
                dict["coolTime"].Initialize($"{skill.spec.coolTime}s");
            }
        }

        void SetUpgradeButton(InventorySkillBook inventorySkillBook) {
            this.upgradeButton.Initialize(
                inventorySkillBook,
                onClick: () => {
                    this.soundManager.Click();
                    this.UpgradeSkillBook(inventorySkillBook);
                }
            );
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.upgradeButton.Disable();
            } else {
                bool isEnoughGold = this.userModel.user.gold >= currentUpgradeSpec.requiredGold;
                bool isEnoughAmount = inventorySkillBook.amount >= currentUpgradeSpec.requiredAmount;
                if (isEnoughGold && isEnoughAmount) {
                    this.upgradeButton.Enable();
                } else {
                    this.upgradeButton.Disable();
                }
            }
        }

        void UpdateUpgradeButton(InventorySkillBook inventorySkillBook) {
            this.upgradeButton.UpdateView(inventorySkillBook);
            var currentUpgradeSpec = inventorySkillBook.GetCurrentUpgradeSpec();
            if (currentUpgradeSpec == null) {
                this.upgradeButton.Disable();
            } else {
                bool isEnoughGold = this.userModel.user.gold >= currentUpgradeSpec.requiredGold;
                bool isEnoughAmount = inventorySkillBook.amount >= currentUpgradeSpec.requiredAmount;
                if (isEnoughGold && isEnoughAmount) {
                    this.upgradeButton.Enable();
                } else {
                    this.upgradeButton.Disable();
                }
            }
        }

        void SetEquipButton(InventorySkillBook inventorySkillBook) {
            this.equipButton.Initialize(
                inventorySkillBook,
                onClick: () => {
                    this.soundManager.Click();
                    this.skillBookEquipPopup.Open(inventorySkillBook);
                }
            );
            if (this.userModel.user.IsEquippedSkillBook(inventorySkillBook.skillBook.id)) {
                this.equipButton.Disable();
            } else {
                this.equipButton.Enable();
            }
        }

        void SelectSkill(int index) {
            for (int i = 0; i < this.tabMenus.Length; i++) {
                var tabMenu = this.tabMenus[i];
                var tabContent = this.tabContents[i];
                if (i == index) {
                    tabMenu.Enable(withButton: false);
                    tabContent.SetActive(true);
                } else {
                    tabMenu.Disable(withButton: false);
                    tabContent.SetActive(false);
                }
            }
        }

        void UpgradeSkillBook(InventorySkillBook inventorySkillBook) {
            this.upgradeButton.Disable();
            this.loadingPopup.Open();
            this.userController.UpgradeSkillBook(inventorySkillBook.skillBook.id);
        }
    }
}
