using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace View.InGame.UpgradeSlots {
    using IEquippedSkillBook = Model.InGame.IEquippedSkillBook;

    public class UpgradeSkillBook : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI levelText;
        [SerializeField]
        TextMeshProUGUI spText;
        [SerializeField]
        GameObject mask;
        [SerializeField]
        Image levelUpIconImage;

        public void Initialize(IEquippedSkillBook equippedSkillBook, Action onClick) {
            this.iconImage.sprite = equippedSkillBook.skillBook.image;
            var upgradeSpec = equippedSkillBook.GetCurrentUpgradeSpec();
            if (upgradeSpec != null) {
                this.levelText.text = $"Lv. {upgradeSpec.level}";
                this.spText.text = $"<color=#1CC5FC>SP</color> {upgradeSpec.requiredSp}";
            }
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void UpdateView(IEquippedSkillBook equippedSkillBook) {
            var upgradeSpec = equippedSkillBook.GetCurrentUpgradeSpec();
            if (upgradeSpec != null) {
                this.levelText.text = $"Lv. {upgradeSpec.level}";
                this.spText.text = $"<color=#1CC5FC>SP</color> {upgradeSpec.requiredSp}";
            } else {
                this.spText.text = $"<color=#FF3200>MAX</color>";
            }
        }

        public void LevelUp() {
            DOTween.Sequence()
                .OnStart(() => {
                    this.levelUpIconImage.gameObject.SetActive(true);
                })
                .Join(this.gameObject.transform.DOScale(1f, 0.3f).From(0).SetEase(Ease.OutBack))
                .Join(this.levelUpIconImage.rectTransform.DOAnchorPosY(80, 0.3f).From(Vector2.zero))
                .Join(this.levelUpIconImage.DOFade(0, 0.3f).From(1))
                .OnComplete(() => {
                    this.levelUpIconImage.gameObject.SetActive(false);
                });
        }

        public void Enable() {
            this.mask.SetActive(false);
            this.button.enabled = true;
        }

        public void Disable() {
            this.mask.SetActive(true);
            this.button.enabled = false;
        }
    }
}
