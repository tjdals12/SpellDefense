using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Setting {
    public class Entity
    {
        public bool BGM;
        public bool VFX;
        public Entity(bool BGM, bool VFX) {
            this.BGM = BGM;
            this.VFX = VFX;
        }
    }
}