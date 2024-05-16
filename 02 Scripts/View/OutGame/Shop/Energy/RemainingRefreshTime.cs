using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace View.OutGame.Shop.Energy {
    using UserModel = Model.OutGame.UserModel;
    using ShopController = Controller.OutGame.ShopController;
    using LoadingPopup = Common.LoadingPopup;
    using ErrorPopup = Common.ErrorPopup;

    public class RemainingRefreshTime : MonoBehaviour
    {
        UserModel userModel;
        ShopController shopController;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        TextMeshProUGUI remainingHoursText;
        [SerializeField]
        GameObject hourText;
        [SerializeField]
        TextMeshProUGUI remainingMinutesText;
        [SerializeField]
        GameObject minuteText;
        [SerializeField]
        TextMeshProUGUI remainingSecondsText;
        [SerializeField]
        GameObject secondText;

        LoadingPopup loadingPopup;
        ErrorPopup errorPopup;

        Coroutine timerCoroutine;

        #region Unity Method
        void Awake() {
            this.userModel = GameObject.FindObjectOfType<UserModel>();
            this.shopController = GameObject.FindObjectOfType<ShopController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.errorPopup = GameObject.FindObjectOfType<ErrorPopup>();
            this.ShowRemainingTime();
        }
        void OnEnable() {
            this.userModel.OnResetPartShop += this.OnResetPartShop;
        }
        void OnDisable() {
            this.userModel.OnResetPartShop -= this.OnResetPartShop;
        }
        void OnApplicationPause(bool value) {
            if (value == true) {
                if (this.timerCoroutine != null) {
                    StopCoroutine(this.timerCoroutine);
                    this.timerCoroutine = null;
                }
            } else {
                this.timerCoroutine = StartCoroutine(this.StartTimer());
            }
        }
        #endregion

        #region Event Listeners
        void OnResetPartShop() {
            if (this.timerCoroutine != null) {
                StopCoroutine(this.timerCoroutine);
                this.timerCoroutine = null;
            }
            this.timerCoroutine = StartCoroutine(this.StartTimer());
        }
        #endregion

        void ShowRemainingTime() {
            if (this.timerCoroutine != null) {
                StopCoroutine(this.timerCoroutine);
                this.timerCoroutine = null;
            }
            this.timerCoroutine = StartCoroutine(this.StartTimer());
        }

        IEnumerator StartTimer() {
            yield return null;
            WaitForSeconds waitOneSeconds = new WaitForSeconds(1);
            WaitForSeconds waitOneMinutes = new WaitForSeconds(60);
            TimeSpan timeSpan = DateTime.UtcNow.AddDays(1).Date - DateTime.UtcNow;
            int remainingSeconds = Mathf.CeilToInt((float)timeSpan.TotalSeconds);
            while (remainingSeconds >= 0) {
                if (60 >= remainingSeconds) {
                    this.hourText.SetActive(false);
                    this.remainingHoursText.gameObject.SetActive(false);
                    this.minuteText.SetActive(false);
                    this.remainingMinutesText.gameObject.SetActive(false);
                    this.secondText.SetActive(true);
                    this.remainingSecondsText.text = remainingSeconds.ToString().PadLeft(2, '0');
                    this.remainingSecondsText.gameObject.SetActive(true);
                    remainingSeconds -= 1;
                    yield return waitOneSeconds;
                } else if (3600 >= remainingSeconds) {
                    this.hourText.SetActive(false);
                    this.remainingHoursText.gameObject.SetActive(false);
                    this.secondText.SetActive(false);
                    this.remainingSecondsText.gameObject.SetActive(false);
                    this.minuteText.SetActive(true);
                    int minutes = remainingSeconds / 60;
                    this.remainingMinutesText.text = minutes.ToString().PadLeft(2, '0');
                    this.remainingMinutesText.gameObject.SetActive(true);
                    remainingSeconds -= 60;
                    yield return waitOneMinutes;
                } else {
                    this.secondText.SetActive(false);
                    this.remainingSecondsText.gameObject.SetActive(false);
                    int totalMinutes = remainingSeconds / 60;
                    int hours = totalMinutes / 60;
                    int minutes = totalMinutes % 60;
                    this.hourText.SetActive(true);
                    this.remainingHoursText.text = hours.ToString().PadLeft(2, '0');
                    this.remainingHoursText.gameObject.SetActive(true);
                    this.minuteText.SetActive(true);
                    this.remainingMinutesText.text = minutes.ToString().PadLeft(2, '0');
                    this.remainingMinutesText.gameObject.SetActive(true);
                    remainingSeconds -= 60;
                    yield return waitOneMinutes;
                }
                timeSpan = DateTime.UtcNow.AddDays(1).Date - DateTime.UtcNow;
                remainingSeconds = Mathf.CeilToInt((float)timeSpan.TotalSeconds);
            }
            this.ResetEnergyShop();
        }

        void ResetEnergyShop() {
            if (this.timerCoroutine != null) {
                StopCoroutine(this.timerCoroutine);
                this.timerCoroutine = null;
            }
            this.loadingPopup.Open();
            this.shopController.ResetEnergyShop();
        }
    }
}
