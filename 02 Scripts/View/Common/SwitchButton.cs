using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View.Common {
    public class SwitchButton : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject enableIcon;
        [SerializeField]
        GameObject disableIcon;

        public void Enable() {
            this.enableIcon.SetActive(true);
            this.disableIcon.SetActive(false);
        }

        public void Disable() {
            this.enableIcon.SetActive(false);
            this.disableIcon.SetActive(true);
        }
    }
}
