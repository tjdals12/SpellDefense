using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using ValidationException = Core.CustomException.ValidationException;
    using UserService = Service.OutGame.UserService;

    public class UserController : MonoBehaviour
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

        public async void ChangeDeck(int index) {
            try {
                await this.userService.ChangeDeck(index);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void OpenSilverChest(int count) {
            try {
                await this.userService.OpenSilverChest(count);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void UpgradeSkillBook(int skillBookId) {
            try {
                await this.userService.UpgradeSkillBook(skillBookId);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void EquipSkillBook(int index, int skillBookId) {
            try {
                await this.userService.EquipSkillBook(index, skillBookId);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void UpgradePart(string id, string[] materialIds) {
            try {
                await this.userService.UpgradePart(id, materialIds);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void EquipPart(string id) {
            try {
                await this.userService.EquipPart(id);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }

        public async void GameStart() {
            try {
                await this.userService.GameStart();
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
