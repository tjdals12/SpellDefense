using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.Common {
    using RewardItem = Model.Common.RewardItem;
    using ItemType = Repository.Item.ItemType;

    public class RewardPopup : MonoBehaviour
    {
        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        AudioSource audioSource;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Button button;
        [SerializeField]
        GameObject popup;
        [SerializeField]
        GameObject window;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        TextMeshProUGUI itemNameText;
        [SerializeField]
        Image itemIconImage;
        [SerializeField]
        TextMeshProUGUI itemAmountText;
        [SerializeField]
        TextMeshProUGUI rewardCountText;
        [SerializeField]
        TextMeshProUGUI tapToCloseText;
        [SerializeField]
        GameObject goldIcon;
        [SerializeField]
        GameObject energyIcon;
        [SerializeField]
        GameObject silverKeyIcon;
        [SerializeField]
        GameObject skillBookMenuIcon;
        [SerializeField]
        GameObject partMenuIcon;

        [Header("Effect")]
        [Space(4)]
        [SerializeField]
        GameObject goldAttraction;
        [SerializeField]
        GameObject energyAttraction;
        [SerializeField]
        GameObject keyAttraction;
        [SerializeField]
        GameObject skillBookAttraction;
        [SerializeField]
        GameObject partAttraction;

        #region Unity Method
        void Awake() {
            DOTween.Sequence()
                .Append(this.itemIconImage.transform.DOScale(0.8f, 1f))
                .Append(this.itemIconImage.transform.DOScale(1f, 1f))
                .SetLoops(-1);
            DOTween.Sequence()
                .Append(this.tapToCloseText.DOFade(0.5f, 1f))
                .Append(this.tapToCloseText.DOFade(0.8f, 1f))
                .SetLoops(-1);
        }
        #endregion

        public void Open(RewardItem rewardItem) {
            this.ShowReward(rewardItem);
            this.popup.SetActive(true);
            this.backgroundImage
                .DOFade(0.99f, 0.1f)
                .From(0);
        }

        public void Open(List<RewardItem> rewardItems) {
            StartCoroutine(this.ShowRewards(rewardItems));
            this.popup.SetActive(true);
            this.backgroundImage
                .DOFade(0.99f, 0.1f)
                .From(0);
        }

        void Close() {
            this.popup.SetActive(false);
        }

        void ShowReward(RewardItem rewardItem) {
            this.audioSource.Play();
            this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(this.Close);
            this.itemNameText.text = rewardItem.item.name;
            this.itemIconImage.sprite = rewardItem.item.image;
            this.itemAmountText.text = rewardItem.amount.ToString();
            this.PlayEarningEffect(rewardItem);
        }

        IEnumerator ShowRewards(List<RewardItem> rewardItems) {
            int currentIndex = 0;
            bool isWaiting = true;
            this.rewardCountText.gameObject.SetActive(true);
            this.tapToCloseText.gameObject.SetActive(false);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => {
                isWaiting = false;
            });
            while (rewardItems.Count > currentIndex) {
                this.audioSource.Play();
                this.rewardCountText.text = $"{currentIndex + 1} / {rewardItems.Count}";
                isWaiting = true;
                RewardItem rewardItem = rewardItems[currentIndex];
                this.itemNameText.text = rewardItem.item.name;
                this.itemIconImage.sprite = rewardItem.item.image;
                this.itemAmountText.text = rewardItem.amount.ToString();
                this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack);
                this.PlayEarningEffect(rewardItem);
                while (isWaiting) {
                    yield return null;
                }
                currentIndex++;
            }
            this.rewardCountText.gameObject.SetActive(false);
            this.tapToCloseText.gameObject.SetActive(true);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(this.Close);
            yield return null;
        }

        void PlayEarningEffect(RewardItem rewardItem) {
            GameObject prefab = null;
            Transform attractor = null;
            if (rewardItem.item.type == ItemType.Currency) {
                switch (rewardItem.item.id) {
                    // Gold
                    case 1:
                        prefab = this.goldAttraction;
                        attractor = this.goldIcon.transform;
                        break;
                    // Energy
                    case 3:
                        prefab = this.energyAttraction;
                        attractor = this.energyIcon.transform;
                        break;
                }
            } else if (rewardItem.item.type == ItemType.Key) {
                switch (rewardItem.item.id) {
                    // Silver Key
                    case 10:
                        prefab = this.keyAttraction;
                        attractor = this.silverKeyIcon.transform;
                        break;
                }
            } else if (rewardItem.item.type == ItemType.SkillBook) {
                prefab = this.skillBookAttraction;
                attractor = this.skillBookMenuIcon.transform;
            } else if (rewardItem.item.type == ItemType.Part) {
                prefab = this.partAttraction;
                attractor = this.partMenuIcon.transform;
            }
            GameObject clone = Instantiate(prefab, this.popup.transform.parent);
            EarningEffect earningEffect = clone.GetComponent<EarningEffect>();
            earningEffect.Initialize(rewardItem.item.image, attractor);
            if (clone != null) {
                Destroy(clone, 5);
            }
        }
    }
}
