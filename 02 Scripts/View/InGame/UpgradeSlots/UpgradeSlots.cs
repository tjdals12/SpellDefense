using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.UpgradeSlots {
    using BoardModel = Model.InGame.BoardModel;
    using IEquippedSkillBook = Model.InGame.IEquippedSkillBook;
    using InGameController = Controller.InGame.InGameController;
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;

    public class UpgradeSlots : MonoBehaviour
    {
        BoardModel boardModel;
        InGameController inGameController;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject slotsPanel;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject commmonSkillBookPrefab;
        [SerializeField]
        GameObject uncommonSkillBookPrefab;
        [SerializeField]
        GameObject rareSkillBookPrefab;
        [SerializeField]
        GameObject epicSkillBookPrefab;

        Dictionary<int, UpgradeSkillBook> skillBooks;

        #region Unity Method
        void Awake() {
            this.boardModel = GameObject.FindObjectOfType<BoardModel>();
            this.inGameController = GameObject.FindObjectOfType<InGameController>();
        }
        void OnEnable() {
            this.boardModel.OnInitialize += this.OnInitialize;
            this.boardModel.OnChangeCurrentSp += this.OnChangeCurrentSp;
            this.boardModel.OnUpgradeSkillBook += this.OnUpgradeSkillBook;
        }
        void OnDisable() {
            this.boardModel.OnInitialize -= this.OnInitialize;
            this.boardModel.OnChangeCurrentSp -= this.OnChangeCurrentSp;
            this.boardModel.OnUpgradeSkillBook -= this.OnUpgradeSkillBook;
        }
        #endregion

        #region Event Listeners
        void OnInitialize() {
            this.ShowSkillBooks();
        }
        void OnChangeCurrentSp() {
            foreach (var equippedSkillBook in this.boardModel.skillBooks) {
                UpgradeSkillBook upgradeSkillBook = this.skillBooks[equippedSkillBook.skillBook.id];
                var upgradeSpec = equippedSkillBook.GetCurrentUpgradeSpec();
                if (upgradeSpec == null || upgradeSpec.requiredSp > this.boardModel.currentSp) {
                    upgradeSkillBook.Disable();
                } else {
                    upgradeSkillBook.Enable();
                }
            }
        }
        void OnUpgradeSkillBook(int skillBookId) {
            IEquippedSkillBook equippedSkillBook = Array.Find(this.boardModel.skillBooks, (e) => e.skillBook.id == skillBookId);
            this.UpdateSkillBook(equippedSkillBook);
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
            foreach (var equippedSkillBook in this.boardModel.skillBooks) {
                this.CreateSkillBook(equippedSkillBook);
            }
        }

        GameObject GetPrefab(SkillBookGrade skillBookGrade) {
            switch (skillBookGrade) {
                case SkillBookGrade.Common:
                    return this.commmonSkillBookPrefab;
                case SkillBookGrade.Uncommon:
                    return this.uncommonSkillBookPrefab;
                case SkillBookGrade.Rare:
                    return this.rareSkillBookPrefab;
                case SkillBookGrade.Epic:
                    return this.epicSkillBookPrefab;
                default:
                    return this.commmonSkillBookPrefab;
            }
        } 

        void CreateSkillBook(IEquippedSkillBook equippedSkillBook) {
            GameObject prefab = this.GetPrefab(equippedSkillBook.skillBook.grade);
            GameObject clone = Instantiate(prefab, this.slotsPanel.transform);
            UpgradeSkillBook upgradeSkillBook = clone.GetComponent<UpgradeSkillBook>();
            upgradeSkillBook.Initialize(equippedSkillBook, onClick: () => this.UpgradeSkillBook(equippedSkillBook));
            var upgradeSpec = equippedSkillBook.GetCurrentUpgradeSpec();
            if (upgradeSpec == null || upgradeSpec.requiredSp > this.boardModel.currentSp) {
                upgradeSkillBook.Disable();
            } else {
                upgradeSkillBook.Enable();
            }
            this.skillBooks[equippedSkillBook.skillBook.id] = upgradeSkillBook;
        }

        void UpdateSkillBook(IEquippedSkillBook equippedSkillBook) {
            UpgradeSkillBook upgradeSkillBook = this.skillBooks[equippedSkillBook.skillBook.id];
            upgradeSkillBook.UpdateView(equippedSkillBook);
            upgradeSkillBook.LevelUp();
        }

        void UpgradeSkillBook(IEquippedSkillBook equippedSkillBook) {
            this.skillBooks[equippedSkillBook.skillBook.id].Disable();
            this.inGameController.UpgradeSkillBook(equippedSkillBook.skillBook.id);
        }
    }
}