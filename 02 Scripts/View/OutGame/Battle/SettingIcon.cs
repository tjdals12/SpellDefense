using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.OutGame.Battle {
    public class SettingIcon : MonoBehaviour
    {
        [Header("Audio")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        SettingPopup settingPopup;

        #region Unity Method
        void Awake() {
            this.button.onClick.AddListener(this.Open);
        }
        #endregion

        void Open() {
            this.soundManager.Click();
            this.settingPopup.Open();
        }
    }
}
