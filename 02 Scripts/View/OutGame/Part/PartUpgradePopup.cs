using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Part {
    using InventoryPart = Model.OutGame.IInventoryPart;

    public class PartUpgradePopup : MonoBehaviour
    {
        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;
        [SerializeField]
        AudioSource audioSource;
        [SerializeField]
        AudioClip upgradeSfx;
        [SerializeField]
        AudioClip statsSfx;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        TextMeshProUGUI nameText;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI prevLevelText;
        [SerializeField]
        Image arrow;
        [SerializeField]
        TextMeshProUGUI nextLevelText;
        [SerializeField]
        GameObject statsPanel;
        [SerializeField]
        CanvasGroup statsPanelCanvasGroup;
        [SerializeField]
        Stat attackPowerStat;
        [SerializeField]
        Stat attackSpeedStat;
        [SerializeField]
        Stat criticalRateStat;
        [SerializeField]
        Stat criticalDamageStat;
        [SerializeField]
        Button closeButton;

        Sequence sequence;
        Sequence statsSequence;

        #region Unity Method
        void Awake() {
            this.closeButton.onClick.AddListener(this.Close);
            this.statsSequence = DOTween.Sequence()
                .PrependCallback(() => {
                    this.audioSource.clip = this.statsSfx;
                    this.audioSource.Play();
                })
                .Join(((RectTransform)this.statsPanel.transform).DOAnchorPosY(0, 0.3f).From(new Vector2(0, 100), isRelative: true))
                .Join(this.statsPanelCanvasGroup.DOFade(1, 0.3f).From(0));
            this.sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Join(this.backgroundImage.DOFade(1f, 0.1f).From(0))
                .Append(
                    DOTween.Sequence()
                        .PrependCallback(() => {
                            this.audioSource.clip = this.upgradeSfx;
                            this.audioSource.Play();
                        })
                        .Join(this.iconImage.transform.DOScale(1.2f, 0.2f).From(0).SetEase(Ease.OutBack))
                )
                .Append(this.iconImage.transform.DOScale(1, 0.1f))
                .Append(this.prevLevelText.transform.DOScale(1, 0.3f).From(0))
                .Append(
                    DOTween.Sequence()
                        .Join(this.arrow.rectTransform.DOAnchorPosX(0, 0.3f).From(new Vector2(-50, 0), isRelative: true))
                        .Join(this.arrow.DOFade(1, 0.3f).From(0))
                )
                .Append(this.nextLevelText.transform.DOScale(1.5f, 0.3f).From(0).SetEase(Ease.OutBack))
                .Append(this.nextLevelText.transform.DOScale(1, 0.1f))
                .Append(this.statsSequence)
                .Pause();
        }
        #endregion

        public void Open(InventoryPart inventoryPart, int upLevel) {
            this.ShowPart(inventoryPart);
            this.ShowLevel(inventoryPart, upLevel);
            this.ShowStats(inventoryPart, upLevel);
            this.popup.SetActive(true);
            this.sequence.Restart();
        }

        void Close() {
            this.soundManager.Close();
            this.sequence.Complete();
            this.popup.SetActive(false);
        }

        void ShowPart(InventoryPart inventoryPart) {
            this.nameText.text = inventoryPart.part.name;
            this.iconImage.sprite = inventoryPart.part.image;
        }

        void ShowLevel(InventoryPart inventoryPart, int upLevel) {
            this.prevLevelText.text = (inventoryPart.level - upLevel).ToString();
            this.nextLevelText.text = inventoryPart.level.ToString();
        }

        void ShowStats(InventoryPart inventoryPart, int upLevel) {
            int prevLevel = inventoryPart.level - upLevel;
            var spec = inventoryPart.part.spec;

            int attackPower = spec.attackPower + (spec.attackPowerPerLevel * prevLevel);
            int increasingAttackPower = spec.attackPowerPerLevel * upLevel;
            if (attackPower > 0 && increasingAttackPower > 0) {
                this.attackPowerStat.Initialize($"{attackPower.ToString()} <color=green>+{increasingAttackPower.ToString()}</color>");
                this.attackPowerStat.gameObject.SetActive(true);
            } else {
                this.attackPowerStat.gameObject.SetActive(false);
            }

            float attackSpeed = spec.attackSpeed + (spec.attackSpeedPerLevel * prevLevel);
            float increasingAttackSpeed = spec.attackSpeedPerLevel * upLevel;
            if (attackSpeed > 0 && increasingAttackSpeed > 0) {
                this.attackSpeedStat.Initialize($"{attackSpeed.ToString("0.00")} <color=green>+{increasingAttackSpeed.ToString("0.00")}</color>");
                this.attackSpeedStat.gameObject.SetActive(true);
            } else {
                this.attackSpeedStat.gameObject.SetActive(false);
            }

            int criticalRate = spec.criticalRate + (spec.criticalRatePerLevel * prevLevel);
            int increasingCriticalRate = spec.criticalRatePerLevel * upLevel;
            if (criticalRate > 0 && increasingCriticalRate > 0) {
                this.criticalRateStat.Initialize($"{criticalRate}% <color=green>+{increasingCriticalRate}%</color>");
                this.criticalRateStat.gameObject.SetActive(true);
            } else {
                this.criticalRateStat.gameObject.SetActive(false);
            }

            int criticalDamage = spec.criticalDamage + (spec.criticalDamagePerLevel * prevLevel);
            int increasingCriticalDamage = spec.criticalDamagePerLevel * upLevel;
            if (criticalDamage > 0 && increasingCriticalDamage > 0) {
                this.criticalDamageStat.Initialize($"{criticalDamage}% <color=green>+{increasingCriticalDamage}%</color>");
                this.criticalDamageStat.gameObject.SetActive(true);
            } else {
                this.criticalDamageStat.gameObject.SetActive(false);
            }
        }
    }
}
