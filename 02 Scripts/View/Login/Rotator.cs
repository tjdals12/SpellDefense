using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View.Login {
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        float rotationSpeed;

        Vector3 angle;

        #region Unity Method
        void Awake() {
            this.angle = new Vector3(0f, 0f, this.rotationSpeed / 10f);
        }
        void Update() {
            this.transform.eulerAngles += angle;
        }
        #endregion
    }
}
