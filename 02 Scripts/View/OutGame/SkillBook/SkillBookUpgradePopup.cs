using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;

    public class SkillBookUpgradePopup : MonoBehaviour
    {
        [Header("Audio")]
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
        GameObject[] skillsPanel;
        [SerializeField]
        Stat[] stats;
        [SerializeField]
        Button closeButton;

        Sequence sequence;

        #region Unity Method
        void Awake() {
            this.closeButton.onClick.AddListener(this.Close);
            Sequence skillsSequence = DOTween.Sequence();
            foreach (var skillPanel in this.skillsPanel) {
                CanvasGroup canvasGroup = skillPanel.GetComponent<CanvasGroup>();
                skillsSequence.Append(
                    DOTween.Sequence()
                        .PrependCallback(() => {
                            this.audioSource.clip = this.statsSfx;
                            this.audioSource.Play();
                        })
                        .Join(((RectTransform)skillPanel.transform).DOAnchorPosY(0, 0.3f).From(new Vector2(0, 100), isRelative: true))
                        .Join(canvasGroup.DOFade(1, 0.3f).From(0))
                );
            }
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
                .Append(skillsSequence)
                .Pause();
        }
        #endregion

        public void Open(InventorySkillBook inventorySkillBook) {
            this.nameText.text = inventorySkillBook.skillBook.name;
            this.iconImage.sprite = inventorySkillBook.skillBook.image;
            this.prevLevelText.text = (inventorySkillBook.level - 1).ToString();
            this.nextLevelText.text = inventorySkillBook.level.ToString();
            var skills = inventorySkillBook.skillBook.skills;
            for (int i = 0; i < skills.Length; i++) {
                var skill = skills[i];
                Stat stat = this.stats[i];
                float damage = skill.spec.damage + (skill.spec.damagePerLevel * (inventorySkillBook.level - 1));
                stat.Initialize($"ATK * {damage} <color=green>+{skill.spec.damagePerLevel}</color>");
            }

            this.popup.SetActive(true);
            this.sequence.Restart();
        }

        void Close() {
            this.soundManager.Close();
            this.sequence.Complete();
            this.popup.SetActive(false);
        }
    }
}
