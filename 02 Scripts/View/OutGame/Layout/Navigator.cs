using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.OutGame.Layout {
    using ToastMessage = View.Common.ToastMessage;

    public class Navigator : MonoBehaviour
    {
        [Header("Audio")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI - Menu")]
        [Space(4)]
        [SerializeField]
        NavigatorMenu shopMenu;
        [SerializeField]
        NavigatorMenu skillBookMenu;
        [SerializeField]
        NavigatorMenu battleMenu;
        [SerializeField]
        NavigatorMenu partMenu;
        [SerializeField]
        NavigatorMenu masteryMenu;

        [Header("UI - Tab")]
        [Space(4)]
        [SerializeField]
        GameObject shopTab;
        [SerializeField]
        GameObject skillBookTab;
        [SerializeField]
        GameObject battleTab;
        [SerializeField]
        GameObject partTab;

        [Header("UI - Toast Message")]
        [Space(4)]
        [SerializeField]
        ToastMessage toBeUpdatedToastMessage;

        NavigatorMenu[] menus;
        GameObject[] contents;

        int currentIndex = 2;

        #region Unity Method
        void Awake() {
            this.menus = new NavigatorMenu[5] {
                this.shopMenu,
                this.skillBookMenu,
                this.battleMenu,
                this.partMenu,
                this.masteryMenu
            };
            for (int i = 0; i < this.menus.Length; i++) {
                int index = i;
                this.menus[i].Initialize(onClick: () => {
                    this.ChangeTab(index);
                });
            }
            this.contents = new GameObject[5] {
                this.shopTab,
                this.skillBookTab,
                this.battleTab,
                this.partTab,
                null
            };
        }
        #endregion

        void ChangeTab(int index) {
            this.soundManager.Click();
            if (this.contents[index] == null) {
                this.toBeUpdatedToastMessage.Open();
                return;
            }
            int positionX = 0;
            for (int i = 0; i < this.menus.Length; i++) {
                NavigatorMenu menu = this.menus[i];
                Vector2 position = new Vector2(positionX, 0);
                menu.SetPosition(position);
                if (i == index) {
                    menu.Select();
                    positionX += 360;
                } else {
                    menu.Deselect();
                    positionX += 180;
                }

                if (i == index) {
                    GameObject content = this.contents[i];
                    if (i > this.currentIndex) {
                        content.SetActive(true);
                        ((RectTransform)content.transform).anchoredPosition = new Vector3(1080, 0, 0);
                        ((RectTransform)content.transform).DOAnchorPosX(0, 0.1f);
                    } else if (this.currentIndex > i) {
                        content.SetActive(true);
                        ((RectTransform)content.transform).anchoredPosition = new Vector3(-1080, 0, 0);
                        ((RectTransform)content.transform).DOAnchorPosX(0, 0.1f);
                    }
                } else if (i == this.currentIndex) {
                    GameObject currentContent = this.contents[this.currentIndex];
                    if (index > this.currentIndex) {
                        ((RectTransform)currentContent.transform)
                            .DOAnchorPosX(-1080, 0.1f)
                            .OnComplete(() => {
                                currentContent.SetActive(false);
                            });
                    } else if (this.currentIndex > index) {
                        ((RectTransform)currentContent.transform)
                            .DOAnchorPosX(1080, 0.1f)
                            .OnComplete(() => {
                                currentContent.SetActive(false);
                            });
                    }
                }
            }
            this.currentIndex = index;
        }
    }
}
