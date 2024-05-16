using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.OutGame.Battle {
    using Chest = Model.OutGame.Chest;

    public class ChestOpeningPopup : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;
        [SerializeField]
        AudioClip chestSfx;
        [SerializeField]
        AudioClip openingSfx;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        Image foregroundImage;
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        GameObject window;
        [SerializeField]
        Image chestImage;
        [SerializeField]
        TextMeshProUGUI openCountText;

        Action onClick;

        Sequence waitingSequence;
        Sequence openingSequence;

        #region Unity Method
        void Awake() {
            this.waitingSequence = DOTween.Sequence()
                .Append(this.chestImage.transform.DOScale(1.2f, 1f))
                .Append(this.chestImage.transform.DOScale(1, 1f))
                .SetLoops(-1)
                .Pause();
            this.openingSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(
                    DOTween.Sequence()
                        .Append(this.chestImage.transform.DOScale(1, 0.3f).From(1.2f))
                        .Append(this.chestImage.transform.DOScale(1.2f, 0.3f).From(1))
                        .SetLoops(2)
                )
                .AppendCallback(() => {
                    this.audioSource.clip = this.openingSfx;
                    this.audioSource.Play();
                })
                .Append(this.chestImage.transform.DOScale(1.5f, 0.5f))
                .Join(this.foregroundImage.DOFade(1, 1f).From(0).SetEase(Ease.OutQuint))
                .OnComplete(() => {
                    this.Close();
                    this.onClick?.Invoke();
                })
                .Pause();
        }
        #endregion

        public void Open(Chest chest, int count, Action onClick) {
            this.chestImage.sprite = chest.image;
            this.openCountText.text = $"{chest.name} x {count}";
            this.popup.SetActive(true);
            DOTween.Sequence()
                .OnStart(() => {
                    this.audioSource.clip = this.chestSfx;
                    this.audioSource.Play();
                })
                .Join(this.backgroundImage.DOFade(1f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .OnComplete(() => {
                    this.waitingSequence.Restart();
                    this.onClick = onClick;
                    this.button.onClick.AddListener(this.Open);
                });
        }

        public void Close() {
            this.waitingSequence.Pause();
            this.openingSequence.Pause();
            this.chestImage.transform.localScale = Vector3.one;
            this.foregroundImage.color = new Color32(255, 255, 255, 0);
            this.popup.SetActive(false);
        }

        void Open() {
            this.waitingSequence.Pause();
            this.openingSequence.Restart();
            this.button.onClick.RemoveAllListeners();
        }
    }
}
