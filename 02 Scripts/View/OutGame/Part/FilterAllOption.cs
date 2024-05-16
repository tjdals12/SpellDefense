using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Part {
    public class FilterAllOption : MonoBehaviour
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
        TextMeshProUGUI activeCountText;
        [SerializeField]
        TextMeshProUGUI deactiveCountText;
        [SerializeField]
        GameObject bar;

        public void Initialize(int count, Action onClick) {
            this.activeCountText.text = $"({count}/100)";
            this.deactiveCountText.text = $"({count}/100)";
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void UpdateView(int count) {
            this.activeCountText.text = $"({count}/100)";
            this.deactiveCountText.text = $"({count}/100)";
        }

        public void Activate() {
            this.button.enabled = false;
            this.activeIcon.SetActive(true);
            this.deactiveIcon.SetActive(false);
            this.activeCountText.gameObject.SetActive(true);
            this.deactiveCountText.gameObject.SetActive(false);
            this.bar.SetActive(true);
        }

        public void Deactivate() {
            this.button.enabled = true;
            this.activeIcon.SetActive(false);
            this.deactiveIcon.SetActive(true);
            this.activeCountText.gameObject.SetActive(false);
            this.deactiveCountText.gameObject.SetActive(true);
            this.bar.SetActive(false);
        }
    }
}
