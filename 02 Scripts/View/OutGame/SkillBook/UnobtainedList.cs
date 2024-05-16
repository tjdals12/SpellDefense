using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.OutGame.SkillBook {
    using Model.OutGame;
    using Model.Common;
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;

    public class UnobtainedList : MonoBehaviour
    {
        IUserModel userModel;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject listPanel;

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

        Dictionary<int, UnobtainedSkillBook> skillBooks;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.ShowSkillBooks();
        }
        void OnEnable() {
            this.userModel.OnOpenChest += this.OnOpenChest;
        }
        void OnDisable() {
            this.userModel.OnOpenChest -= this.OnOpenChest;
        }
        #endregion

        #region Event Listeners
        void OnOpenChest(Chest chest, int count, List<RewardItem> rewardItems) {
            foreach (var rewardItem in rewardItems) {
                if (this.skillBooks.ContainsKey(rewardItem.item.id)) {
                    GameObject gameObject = this.skillBooks[rewardItem.item.id].gameObject;
                    Destroy(gameObject);
                    this.skillBooks.Remove(rewardItem.item.id);
                }
            }
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
                if (inventorySkillBook.isObtained) continue;
                this.CreateSkillBook(inventorySkillBook);
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

        void CreateSkillBook(IInventorySkillBook inventorySkillBook) {
            GameObject prefab = this.GetPrefab(inventorySkillBook.skillBook.grade);
            GameObject clone = Instantiate(prefab, this.listPanel.transform);
            UnobtainedSkillBook unobtainedSkillBook = clone.GetComponent<UnobtainedSkillBook>();
            unobtainedSkillBook.Initialize(inventorySkillBook);
            skillBooks[inventorySkillBook.skillBook.id] = unobtainedSkillBook;
        }
    }
}
