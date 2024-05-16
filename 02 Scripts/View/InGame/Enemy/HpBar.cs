using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.InGame.Enemy {
    public class HpBar : MonoBehaviour
    {
        [SerializeField]
        Image currentHpImage;

        Action callback;


        Camera mainCamera;
        Vector3 offsetPosition;
        Transform parent;

        #region Unity Method
        void Awake() {
            this.mainCamera = Camera.main;
            this.offsetPosition = new Vector3(0, 1.5f, 0);
        }
        void Update() {
            if (this.parent == null) return;
            Vector2 position = this.mainCamera.WorldToScreenPoint(this.parent.transform.position + offsetPosition);
            this.transform.position = position;
        }
        #endregion

        public void Initialize(Action callback) {
            this.callback = callback;
        }

        public void UpdateView(Transform parent, int maxHp, int currentHp) {
            this.parent = parent;
            float percent = (float)currentHp / maxHp;
            this.currentHpImage.fillAmount = 0.1f > percent ? 0.1f : percent;
            if (currentHp == 0) {
                this.callback?.Invoke();
            }
        }
    }
}
