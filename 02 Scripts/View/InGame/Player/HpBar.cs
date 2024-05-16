using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.InGame.Player {
    public class HpBar : MonoBehaviour
    {
        [SerializeField]
        Image currentHpImage;
        [SerializeField]
        TextMeshProUGUI currentHpText;

        int maxHp;

        public void Initialize(int maxHp) {
            this.maxHp = maxHp;
            this.currentHpImage.fillAmount = 1;
            this.currentHpText.text = this.maxHp.ToString();
        }

        public void UpdateView(int currentHp) {
            this.currentHpImage.fillAmount = (float)currentHp / maxHp;
            this.currentHpText.text = currentHp.ToString();
        }
    }
}
