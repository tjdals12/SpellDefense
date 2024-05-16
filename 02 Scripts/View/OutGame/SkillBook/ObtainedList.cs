using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.OutGame.SkillBook {
    using UserModel = Model.OutGame.IUserModel;
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;
    using Deck = Model.OutGame.IDeck;
    using Model.OutGame;
    using Model.Common;
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;

    public class ObtainedList : MonoBehaviour
    {
        UserModel userModel;

        [Header("Audio")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject listPanel;

        [Header("UI - Popup")]
        [Space(4)]
        [SerializeField]
        SkillBookPopup skillBookPopup;

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

        Dictionary<int, ObtainedSkillBook> skillBooks;
        ObtainedSkillBook[] equippedSkillBooks;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.ShowSkillBooks();
        }
        void OnEnable() {
            this.userModel.OnOpenChest += this.OnOpenChest;
            this.userModel.OnEarnSkillBook += this.OnEarnSkillBook;
            this.userModel.OnUpgradeSkillBook += this.OnUpgradeSkillBook;
            this.userModel.OnChangeDeck += this.OnChangeDeck;
            this.userModel.OnEquipSkillBook += this.OnEquipSkillBook;
        }
        void OnDisable() {
            this.userModel.OnOpenChest -= this.OnOpenChest;
            this.userModel.OnEarnSkillBook -= this.OnEarnSkillBook;
            this.userModel.OnUpgradeSkillBook -= this.OnUpgradeSkillBook;
            this.userModel.OnChangeDeck -= this.OnChangeDeck;
            this.userModel.OnEquipSkillBook -= this.OnEquipSkillBook;
        }
        #endregion

        #region Event Listeners
        void OnOpenChest(Chest chest, int count, List<RewardItem> rewardItems) {
            foreach (var rewardItem in rewardItems) {
                InventorySkillBook inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == rewardItem.item.id);
                if (inventorySkillBook != null) {
                    if (this.skillBooks.ContainsKey(inventorySkillBook.skillBook.id)) {
                        this.skillBooks[inventorySkillBook.skillBook.id].UpdateView(inventorySkillBook);
                    } else {
                        this.CreateSkillBook(inventorySkillBook);
                    }
                }
            }
        }
        void OnEarnSkillBook(int skillBookId) {
            InventorySkillBook inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
            if (this.skillBooks.ContainsKey(inventorySkillBook.skillBook.id)) {
                this.UpdateSkillBook(inventorySkillBook);
            } else {
                this.CreateSkillBook(inventorySkillBook);
            }
        }
        void OnUpgradeSkillBook(int skillBookId) {
            InventorySkillBook inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
            if (this.skillBooks.ContainsKey(inventorySkillBook.skillBook.id)) {
                this.UpdateSkillBook(inventorySkillBook);
            }
        }
        void OnChangeDeck(int index) {
            var currentDeck = this.userModel.user.decks[index];
            this.UpdateEquippedSkillBooks(currentDeck);
        }
        void OnEquipSkillBook(int index) {
            this.equippedSkillBooks[index].Unequip();
            var currentDeck = Array.Find(this.userModel.user.decks, (e) => e.isUse);
            int skillBookId = currentDeck.skillBooks[index];
            ObtainedSkillBook obtainedSkillBook = this.skillBooks[skillBookId];
            obtainedSkillBook.Equip();
            this.equippedSkillBooks[index] = obtainedSkillBook;
        }
        #endregion

        void ShowSkillBooks() {
            if (this.skillBooks == null) {
                this.skillBooks = new();
            } else {
                foreach (var skillBook in this.skillBooks.Values) {
                    Destroy(skillBook.gameObject);
                }
            }
            foreach (var inventorySkillBook in this.userModel.user.inventorySkillBooks) {
                if (inventorySkillBook.isObtained == false) continue;
                this.CreateSkillBook(inventorySkillBook);
            }
            var currentDeck = Array.Find(this.userModel.user.decks, (e) => e.isUse == true);
            this.UpdateEquippedSkillBooks(currentDeck);
        }

        GameObject GetPrefab(SkillBookGrade skillBookGrade) {
            GameObject prefab = null;
            switch (skillBookGrade) {
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
            return prefab;
        }

        void CreateSkillBook(InventorySkillBook inventorySkillBook) {
            GameObject prefab = this.GetPrefab(inventorySkillBook.skillBook.grade);
            GameObject clone = Instantiate(prefab, this.listPanel.transform);
            ObtainedSkillBook obtainedSkillBook = clone.GetComponent<ObtainedSkillBook>();
            obtainedSkillBook.Unequip();
            obtainedSkillBook.Initialize(inventorySkillBook, (inventorySkillBook) => {
                this.soundManager.Click();
                this.skillBookPopup.Open(inventorySkillBook);
            });
            this.skillBooks[inventorySkillBook.skillBook.id] = obtainedSkillBook;
        }

        void UpdateSkillBook(InventorySkillBook inventorySkillBook) {
            this.skillBooks[inventorySkillBook.skillBook.id].UpdateView(inventorySkillBook);
        }

        void UpdateEquippedSkillBooks(Deck currentDeck) {
            int[] skillBookIds = currentDeck.skillBooks;
            if (this.equippedSkillBooks == null) {
                this.equippedSkillBooks = new ObtainedSkillBook[skillBookIds.Length];
            } else {
                foreach (var obtainedSkillBook in this.equippedSkillBooks) {
                    obtainedSkillBook.Unequip();
                }
            }
            for (int i = 0; i < skillBookIds.Length; i++) {
                int skillBookId = skillBookIds[i];
                ObtainedSkillBook obtainedSkillBook = this.skillBooks[skillBookId];
                obtainedSkillBook.Equip();
                this.equippedSkillBooks[i] = obtainedSkillBook;
            }
        }
    }
}
