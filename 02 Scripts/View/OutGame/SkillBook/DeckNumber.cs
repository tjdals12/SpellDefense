using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.OutGame.SkillBook {
    public class DeckNumber : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image enableImage;
        [SerializeField]
        Image disableImage;

        public void Initialize(Action onClick) {
            this.button.enabled = this.disableImage.gameObject.activeSelf;
            this.button.onClick.AddListener(() => {
                onClick?.Invoke();
            });
        }

        public void Enable() {
            this.button.enabled = false;
            this.enableImage.gameObject.SetActive(true);
            this.disableImage.gameObject.SetActive(false);
        }

        public void Disable() {
            this.button.enabled = true;
            this.enableImage.gameObject.SetActive(false);
            this.disableImage.gameObject.SetActive(true);
        }
    }
}
