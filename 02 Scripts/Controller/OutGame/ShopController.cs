using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using ValidationException = Core.CustomException.ValidationException;
    using UserService = Service.OutGame.UserService;

    public class ShopController : MonoBehaviour
    {
        UserService userService;

        public event Action OnSuccess;
        public event Action<string> OnAlert;
        public event Action OnError;

        #region Unity Method
        void Awake() {
            this.userService = GameObject.FindObjectOfType<UserService>();
        }
        #endregion

        public async void BuySkillBookInShop(int index) {
            try {
                await this.userService.BuySkillBookInShop(index);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ResetSkillBookShop() {
            try {
                await this.userService.ResetSkillBookShop();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ResetSkillBookShopByAds() {
            try {
                await this.userService.ResetSkillBookShopByAds();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void BuyPartInShop(int index) {
            try {
                await this.userService.BuyPartInShop(index);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ResetPartShop() {
            try {
                await this.userService.ResetPartShop();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ResetPartShopByAds() {
            try {
                await this.userService.ResetPartShopByAds();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void BuyGoldInShop(int index) {
            try {
                await this.userService.BuyGoldInShop(index);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ResetGoldShop() {
            try {
                await this.userService.ResetGoldShop();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void BuyEnergyInShop(int index) {
            try {
                await this.userService.BuyEnergyInShop(index);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void ResetEnergyShop() {
            try {
                await this.userService.ResetEnergyShop();
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }
    }
}
