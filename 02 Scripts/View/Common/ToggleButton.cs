using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.Common {
    public class ToggleButton : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject enableImage;
        [SerializeField]
        GameObject disableImage;
        
        public void Initialize(Action onClick) {
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void Enable(bool withButton = true) {
            this.enableImage.SetActive(true);
            this.disableImage.SetActive(false);
            if (withButton) {
                this.button.enabled = true;
            }
        }
        public void Disable(bool withButton = true) {
            this.disableImage.SetActive(true);
            this.enableImage.SetActive(false);
            if (withButton) {
                this.button.enabled = false;
            }
        }
    }
}
