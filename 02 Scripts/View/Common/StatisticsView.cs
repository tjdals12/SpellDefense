using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

namespace View.Common {
    public class StatisticsView : MonoBehaviour
    {
        [SerializeField]
        bool isShow;
        [SerializeField]
        int fontSize;

        ProfilerRecorder drawCallsRecorder;
        ProfilerRecorder setPassCallsRecorder;
        ProfilerRecorder totalMemoryRecorder;
        ProfilerRecorder systemMemoryRecorder;
        ProfilerRecorder gcMemoryRecorder;

        float deltaTime;

        #region Unity Method
        void OnEnable() {
            this.drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            this.setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            this.totalMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
            this.systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            this.gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        }
        void OnDisable() {
            this.drawCallsRecorder.Dispose();
            this.setPassCallsRecorder.Dispose();
            this.totalMemoryRecorder.Dispose();
            this.systemMemoryRecorder.Dispose();
            this.gcMemoryRecorder.Dispose();
        }
        void OnGUI() {
            if (this.isShow == false) return;
            StringBuilder sb = new StringBuilder(500);
            if (this.drawCallsRecorder.Valid) {
                sb.AppendLine($"Draw Calls: {this.drawCallsRecorder.LastValue}");
            }
            if (this.setPassCallsRecorder.Valid) {
                sb.AppendLine($"SetPass Calls: {this.setPassCallsRecorder.LastValue}");
            }
            sb.AppendLine($"Total Used Memory: {this.totalMemoryRecorder.LastValue / (1024 * 1024)} MB");
            sb.AppendLine($"System Used Memory: {this.systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
            sb.AppendLine($"GC Reserved Memory: {this.gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
            float widthRatio = Screen.width / 1080f;
            float heightRatio = Screen.height / 1920f;
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = Mathf.RoundToInt(this.fontSize * widthRatio);
            style.normal.textColor = Color.white;
            Rect rect = new Rect(30 * widthRatio, 80 * heightRatio, 500 * widthRatio, 500 * heightRatio);
            GUI.Label(rect, sb.ToString(), style);
        }
        #endregion
    }
}
