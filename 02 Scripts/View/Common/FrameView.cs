using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.Common {
    public class FrameView : MonoBehaviour
    {
        [SerializeField]
        bool isShow;
        [SerializeField]
        int fontSize;

        float deltaTime;

        #region Unity Method
        void Awake() {
            Application.targetFrameRate = 60;
            var obj = GameObject.FindObjectsOfType<FrameView>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        void Update() {
            if (this.isShow == false) return;
            this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
        }
        void OnGUI() {
            if (this.isShow == false) return;
            float ms = this.deltaTime / 1000f;
            float fps = 1f / this.deltaTime;
            float widthRatio = Screen.width / 1080f;
            float heightRatio = Screen.height / 1920f;
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = Mathf.RoundToInt(this.fontSize * widthRatio);
            style.normal.textColor = Color.yellow;
            Rect rect = new Rect(30 * widthRatio, 30 * heightRatio, 500 * widthRatio, 100 * heightRatio);
            GUI.Label(rect, string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms), style);
        }
        #endregion
    }
}
