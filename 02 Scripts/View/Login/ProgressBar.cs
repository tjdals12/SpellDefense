using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.Login {
    public enum Step {
        LoadRemoteConfig = 0,
        CheckDataTableVersion,
        StartPatchDataTable,
        StepCompleteDataTable,
        CompletePatchDataTable,
        CheckAppVersion,
        TryAutoLogin,
    }

    public class ProgressBar : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject progressBar;
        [SerializeField]
        Image barImage;
        [SerializeField]
        TextMeshProUGUI percentText;
        [SerializeField]
        TextMeshProUGUI messageText;

        int currentPercent = 0;
        int targetPercent = 0;

        Coroutine lerpBarCoroutine;

        Dictionary<Step, string> englishMessages = new() {
            { Step.LoadRemoteConfig, "Loading app information..." },
            { Step.CheckDataTableVersion, "Checking data version..." },
            { Step.StartPatchDataTable, "Patching data..." },
            { Step.StepCompleteDataTable, "Patching data..." },
            { Step.CompletePatchDataTable, "Data patch complete..." },
            { Step.CheckAppVersion, "Checking app version..." },
            { Step.TryAutoLogin, "Checking login information..." },
        };
        Dictionary<Step, string> koreanMessages = new() {
            { Step.LoadRemoteConfig, "앱 정보를 불러오는 중..." },
            { Step.CheckDataTableVersion, "데이터 버전을 확인하는 중..." },
            { Step.StartPatchDataTable, "데이터 패치 진행 중..." },
            { Step.StepCompleteDataTable, "데이터 패치 진행 중..." },
            { Step.CompletePatchDataTable, "데이터 패치 완료..." },
            { Step.CheckAppVersion, "앱 버전을 확인하는 중..." },
            { Step.TryAutoLogin, "로그인 정보를 확인하는 중..." },
        };

        public void Open() {
            this.progressBar.SetActive(true);
            this.barImage.transform.localScale = new Vector3(0, 1, 1);
            this.percentText.text = "0%";
            this.messageText.gameObject.SetActive(false);
        }

        public void UpdateProgress(int percent, string message = null) {
            if (this.lerpBarCoroutine != null) {
                StopCoroutine(this.lerpBarCoroutine);
                this.currentPercent = this.targetPercent;
            }
            this.targetPercent = percent;
            this.lerpBarCoroutine = StartCoroutine(LerpBar());
            if (message == null) {
                this.messageText.gameObject.SetActive(false);
            } else {
                this.messageText.text = message;
                this.messageText.gameObject.SetActive(true);
            }
        }

        public void UpdateProgress(int percent, Step step, string message = null) {
            this.UpdateProgress(percent);
            this.messageText.gameObject.SetActive(true);
            string currentLanguage = Application.systemLanguage == SystemLanguage.Korean ? "ko" : "en";
            if (PlayerPrefs.HasKey("SettingLanguage")) {
                currentLanguage = PlayerPrefs.GetString("SettingLanguage");
            }
            if (currentLanguage == "ko") {
                this.messageText.text = message == null
                    ? this.koreanMessages[step]
                    : this.koreanMessages[step] + $" {message}";
            } else {
                this.messageText.text = message == null
                    ? this.englishMessages[step]
                    : this.englishMessages[step] + $" {message}";
            }
        }

        public void Close() {
            this.progressBar.SetActive(false);
        }

        IEnumerator LerpBar() {
            float elapsedTime = 0f;
            float duration = 0.5f;
            float value = 0f;
            yield return null;
            while (duration > elapsedTime) {
                elapsedTime += Time.deltaTime;
                value = Mathf.Floor(Mathf.Lerp(this.currentPercent, this.targetPercent, elapsedTime / duration));
                this.barImage.transform.localScale = new Vector3(value / 100, 1, 1);
                this.percentText.text = $"{value}%";
                yield return null;
            }
        }
    }
}
