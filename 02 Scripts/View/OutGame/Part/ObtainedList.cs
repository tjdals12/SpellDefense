using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Part {
    using UserModel = Model.OutGame.IUserModel;
    using PartType = Repository.Part.PartType;
    using PartGrade = Repository.Part.PartGrade;
    using InventoryPart = Model.OutGame.IInventoryPart;

    public class ObtainedList : MonoBehaviour
    {
        UserModel userModel;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject listPanel;
        [SerializeField]
        Filter filter;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        PartPopup partPopup;

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

        Dictionary<string, ObtainedPart> parts;
        Dictionary<string, PartType> partTypes;
        Dictionary<PartType, ObtainedPart> equippedParts;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.ShowParts();
        }
        void OnEnable() {
            this.userModel.OnEarnPart += this.OnEarnPart;
            this.userModel.OnUpgradePart += this.OnUpgradePart;
            this.userModel.OnEquipPart += this.OnEquipPart;
            this.filter.onChangeFilter += this.ChangeFilter;
        }
        void OnDisable() {
            this.userModel.OnEarnPart -= this.OnEarnPart;
            this.userModel.OnUpgradePart -= this.OnUpgradePart;
            this.userModel.OnEquipPart -= this.OnEquipPart;
            this.filter.onChangeFilter -= this.ChangeFilter;
        }
        #endregion

        #region Event Listeners
        void OnEarnPart(string id) {
            InventoryPart inventoryPart = this.userModel.user.GetPart(id);
            this.UpdatePart(inventoryPart);
        }
        void OnUpgradePart(string id, string[] materialIds) {
            InventoryPart inventoryPart = this.userModel.user.GetPart(id);
            this.UpdatePart(inventoryPart);
            foreach (string materialId in materialIds) {
                this.DeletePart(materialId);
            }
        }
        void OnEquipPart(string id) {
            this.UpdateEquippedParts();
        }
        #endregion

        void ShowParts() {
            if (this.parts == null) {
                this.parts = new();
                this.partTypes = new();
                this.equippedParts = new();
            } else {
                foreach (var part in this.parts.Values) {
                    Destroy(part.gameObject);
                }
                this.parts = new();
                this.partTypes = new();
                this.equippedParts = new();
            }
            var inventoryParts = this.userModel.user.inventoryParts;
            foreach (var inventoryPart in inventoryParts) {
                this.CreatePart(inventoryPart);
            }
            this.UpdateEquippedParts();
        }

        void ChangeFilter(FilterType filterType) {
            switch (filterType) {
                case FilterType.All:
                    this.ShowAll();
                    break;
                case FilterType.Weapon:
                    this.ShowOnly(PartType.Weapon);
                    break;
                case FilterType.Armor:
                    this.ShowOnly(PartType.Armor);
                    break;
                case FilterType.Jewelry:
                    this.ShowOnly(PartType.Jewelry);
                    break;
            }
        }

        void ShowAll() {
            foreach (var part in this.parts.Values) {
                part.gameObject.SetActive(true);
            }
        }

        void ShowOnly(PartType filterPartType) {
            foreach (var kv in this.parts) {
                PartType partType = this.partTypes[kv.Key];
                ObtainedPart part = kv.Value;
                if (partType == filterPartType) {
                    part.gameObject.SetActive(true);
                } else {
                    part.gameObject.SetActive(false);
                }
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

        void CreatePart(InventoryPart inventoryPart) {
            PartType partType = inventoryPart.part.type;
            PartGrade partGrade = inventoryPart.part.grade;
            GameObject prefab = this.GetPrefab(partType, partGrade);
            GameObject clone = Instantiate(prefab, this.listPanel.transform);
            ObtainedPart part = clone.GetComponent<ObtainedPart>();
            part.Initialize(inventoryPart, (inventoryPart) => {
                this.soundManager.Click();
                this.partPopup.Open(inventoryPart);
            });
            this.parts[inventoryPart.id] = part;
            this.partTypes[inventoryPart.id] = partType;
            if (this.userModel.user.IsEquippedPart(inventoryPart.id)) {
                part.Equip();
                this.equippedParts[inventoryPart.part.type] = part;
            } else {
                part.Unequip();
            }
        }

        void UpdatePart(InventoryPart inventoryPart) {
            if (this.parts.ContainsKey(inventoryPart.id)) {
                ObtainedPart part = this.parts[inventoryPart.id];
                part.UpdateView(inventoryPart);
            } else {
                this.CreatePart(inventoryPart);
            }
        }

        void DeletePart(string id) {
            if (this.parts.ContainsKey(id)) {
                ObtainedPart part = this.parts[id];
                Destroy(part.gameObject);
                this.parts.Remove(id);
                this.partTypes.Remove(id);
            }
        }

        void UpdateEquippedParts() {
            foreach (var equippedPart in this.equippedParts.Values) {
                equippedPart.Unequip();
            }
            InventoryPart equippedArmor = this.userModel.user.GetEquippedArmor();
            ObtainedPart armor = this.parts[equippedArmor.id];
            armor.Equip();
            this.equippedParts[equippedArmor.part.type] = armor;

            InventoryPart equippedWeapon = this.userModel.user.GetEquippedWeapon();
            ObtainedPart weapon = this.parts[equippedWeapon.id];
            weapon.Equip();
            this.equippedParts[equippedWeapon.part.type] = weapon;

            InventoryPart equippedJewelry = this.userModel.user.GetEquippedJewelry();
            ObtainedPart jewelry = this.parts[equippedJewelry.id];
            jewelry.Equip();
            this.equippedParts[equippedJewelry.part.type] = jewelry;
        }
    }
}
