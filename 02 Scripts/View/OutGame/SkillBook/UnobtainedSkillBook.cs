using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace View.OutGame.SkillBook {
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;

    public class UnobtainedSkillBook : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TextMeshProUGUI gradeText;

        public void Initialize(InventorySkillBook inventorySkillBook) {
            this.iconImage.sprite = inventorySkillBook.skillBook.image;
            this.gradeText.text = inventorySkillBook.skillBook.gradeName;
        }
    }
}
