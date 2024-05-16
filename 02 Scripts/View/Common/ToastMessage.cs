using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace View.Common {
    public class ToastMessage : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        CanvasGroup canvasGroup;
        [SerializeField]
        RectTransform window;
        [SerializeField]
        float closeDelaySeconds;

        public bool isOpen {
            get => this.window.gameObject.activeSelf;
        }
        
        WaitForSeconds closeDelay;

        Coroutine closeCoroutine;

        #region Unity Method
        void Awake() {
            this.closeDelay = new WaitForSeconds(this.closeDelaySeconds);
        }
        #endregion

        public void Open() {
            if (this.window.gameObject.activeSelf == true) return;
            this.window.gameObject.SetActive(true);
            DOTween.Sequence()
                .Join(this.canvasGroup.DOFade(1, 0.3f).From(0))
                .Join(this.window.DOAnchorPosY(-200, 0.3f).From(new Vector2(0, -150)))
                .OnComplete(() => {
                    this.closeCoroutine = StartCoroutine(this.Close());
                });
        }

        IEnumerator Close() {
            yield return this.closeDelay;
            this.canvasGroup.DOFade(0, 0.5f).From(1).OnComplete(() => {
                this.window.gameObject.SetActive(false);
            });
        }
    }
}
