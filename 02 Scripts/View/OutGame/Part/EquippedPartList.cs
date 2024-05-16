using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace View.OutGame.Part {
    using UserModel = Model.OutGame.UserModel;
    using InventoryPart = Model.OutGame.IInventoryPart;
    using PartType = Repository.Part.PartType;
    using PartGrade = Repository.Part.PartGrade;

    public class EquippedPartList : MonoBehaviour
    {
        UserModel userModel;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject weaponPanel;
        [SerializeField]
        GameObject armorPanel;
        [SerializeField]
        GameObject jewelryPanel;

        [Header("UI - Popup")]
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

        Dictionary<PartType, EquippedPart> parts;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.ShowParts();
        }
        void OnEnable() {
            this.userModel.OnUpgradePart += this.OnUpgradePart;
            this.userModel.OnEquipPart += this.OnEquipPart;
        }
        void OnDisable() {
            this.userModel.OnUpgradePart -= this.OnUpgradePart;
            this.userModel.OnEquipPart -= this.OnEquipPart;
        }
        #endregion

        #region Event Listeners
        void OnUpgradePart(string id, string[] materialIds) {
            InventoryPart inventoryPart = this.userModel.user.GetPart(id);
            this.UpdatePart(inventoryPart);
        }
        void OnEquipPart(string id) {
            InventoryPart inventoryPart = this.userModel.user.GetPart(id);
            this.ChangePart(inventoryPart);
        }
        #endregion

        void ShowParts() {
            if (this.parts == null) {
                this.parts = new();
            } else {
                foreach (var part in this.parts.Values) {
                    Destroy(part.gameObject);
                }
            }

            InventoryPart armor = this.userModel.user.GetEquippedArmor();
            this.CreatePart(armor);

            InventoryPart weapon = this.userModel.user.GetEquippedWeapon();
            this.CreatePart(weapon);

            InventoryPart jewelry = this.userModel.user.GetEquippedJewelry();
            this.CreatePart(jewelry);
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

        GameObject GetParent(PartType partType) {
            GameObject parent = null;
            if (partType == PartType.Weapon) {
                parent = this.weaponPanel;
            } else if (partType == PartType.Armor) {
                parent = this.armorPanel;
            } else if (partType == PartType.Jewelry) {
                parent = this.jewelryPanel;
            }
            return parent;
        }

        EquippedPart CreatePart(InventoryPart inventoryPart) {
            PartType partType = inventoryPart.part.type;
            PartGrade partGrade = inventoryPart.part.grade;
            GameObject prefab = this.GetPrefab(partType, partGrade);
            GameObject parent = this.GetParent(partType);
            GameObject clone = Instantiate(prefab, parent.transform);
            EquippedPart equippedPart = clone.GetComponent<EquippedPart>();
            equippedPart.Initialize(inventoryPart, (InventoryPart) => {
                this.soundManager.Click();
                this.partPopup.Open(inventoryPart);
            });
            this.parts[partType] = equippedPart;
            return equippedPart;
        }

        void UpdatePart(InventoryPart inventoryPart) {
            if (this.userModel.user.IsEquippedPart(inventoryPart.id))  {
                this.parts[inventoryPart.part.type].UpdateView(inventoryPart);
            }
        }

        void ChangePart(InventoryPart inventoryPart) {
            if (this.userModel.user.IsEquippedPart(inventoryPart.id)) {
                EquippedPart equippedPart = this.parts[inventoryPart.part.type];
                Destroy(equippedPart.gameObject);
                equippedPart = this.CreatePart(inventoryPart);
                DOTween.Sequence()
                    .Append(equippedPart.transform.DOScale(1.2f, 0.2f).From(1).SetEase(Ease.OutBack))
                    .Append(equippedPart.transform.DOScale(1, 0.1f));
            }
        }
    }
}
