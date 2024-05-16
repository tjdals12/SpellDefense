using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Part {
    using UserModel = Model.OutGame.UserModel;
    using UserController = Controller.OutGame.UserController;
    using InventoryPart = Model.OutGame.IInventoryPart;
    using PartType = Repository.Part.PartType;
    using PartGrade = Repository.Part.PartGrade;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class PartPopup : MonoBehaviour
    {
        UserModel userModel;
        UserController userController;

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
        TextMeshProUGUI partNameText;
        [SerializeField]
        TextMeshProUGUI partGradeText;
        [SerializeField]
        GameObject partPanel;
        [SerializeField]
        PartSpec partSpec;
        [SerializeField]
        GameObject materialPartListPanel;
        [SerializeField]
        UpgradeButton upgradeButton;
        [SerializeField]
        EquipButton equipButton;
        [SerializeField]
        Button closeButton;

        [Header("UI-Popup")]
        [Space(4)]
        [SerializeField]
        PartUpgradePopup partUpgradePopup;

        [Header("Prefab - Common")]
        [Space(4)]
        [SerializeField]
        GameObject commonWeaponPrefab;
        [SerializeField]
        GameObject commonArmorPrefab;
        [SerializeField]
        GameObject commonJewelryPrefab;

        [Header("Prefab - Uncommon")]
        [Space(4)]
        [SerializeField]
        GameObject uncommonWeaponPrefab;
        [SerializeField]
        GameObject uncommonArmorPrefab;
        [SerializeField]
        GameObject uncommonJewelryPrefab;

        [Header("Prefab - Rare")]
        [Space(4)]
        [SerializeField]
        GameObject rareWeaponPrefab;
        [SerializeField]
        GameObject rareArmorPrefab;
        [SerializeField]
        GameObject rareJewelryPrefab;

        [Header("Prefab - Epic")]
        [Space(4)]
        [SerializeField]
        GameObject epicWeaponPrefab;
        [SerializeField]
        GameObject epicArmorPrefab;
        [SerializeField]
        GameObject epicJewelryPrefab;

        [Header("Prefab - Material")]
        [Space(4)]
        [SerializeField]
        GameObject commonMaterialPartPrefab;
        [SerializeField]
        GameObject uncommonMaterialPartPrefab;
        [SerializeField]
        GameObject rareMaterialPartPrefab;
        [SerializeField]
        GameObject epicMaterialPartPrefab;

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Part part;
        Dictionary<string, MaterialPart> materialParts;
        HashSet<string> selectedParts;
        int earnExp = 0;
        int prevLevel;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.closeButton.onClick.AddListener(this.Close);
        }
        void OnEnable() {
            this.userModel.OnEarnPart += this.OnEarnPart;
            this.userModel.OnUpgradePart += this.OnUpgradePart;
            this.userModel.OnEquipPart += this.OnEquipPart;
        }
        void OnDisable() {
            this.userModel.OnEarnPart -= this.OnEarnPart;
            this.userModel.OnUpgradePart -= this.OnUpgradePart;
            this.userModel.OnEquipPart -= this.OnEquipPart;
        }
        #endregion

        #region Event Listeners
        void OnEarnPart(string id) {
            if (this.materialParts != null) {
                InventoryPart inventoryPart = this.userModel.user.inventoryParts.Find((e) => e.id == id);
                GameObject prefab = this.GetMaterialPrefab(inventoryPart.part.grade);
                GameObject clone = Instantiate(prefab, this.materialPartListPanel.transform);
                MaterialPart materialPart = clone.GetComponent<MaterialPart>();
                materialPart.Initialize(inventoryPart, (inventoryPart) => this.SelectMaterial(inventoryPart));
                this.materialParts[inventoryPart.id] = materialPart;
            }
        }
        void OnUpgradePart(string id, string[] materialIds) {
            InventoryPart inventoryPart = this.userModel.user.inventoryParts.Find((e) => e.id == id);
            this.partUpgradePopup.Open(inventoryPart, inventoryPart.level - this.prevLevel);
            this.part.UpdateView(inventoryPart);
            this.partSpec.UpdateView(inventoryPart);
            foreach (string materialId in materialIds) {
                Destroy(this.materialParts[materialId].gameObject);
                this.materialParts.Remove(materialId);
            }
            this.selectedParts = new();
            this.earnExp = 0;
            this.upgradeButton.UpdateView(inventoryPart);
        }
        void OnEquipPart(string id) {
            this.Close();
        }
        #endregion

        public void Open(InventoryPart inventoryPart) {
            this.SetTitle(inventoryPart);
            this.SetPart(inventoryPart);
            this.SetPartSpec(inventoryPart);
            this.SetMaterialPartList(inventoryPart);
            this.SetUpgradeButton(inventoryPart);
            this.SetEquipButton(inventoryPart);
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack));
        }

        void Close() {
            this.soundManager.Close();
            foreach (var materialPart in this.materialParts.Values) {
                materialPart.gameObject.SetActive(true);
                materialPart.Deselect();
            }
            this.selectedParts = new();
            this.earnExp = 0;
            this.popup.SetActive(false);
        }

        void SetTitle(InventoryPart inventoryPart) {
            this.partNameText.text = inventoryPart.part.name;
            this.partGradeText.text = inventoryPart.part.gradeName;
            switch (inventoryPart.part.grade) {
                case PartGrade.Common:
                    this.partGradeText.color = new Color32(19, 123, 255, 255);
                    break;
                case PartGrade.Uncommon:
                    this.partGradeText.color = new Color32(67, 161, 37, 255);
                    break;
                case PartGrade.Rare:
                    this.partGradeText.color = new Color32(184, 63, 234, 255);
                    break;
                case PartGrade.Epic:
                    this.partGradeText.color = new Color32(252, 23, 0, 255);
                    break;
            }
        }

        void SetPart(InventoryPart inventoryPart) {
            for (int i = 0; i < this.partPanel.transform.childCount; i++) {
                GameObject child = this.partPanel.transform.GetChild(i).gameObject;
                Destroy(child);
            }
            PartType partType = inventoryPart.part.type;
            PartGrade partGrade = inventoryPart.part.grade;
            GameObject prefab = this.GetPrefab(partType, partGrade);
            GameObject clone = Instantiate(prefab, this.partPanel.transform);
            Part part = clone.GetComponent<Part>();
            part.Initialize(inventoryPart);
            this.part = part;
        }

        void SetPartSpec(InventoryPart inventoryPart) {
            this.partSpec.Initialize(inventoryPart);
        }

        void SetMaterialPartList(InventoryPart selectedInventoryPart) {
            var equippedParts = this.userModel.user.equippedParts;
            if (this.materialParts == null) {
                this.materialParts = new();
                var inventoryParts = this.userModel.user.inventoryParts;
                foreach (var inventoryPart in inventoryParts) {
                    GameObject prefab = this.GetMaterialPrefab(inventoryPart.part.grade);
                    GameObject clone = Instantiate(prefab, this.materialPartListPanel.transform);
                    MaterialPart materialPart = clone.GetComponent<MaterialPart>();
                    materialPart.Initialize(inventoryPart, (inventoryPart) => {
                        this.soundManager.Click();
                        this.SelectMaterial(inventoryPart);
                    });
                    if (inventoryPart.id == selectedInventoryPart.id) {
                        clone.SetActive(false);
                    } else if (inventoryPart.id == equippedParts.weapon) {
                        clone.SetActive(false);
                    } else if (inventoryPart.id == equippedParts.armor) {
                        clone.SetActive(false);
                    } else if (inventoryPart.id == equippedParts.jewelry) {
                        clone.SetActive(false);
                    }
                    this.materialParts[inventoryPart.id] = materialPart;
                }
            } else {
                if (this.materialParts.ContainsKey(selectedInventoryPart.id)) {
                    this.materialParts[selectedInventoryPart.id].gameObject.SetActive(false);
                }
                if (this.materialParts.ContainsKey(equippedParts.weapon)) {
                    this.materialParts[equippedParts.weapon].gameObject.SetActive(false);
                }
                if (this.materialParts.ContainsKey(equippedParts.armor)) {
                    this.materialParts[equippedParts.armor].gameObject.SetActive(false);
                }
                if (this.materialParts.ContainsKey(equippedParts.jewelry)) {
                    this.materialParts[equippedParts.jewelry].gameObject.SetActive(false);
                }
            }
        }

        void SetUpgradeButton(InventoryPart inventoryPart) {
            this.upgradeButton.Initialize(
                inventoryPart,
                this.userModel.user.gold,
                onClick: () => {
                    this.soundManager.Click();
                    this.UpgradePart(inventoryPart);
                }
            );
        }

        void SetEquipButton(InventoryPart inventoryPart) {
            this.equipButton.Initialize(
                inventoryPart,
                onClick: () => {
                    this.soundManager.Click();
                    this.EquipPart(inventoryPart);
                }
            );
            var equippedParts = this.userModel.user.equippedParts;
            if (
                equippedParts.weapon == inventoryPart.id ||
                equippedParts.armor == inventoryPart.id ||
                equippedParts.jewelry == inventoryPart.id
            ) {
                this.equipButton.Disable();
            } else {
                this.equipButton.Enable();
            }
        }

        GameObject GetPrefab(PartType partType, PartGrade partGrade) {
            GameObject prefab = null;
            if (partType == PartType.Weapon) {
                switch (partGrade) {
                    case PartGrade.Common:
                        prefab = commonWeaponPrefab;
                        break;
                    case PartGrade.Uncommon:
                        prefab = uncommonWeaponPrefab;
                        break;
                    case PartGrade.Rare:
                        prefab = rareWeaponPrefab;
                        break;
                    case PartGrade.Epic:
                        prefab = epicWeaponPrefab;
                        break;
                    default:
                        prefab = commonWeaponPrefab;
                        break;
                }
            } else if (partType == PartType.Armor) {
                switch (partGrade) {
                    case PartGrade.Common:
                        prefab = commonArmorPrefab;
                        break;
                    case PartGrade.Uncommon:
                        prefab = uncommonArmorPrefab;
                        break;
                    case PartGrade.Rare:
                        prefab = rareArmorPrefab;
                        break;
                    case PartGrade.Epic:
                        prefab = epicArmorPrefab;
                        break;
                    default:
                        prefab = commonArmorPrefab;
                        break;
                }
            } else if (partType == PartType.Jewelry) {
                switch (partGrade) {
                    case PartGrade.Common:
                        prefab = commonJewelryPrefab;
                        break;
                    case PartGrade.Uncommon:
                        prefab = uncommonJewelryPrefab;
                        break;
                    case PartGrade.Rare:
                        prefab = rareJewelryPrefab;
                        break;
                    case PartGrade.Epic:
                        prefab = epicJewelryPrefab;
                        break;
                    default:
                        prefab = commonJewelryPrefab;
                        break;
                }
            }
            return prefab;
        }

        GameObject GetMaterialPrefab(PartGrade partGrade) {
            GameObject prefab = null;
            switch (partGrade) {
                case PartGrade.Common:
                    prefab = this.commonMaterialPartPrefab;
                    break;
                case PartGrade.Uncommon:
                    prefab = this.uncommonMaterialPartPrefab;
                    break;
                case PartGrade.Rare:
                    prefab = this.rareMaterialPartPrefab;
                    break;
                case PartGrade.Epic:
                    prefab = this.epicMaterialPartPrefab;
                    break;
                default:
                    prefab = this.commonMaterialPartPrefab;
                    break;
            }
            return prefab;
        }

        void SelectMaterial(InventoryPart inventoryPart) {
            if (this.selectedParts == null) {
                this.selectedParts = new();
            }
            if (this.selectedParts.Contains(inventoryPart.id)) {
                this.materialParts[inventoryPart.id].Deselect();
                this.selectedParts.Remove(inventoryPart.id);
                if (this.earnExp > 0) {
                    this.earnExp -= inventoryPart.exp;
                }
            } else {
                this.materialParts[inventoryPart.id].Select();
                this.selectedParts.Add(inventoryPart.id);
                this.earnExp += inventoryPart.exp;
            }
            this.part.UpdateView(this.earnExp);
            this.partSpec.UpdateView(this.earnExp);
            this.upgradeButton.UpdateView(this.earnExp, this.userModel.user.gold);
        }

        void UpgradePart(InventoryPart inventoryPart) {
            this.loadingPopup.Open();
            this.prevLevel = inventoryPart.level;
            string id = inventoryPart.id;
            string[] materialPartIds = this.selectedParts.ToArray();
            this.userController.UpgradePart(id, materialPartIds);
        }

        void EquipPart(InventoryPart inventoryPart) {
            this.loadingPopup.Open();
            this.userController.EquipPart(inventoryPart.id);
        }
    }
}
