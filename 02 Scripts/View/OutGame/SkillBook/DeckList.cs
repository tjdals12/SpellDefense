using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.OutGame.SkillBook {
    using UserModel = Model.OutGame.IUserModel;
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;
    using UserController = Controller.OutGame.UserController;
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class DeckList : MonoBehaviour
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
        GameObject[] deckPanels;
        [SerializeField]
        DeckNumber[] deckNumbers;

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

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Dictionary<int, DeckSkillBook>[] skillBooksByDeck;
        int[][] skillBookIdsByDeck;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.ShowDecks();
        }
        void OnEnable() {
            this.userModel.OnChangeDeck += this.OnChangeDeck;
            this.userModel.OnEarnSkillBook += this.OnEarnSkillBook;
            this.userModel.OnUpgradeSkillBook += this.OnUpgradeSkillBook;
            this.userModel.OnEquipSkillBook += this.OnEquipSkillBook;
        }
        void OnDisable() {
            this.userModel.OnChangeDeck -= this.OnChangeDeck;
            this.userModel.OnEarnSkillBook -= this.OnEarnSkillBook;
            this.userModel.OnUpgradeSkillBook -= this.OnUpgradeSkillBook;
            this.userModel.OnEquipSkillBook -= this.OnEquipSkillBook;
        }
        #endregion

        #region Event Listeners
        void OnChangeDeck(int index) {
            for (int i = 0; i < this.deckPanels.Length; i++) {
                GameObject parent = this.deckPanels[i];
                DeckNumber deckNumber = this.deckNumbers[i];
                if (i == index) {
                    parent.SetActive(true);
                    deckNumber.Enable();
                } else {
                    parent.SetActive(false);
                    deckNumber.Disable();
                }
            } 
        }
        void OnEarnSkillBook(int skillBookId) {
            InventorySkillBook inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
            this.UpdateDeck(inventorySkillBook);
        }
        void OnUpgradeSkillBook(int skillBookId) {
            InventorySkillBook inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
            this.UpdateDeck(inventorySkillBook);
        }
        void OnEquipSkillBook(int index) {
            this.ChangeSkillBookInCurrentDeck(index);
        }
        #endregion

        void ShowDecks() {
            this.skillBooksByDeck = new Dictionary<int, DeckSkillBook>[this.deckPanels.Length];
            this.skillBookIdsByDeck = new int[this.deckPanels.Length][];
            for (int i = 0; i < this.deckPanels.Length; i++) {
                this.CreateDeck(i);
            }

        }

        void CreateDeck(int index) {
            Dictionary<int, DeckSkillBook> skillBooks = new();
            List<int> skillBookIds = new();
            var deck = this.userModel.user.decks[index];
            GameObject parent = this.deckPanels[index];
            foreach (int skillBookId in deck.skillBooks) {
                var inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == skillBookId);
                GameObject prefab = this.GetPrefab(inventorySkillBook.skillBook.grade);
                GameObject clone = Instantiate(prefab, parent.transform);
                DeckSkillBook skillBook = clone.GetComponent<DeckSkillBook>();
                skillBook.Initialize(inventorySkillBook, (inventorySkillBook) => this.skillBookPopup.Open(inventorySkillBook));
                skillBooks[skillBookId] = skillBook;
                skillBookIds.Add(skillBookId);
            }
            this.skillBooksByDeck[index] = skillBooks;
            this.skillBookIdsByDeck[index] = skillBookIds.ToArray();

            DeckNumber deckNumber = this.deckNumbers[index];
            deckNumber.Initialize(onClick: () => this.ChangeDeck(index));
            if (deck.isUse) {
                parent.SetActive(true);
                deckNumber.Enable();
            } else {
                parent.SetActive(false);
                deckNumber.Disable();
            }
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

        void ChangeDeck(int index) {
            this.soundManager.Click();
            this.loadingPopup.Open();
            this.userController.ChangeDeck(index);
        }

        void UpdateDeck(InventorySkillBook inventorySkillBook) {
            foreach (var skillBooks in this.skillBooksByDeck) {
                if (skillBooks.ContainsKey(inventorySkillBook.skillBook.id)) {
                    skillBooks[inventorySkillBook.skillBook.id].UpdateView(inventorySkillBook);
                }
            }
        }

        void ChangeSkillBookInCurrentDeck(int index) {
            int currentDeckIndex = Array.FindIndex(this.userModel.user.decks, (e) => e.isUse == true);
            var currentDeck = this.userModel.user.decks[currentDeckIndex];

            int[] skillBookIds = this.skillBookIdsByDeck[currentDeckIndex];
            int prevSkillBookId = skillBookIds[index];
            int newSkillBookId = currentDeck.skillBooks[index];

            Dictionary<int, DeckSkillBook> skillBooks = this.skillBooksByDeck[currentDeckIndex];
            DeckSkillBook prevSkillBook = skillBooks[prevSkillBookId];
            Destroy(prevSkillBook.gameObject);
            var inventorySkillBook = Array.Find(this.userModel.user.inventorySkillBooks, (e) => e.skillBook.id == newSkillBookId);
            GameObject prefab = this.GetPrefab(inventorySkillBook.skillBook.grade);
            GameObject parent = this.deckPanels[currentDeckIndex];
            GameObject clone = Instantiate(prefab, parent.transform);
            clone.transform.SetSiblingIndex(index);
            DeckSkillBook skillBook = clone.GetComponent<DeckSkillBook>();
            skillBook.Initialize(inventorySkillBook, (inventorySkillBook) => {
                this.soundManager.Click();
                this.skillBookPopup.Open(inventorySkillBook);
            });

            skillBookIds[index] = newSkillBookId;
            skillBooks.Add(newSkillBookId, skillBook);
            skillBooks.Remove(prevSkillBookId);
        }
    }
}
