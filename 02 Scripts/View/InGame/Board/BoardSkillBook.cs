using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

namespace View.InGame.Board {
    using SpawnedSkillBook = Model.InGame.SpawnedSkillBook;

    public class BoardSkillBook : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Image rootImage;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        GameObject[] starImages;
        [SerializeField]
        Image maskImage;
        [SerializeField]
        TextMeshProUGUI castedCountText;
        [SerializeField]
        Image levelUpIconImage;

        public int slot { get; private set; }
        Transform parent;
        Action<int> OnCast;
        Action<int, int> OnMerge;

        float elapsedTime;
        int coolTime;
        Coroutine coolTimeCoroutine;
        int castedCount;

        public void Initialize(int slot, SpawnedSkillBook spawnedSkillBook, Action<int> OnCast, Action<int, int> onMerge) {
            this.iconImage.sprite = spawnedSkillBook.image;
            for (int i = 0; i < this.starImages.Length; i++) {
                if (spawnedSkillBook.mergeCount >= i) {
                    this.starImages[i].SetActive(true);
                } else {
                    this.starImages[i].SetActive(false);
                }
            }
            this.castedCount = 0;
            this.castedCountText.text = this.castedCount.ToString();
            this.slot = slot;
            this.coolTime = spawnedSkillBook.skill.skillSpec.coolTime;
            this.OnCast = OnCast;
            this.OnMerge = onMerge;
            this.maskImage.gameObject.SetActive(false);
            this.OnCast?.Invoke(this.slot);
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

        public void StartCoolTime() {
            if (this.coolTimeCoroutine != null) {
                StopCoroutine(this.coolTimeCoroutine);
            }
            this.coolTimeCoroutine = StartCoroutine(this.StartTimer());
        }

        IEnumerator StartTimer() {
            WaitForSeconds delay = new WaitForSeconds(1);
            this.castedCount += 1;
            this.castedCountText.text = this.castedCount.ToString();
            this.elapsedTime = 0f;
            this.maskImage.gameObject.SetActive(true);
            this.maskImage.fillAmount = 1;
            while (this.coolTime > this.elapsedTime) {
                yield return delay;
                this.elapsedTime += 1;
                this.maskImage.fillAmount = 1 - (this.elapsedTime / this.coolTime);
            }
            this.maskImage.gameObject.SetActive(false);
            this.OnCast?.Invoke(this.slot);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.parent = this.transform.parent;
            this.transform.SetParent(this.transform.root);
            this.transform.SetAsLastSibling();
            this.rootImage.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.transform.SetParent(this.parent);
            this.rootImage.raycastTarget = true;
        }

        public void OnDrop(PointerEventData eventData)
        {
            BoardSkillBook currentSkillBook = eventData.pointerDrag.GetComponent<BoardSkillBook>();
            this.OnMerge?.Invoke(this.slot, currentSkillBook.slot);
        }
    }
}
