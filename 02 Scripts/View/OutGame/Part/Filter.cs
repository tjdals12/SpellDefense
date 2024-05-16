using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.OutGame.Part {
    using UserModel = Model.OutGame.UserModel;

    public enum FilterType {
        All = 0,
        Weapon,
        Armor,
        Jewelry
    }

    public class Filter : MonoBehaviour
    {
        UserModel userModel;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        FilterAllOption filterAll;
        [SerializeField]
        FilterOption filterWeapon;
        [SerializeField]
        FilterOption filterArmor;
        [SerializeField]
        FilterOption filterJewelry;

        public event Action<FilterType> onChangeFilter;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            int count = this.userModel.user.inventoryParts.Count;
            this.filterAll.Initialize(count, this.ChangeToAll);
            this.filterWeapon.Initialize(this.ChangeToWeapon);
            this.filterArmor.Initialize(this.ChangeToArmor);
            this.filterJewelry.Initialize(this.ChangeToJewelry);
        }
        void OnEnable() {
            this.userModel.OnEarnPart += this.OnEarnPart;
            this.userModel.OnUpgradePart += this.OnUpgradePart;
        }
        void OnDisable() {
            this.userModel.OnEarnPart -= this.OnEarnPart;
            this.userModel.OnUpgradePart -= this.OnUpgradePart;
        }
        #endregion

        #region Event Listeners
        void OnEarnPart(string id) {
            int count = this.userModel.user.inventoryParts.Count;
            this.filterAll.UpdateView(count);
        }
        void OnUpgradePart(string id, string[] materialIds) {
            int count = this.userModel.user.inventoryParts.Count;
            this.filterAll.UpdateView(count);
        }
        #endregion

        void ChangeToAll() {
            this.soundManager.Click();
            this.filterAll.Activate();
            this.filterWeapon.Deactivate();
            this.filterArmor.Deactivate();
            this.filterJewelry.Deactivate();
            this.onChangeFilter?.Invoke(FilterType.All);
        }

        void ChangeToWeapon() {
            this.soundManager.Click();
            this.filterAll.Deactivate();
            this.filterWeapon.Activate();
            this.filterArmor.Deactivate();
            this.filterJewelry.Deactivate();
            this.onChangeFilter?.Invoke(FilterType.Weapon);
        }

        void ChangeToArmor() {
            this.soundManager.Click();
            this.filterAll.Deactivate();
            this.filterWeapon.Deactivate();
            this.filterArmor.Activate();
            this.filterJewelry.Deactivate();
            this.onChangeFilter?.Invoke(FilterType.Armor);
        }

        void ChangeToJewelry() {
            this.soundManager.Click();
            this.filterAll.Deactivate();
            this.filterWeapon.Deactivate();
            this.filterArmor.Deactivate();
            this.filterJewelry.Activate();
            this.onChangeFilter?.Invoke(FilterType.Jewelry);
        }
    }
}
