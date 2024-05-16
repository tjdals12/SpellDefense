using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.OutGame.Part {
    public class FilterOption : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject activeIcon;
        [SerializeField]
        GameObject deactiveIcon;
        [SerializeField]
        GameObject bar;

        public void Initialize(Action onClick) {
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void Activate() {
            this.button.enabled = false;
            this.activeIcon.SetActive(true);
            this.deactiveIcon.SetActive(false);
            this.bar.SetActive(true);
        }
        
        public void Deactivate() {
            this.button.enabled = true;
            this.activeIcon.SetActive(false);
            this.deactiveIcon.SetActive(true);
            this.bar.SetActive(false);
        }
    }
}
