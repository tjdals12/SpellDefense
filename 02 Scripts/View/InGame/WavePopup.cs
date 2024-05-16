using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.InGame {
    using Common;

    public class WavePopup : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        CanvasGroup canvasGroup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        TextMeshProUGUI waveText;
        [SerializeField]
        LocalizationText waveLocalizationText;

        public void Open(int currentWaveNumber) {
            this.waveLocalizationText.UpdateView(currentWaveNumber);
            this.canvasGroup.alpha = 0;
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.canvasGroup.DOFade(1, 0.5f).From(0))
                .Join(this.backgroundImage.rectTransform.DOScaleY(1, 1f).From(0))
                .Join(this.waveText.rectTransform.DOScale(1, 0.5f).From(0))
                .Join(this.backgroundImage.rectTransform.DOScaleY(1.2f, 0.5f).From(1).SetDelay(1.5f))
                .Join(this.waveText.rectTransform.DOScale(1.2f, 0.5f).From(1))
                .Join(this.backgroundImage.DOFade(0, 0.5f).From(1))
                .Join(this.waveText.DOFade(0, 0.5f).From(1))
                .OnComplete(() => {
                    this.popup.SetActive(false);
                });
        }
    }
}
