using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Enemy {
    public class HpBarPool : MonoBehaviour
    {
        [SerializeField]
        Canvas canvas;
        [SerializeField]
        int poolCount;
        [SerializeField]
        GameObject prefab;

        Queue<HpBar> pool = new();

        #region Unity Method
        void Start() {
            this.Initialize();
        }
        #endregion

        void Initialize() {
            for (int i = 0; i < this.poolCount; i++) {
                HpBar hpBar = this.CreateObject();
                hpBar.gameObject.SetActive(false);
                this.pool.Enqueue(hpBar);
            }
        }

        HpBar CreateObject() {
            GameObject clone = Instantiate(this.prefab, this.canvas.transform);
            HpBar hpBar = clone.GetComponent<HpBar>();
            hpBar.Initialize(callback: () => this.ReturnObject(hpBar));
            return hpBar;
        }

        public HpBar GetObject() {
            HpBar hpBar = this.pool.Count > 0
                ? this.pool.Dequeue()
                : this.CreateObject();
            hpBar.gameObject.SetActive(true);
            return hpBar;
        }

        public void ReturnObject(HpBar hpBar) {
            hpBar.transform.position = this.transform.position;
            hpBar.gameObject.SetActive(false);
            this.pool.Enqueue(hpBar);
        }
    }
}
