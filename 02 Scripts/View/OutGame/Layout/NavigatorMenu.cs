using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Layout {
    public class NavigatorMenu : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject menu;
        [SerializeField]
        Button button;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI menuNameText;

        RectTransform rectTransform;

        #region Unity Method
        void Awake() {
            this.rectTransform = this.menu.GetComponent<RectTransform>();
        }
        #endregion

        public void Initialize(Action onClick) {
            this.button.enabled = this.backgroundImage.gameObject.activeSelf ? false : true;
            this.button.onClick.AddListener(() => {
                onClick?.Invoke();
            });
        }

        public void SetPosition(Vector2 position) {
            this.rectTransform.anchoredPosition = position;
        }

        public void Select() {
            if (this.button.enabled == false) return;
            this.rectTransform.sizeDelta = new Vector2(360, 200);
            this.backgroundImage.gameObject.SetActive(true);
            this.iconImage.rectTransform.anchoredPosition = new Vector2(0, 40);
            this.iconImage.rectTransform.DOScale(1.5f, 0.1f);
            this.menuNameText.gameObject.SetActive(true);
            this.menuNameText.rectTransform.DOScale(1f, 0.1f).From(0);
            this.button.enabled = false;
        }

        public void Deselect() {
            if (this.button.enabled == true) return;
            this.rectTransform.sizeDelta = new Vector2(180, 200);
            this.backgroundImage.gameObject.SetActive(false);
            this.iconImage.rectTransform.anchoredPosition = new Vector2(0, 0);
            this.iconImage.rectTransform.DOScale(1f, 0.1f);
            this.menuNameText.gameObject.SetActive(false);
            this.button.enabled = true;
        }
    }
}
