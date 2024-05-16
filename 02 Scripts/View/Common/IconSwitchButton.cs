using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.Common {
    public class IconSwitchButton : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject enableIcon;
        [SerializeField]
        GameObject disableIcon;
        [SerializeField]
        SwitchButton switchButton;

        public void Initialize(bool value, Action onClick) {
            if (value) {
                this.Enable();
            } else {
                this.Disable();
            }
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => {
                onClick?.Invoke();
            });
        }

        public void Enable() {
            this.enableIcon.SetActive(true);
            this.disableIcon.SetActive(false);
            this.switchButton.Enable();
        }

        public void Disable() {
            this.enableIcon.SetActive(false);
            this.disableIcon.SetActive(true);
            this.switchButton.Disable();
        }
    }
}
