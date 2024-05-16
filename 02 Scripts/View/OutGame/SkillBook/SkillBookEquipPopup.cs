using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.SkillBook {
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;
    using UserModel = Model.OutGame.IUserModel;
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;
    using UserController = Controller.OutGame.UserController;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class SkillBookEquipPopup : MonoBehaviour
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
        GameObject arrowPanel;
        [SerializeField]
        GameObject[] deckPanels;
        [SerializeField]
        GameObject skillBookPanel;
        [SerializeField]
        Button cancelButton;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject commonDeckSkillBookPrefab;
        [SerializeField]
        GameObject uncommonDeckSkillBookPrefab;
        [SerializeField]
        GameObject rareDeckSkillBookPrefab;
        [SerializeField]
        GameObject epicDeckSkillBookPrefab;
        [SerializeField]
        GameObject commonObtainedSkillBookPrefab;
        [SerializeField]
        GameObject uncommonObtainedSkillBookPrefab;
        [SerializeField]
        GameObject rareObtainedSkillBookPrefab;
        [SerializeField]
        GameObject epicObtainedSkillBookPrefab;

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Sequence arrowPanelSequence;

        Dictionary<int, DeckSkillBook>[] skillBooksByDeck;
        int[][] skillBookIdsByDeck;

        InventorySkillBook selectedSkillBook;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.userController = GameObject.FindObjectOfType<UserController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.cancelButton.onClick.AddListener(this.Close);
            this.arrowPanelSequence = DOTween.Sequence()
                    .Append(((RectTransform)this.arrowPanel.transform).DOAnchorPosY(10, 0.5f).From(isRelative: true))
                    .Append(((RectTransform)this.arrowPanel.transform).DOAnchorPosY(-10, 0.5f).From(isRelative: true))
                    .SetLoops(-1)
                    .SetAutoKill(false)
                    .Pause();
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
                if (i == index) {
                    parent.SetActive(true);
                } else {
                    parent.SetActive(false);
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
            this.Close();
            this.ChangeSkillBookInCurrentDeck(index);
        }
        #endregion

        public void Open(InventorySkillBook inventorySkillBook) {
            this.ShowSkillBook(inventorySkillBook);
            this.arrowPanelSequence.Restart();
            this.popup.SetActive(true);
        }

        void Close() {
            this.soundManager.Close();
            this.arrowPanelSequence.Pause();
            this.popup.SetActive(false);
        }

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
                GameObject prefab = this.GetDeckSkillBookPrefab(inventorySkillBook.skillBook.grade);
                GameObject clone = Instantiate(prefab, parent.transform);
                DeckSkillBook skillBook = clone.GetComponent<DeckSkillBook>();
                skillBook.Initialize(inventorySkillBook, (inventorySkillBook) => this.EquipSkillBook(inventorySkillBook));
                skillBooks[skillBookId] = skillBook;
                skillBookIds.Add(skillBookId);
            }
            this.skillBooksByDeck[index] = skillBooks;
            this.skillBookIdsByDeck[index] = skillBookIds.ToArray();
            if (deck.isUse) {
                parent.SetActive(true);
            } else {
                parent.SetActive(false);
            }
        }

        GameObject GetDeckSkillBookPrefab(SkillBookGrade skillBookGrade) {
            GameObject prefab = null;
            switch (skillBookGrade) {
                case SkillBookGrade.Common:
                    prefab = this.commonDeckSkillBookPrefab;
                    break;
                case SkillBookGrade.Uncommon:
                    prefab = this.uncommonDeckSkillBookPrefab;
                    break;
                case SkillBookGrade.Rare:   
                    prefab = this.rareDeckSkillBookPrefab;
                    break;
                case SkillBookGrade.Epic:
                    prefab = this.epicDeckSkillBookPrefab;
                    break;
                default:
                    prefab = this.commonDeckSkillBookPrefab;
                    break;
            }
            return prefab;
        }

        void ShowSkillBook(InventorySkillBook inventorySkillBook) {
            for (int i = 0; i < this.skillBookPanel.transform.childCount; i++) {
                GameObject children = this.skillBookPanel.transform.GetChild(i).gameObject;
                Destroy(children);
            }
            GameObject prefab = this.GetObtainedSkillBookPrefab(inventorySkillBook.skillBook.grade);
            GameObject clone = Instantiate(prefab, this.skillBookPanel.transform);
            ObtainedSkillBook skillBook = clone.GetComponent<ObtainedSkillBook>();
            skillBook.Initialize(inventorySkillBook, (_) => {});
            this.selectedSkillBook = inventorySkillBook;
        }

        GameObject GetObtainedSkillBookPrefab(SkillBookGrade skillBookGrade) {
            GameObject prefab = null;
            switch (skillBookGrade) {
                case SkillBookGrade.Common:
                    prefab = this.commonObtainedSkillBookPrefab;
                    break;
                case SkillBookGrade.Uncommon:
                    prefab = this.uncommonObtainedSkillBookPrefab;
                    break;
                case SkillBookGrade.Rare:   
                    prefab = this.rareObtainedSkillBookPrefab;
                    break;
                case SkillBookGrade.Epic:
                    prefab = this.epicObtainedSkillBookPrefab;
                    break;
                default:
                    prefab = this.commonObtainedSkillBookPrefab;
                    break;
            }
            return prefab;
        }

        void EquipSkillBook(InventorySkillBook inventorySkillBook) {
            var currentDeck = Array.Find(this.userModel.user.decks, (e) => e.isUse == true);
            int index = Array.FindIndex(currentDeck.skillBooks, (e) => e == inventorySkillBook.skillBook.id);
            this.loadingPopup.Open();
            this.userController.EquipSkillBook(index, this.selectedSkillBook.skillBook.id);
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
            GameObject prefab = this.GetDeckSkillBookPrefab(this.selectedSkillBook.skillBook.grade);
            GameObject parent = this.deckPanels[currentDeckIndex];
            GameObject clone = Instantiate(prefab, parent.transform);
            clone.transform.SetSiblingIndex(index);
            DeckSkillBook skillBook = clone.GetComponent<DeckSkillBook>();
            skillBook.Initialize(this.selectedSkillBook, (inventorySkillBook) => this.EquipSkillBook(inventorySkillBook));

            skillBookIds[index] = newSkillBookId;
            skillBooks.Add(newSkillBookId, skillBook);
            skillBooks.Remove(prevSkillBookId);
        }
    }
}
