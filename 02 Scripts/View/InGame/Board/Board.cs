using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.InGame.Board {
    using BoardModel = Model.InGame.BoardModel;    
    using InGameController = Controller.InGame.InGameController;
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;
    using SpawnedSkillBook = Model.InGame.SpawnedSkillBook;

    public class Board : MonoBehaviour
    {
        BoardModel boardModel;
        InGameController inGameController;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        TextMeshProUGUI currentSpText;
        [SerializeField]
        TextMeshProUGUI requiredSpText;
        [SerializeField]
        Button spawnButton;
        [SerializeField]
        GameObject[] slotPanels;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject commonSkillBook;
        [SerializeField]
        GameObject uncommonSkillBook;
        [SerializeField]
        GameObject rareSkillBook;
        [SerializeField]
        GameObject epicSkillBook;

        Dictionary<int, BoardSkillBook> skillBooks;
        Dictionary<int, int> skillBookIds;

        #region Unity Method
        void Awake() {
            this.boardModel = GameObject.FindObjectOfType<BoardModel>();
            this.inGameController = GameObject.FindObjectOfType<InGameController>();
            this.skillBooks = new();
            this.skillBookIds = new();
        }
        void OnEnable() {
            this.boardModel.OnInitialize += this.OnInitialize;
            this.boardModel.OnChangeCurrentSp += this.OnChangeCurrentSp;
            this.boardModel.OnChangeRequiredSp += this.OnChangeRequiredSp;
            this.boardModel.OnSpawnSkillBook += this.OnSpawnSkillBook;
            this.boardModel.OnMergeSkillBook += this.OnMergeSkillBook;
            this.boardModel.OnCompleteCastingSkillBook += this.OnCompleteCastingSkillBook;
            this.boardModel.OnUpgradeSkillBook += this.OnUpgradeSkillBook;
        }
        void OnDisable() {
            this.boardModel.OnInitialize -= this.OnInitialize;
            this.boardModel.OnChangeCurrentSp -= this.OnChangeCurrentSp;
            this.boardModel.OnChangeRequiredSp -= this.OnChangeRequiredSp;
            this.boardModel.OnSpawnSkillBook -= this.OnSpawnSkillBook;
            this.boardModel.OnMergeSkillBook -= this.OnMergeSkillBook;
            this.boardModel.OnCompleteCastingSkillBook -= this.OnCompleteCastingSkillBook;
            this.boardModel.OnUpgradeSkillBook -= this.OnUpgradeSkillBook;
        }
        #endregion

        #region Event Listeners
        void OnInitialize() {
            this.currentSpText.text = $"<color=#1CC5FC>SP</color> {this.boardModel.currentSp.ToString()}";
            this.requiredSpText.text = this.boardModel.requiredSp.ToString();
            this.spawnButton.onClick.RemoveAllListeners();
            this.spawnButton.onClick.AddListener(this.SpawnSkillBook);
        }
        void OnChangeCurrentSp() {
            this.currentSpText.text = $"<color=#1CC5FC>SP</color> {this.boardModel.currentSp.ToString()}";
        }
        void OnChangeRequiredSp() {
            this.requiredSpText.text = this.boardModel.requiredSp.ToString();
        }
        void OnSpawnSkillBook(int slot) {
            this.spawnButton.enabled = true;
            this.CreateSkillBook(slot);
        }
        void OnMergeSkillBook(int slot1, int slot2) {
            this.spawnButton.enabled = true;
            this.ChangeSkillBook(slot1);
            this.DeleteSkillBook(slot2);
        }
        void OnCompleteCastingSkillBook(int slot) {
            this.CompleteCastingSkillBook(slot);
        }
        void OnUpgradeSkillBook(int skillBookId) {
            this.UpgradeSkillBook(skillBookId);
        }
        #endregion

        void SpawnSkillBook() {
            this.soundManager.Click();
            if (this.boardModel.HasRemainingSlot() && this.boardModel.HasEnoughSp()) {
                this.spawnButton.enabled = false;
                this.inGameController.SpawnSkillBook();
            }
        }
 
        void CreateSkillBook(int slot) {
            SpawnedSkillBook spawnedSkillBook = this.boardModel.usingSlots[slot];
            GameObject prefab = this.GetPrefab(spawnedSkillBook.grade);
            GameObject slotPanel = this.slotPanels[slot];
            GameObject clone = Instantiate(prefab, slotPanel.transform);
            BoardSkillBook boardSkillBook = clone.GetComponent<BoardSkillBook>();
            boardSkillBook.Initialize(slot, spawnedSkillBook, this.CastSkillBook, this.MergeSkillBook);
            clone.transform
                .DOScale(1, 0.2f)
                .OnStart(() => {
                    this.soundManager.Spawn();
                })
                .From(0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    clone.transform.localScale = Vector3.one;
                });
            this.skillBooks[slot] = boardSkillBook;
            this.skillBookIds[slot] = spawnedSkillBook.id;
        }

        void DeleteSkillBook(int slot) {
            if (this.skillBooks.ContainsKey(slot)) {
                Destroy(this.skillBooks[slot].gameObject);
                this.skillBooks.Remove(slot);
                this.skillBookIds.Remove(slot);
            }
        }

        void ChangeSkillBook(int slot) {
            this.DeleteSkillBook(slot);
            this.CreateSkillBook(slot);
        }

        void CompleteCastingSkillBook(int slot) {
            if (this.skillBooks.ContainsKey(slot)) {
                this.skillBooks[slot].StartCoolTime();
            }
        }

        void CastSkillBook(int slot) {
            this.inGameController.CastSkillBook(slot);
        }

        void MergeSkillBook(int slot1, int slot2) {
            if (this.boardModel.CanMerge(slot1, slot2)) {
                this.spawnButton.enabled = false;
                this.inGameController.MergeSkillBook(slot1, slot2);
            }
        }

        void UpgradeSkillBook(int skillBookId) {
            foreach (var kv in this.skillBookIds) {
                if (kv.Value == skillBookId) {
                    this.soundManager.Upgrade();
                    this.skillBooks[kv.Key].LevelUp();
                }
            }
        }

        GameObject GetPrefab(SkillBookGrade skillBookGrade) {
            switch (skillBookGrade) {
                case SkillBookGrade.Common:
                    return this.commonSkillBook;
                case SkillBookGrade.Uncommon:
                    return this.uncommonSkillBook;
                case SkillBookGrade.Rare:
                    return this.rareSkillBook;
                case SkillBookGrade.Epic:
                    return this.epicSkillBook;
                default:
                    return this.commonSkillBook;
            }
        }
    }
}