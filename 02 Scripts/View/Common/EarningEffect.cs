using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetKits.ParticleImage;

namespace View.Common {
    public class EarningEffect : MonoBehaviour
    {
        [SerializeField]
        ParticleImage particleImage;

        public void Initialize(Sprite image, Transform transform) {
            this.particleImage.attractorTarget = transform;
            this.particleImage.sprite = image;
            this.particleImage.Play();
        }
    }
}
