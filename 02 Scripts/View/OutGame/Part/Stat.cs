using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace View.OutGame.Part {
    public class Stat : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        TextMeshProUGUI valueText;

        public void Initialize(string text) {
            this.valueText.text = text;
        }
    }
}
