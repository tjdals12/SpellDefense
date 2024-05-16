using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetKits.ParticleImage;

namespace View.InGame.Skill {
    public class SkillAim : MonoBehaviour
    {
        [SerializeField]
        RectTransform rootCanvasTransform;
        [SerializeField]
        ParticleImage particleImage;
        [SerializeField]
        RectTransform attractor;

        Camera mainCamera;

        #region Unity Method
        void Awake() {
            this.mainCamera = Camera.main;
        }
        #endregion

        public void Aim(Vector3 worldPosition) {
            this.particleImage.Clear();
            Vector2 slotViewPoint = Camera.main.ScreenToViewportPoint(this.transform.position);
            Vector2 slotPosition = new Vector2(slotViewPoint.x * this.rootCanvasTransform.sizeDelta.x, slotViewPoint.y * this.rootCanvasTransform.sizeDelta.y);
            Vector2 viewPoint = Camera.main.WorldToViewportPoint(worldPosition);
            Vector2 position = new Vector2(viewPoint.x * this.rootCanvasTransform.sizeDelta.x, viewPoint.y * this.rootCanvasTransform.sizeDelta.y);
            Vector2 deltaPosition = new Vector2(
                x: position.x - slotPosition.x,
                y: position.y - slotPosition.y
            );
            this.attractor.anchoredPosition = deltaPosition;
            this.particleImage.Play();
        }
    }
}
