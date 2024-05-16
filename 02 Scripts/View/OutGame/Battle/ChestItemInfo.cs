using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.Battle {
    using ChestItem = Model.OutGame.ChestItem;

    public class ChestItemInfo : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI nameText;
        [SerializeField]
        TextMeshProUGUI amountText;
        [SerializeField]
        TextMeshProUGUI probabilityText;

        public void Initialize(ChestItem chestItem) {
            this.iconImage.sprite = chestItem.image;
            this.nameText.text = chestItem.name;
            this.amountText.text = $"x{chestItem.amount.ToString()}";
            this.probabilityText.text = $"{chestItem.probability}%";
        }
    }
}
