using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Setting {
    using Model.OutGame;

    public abstract class IRepository : MonoBehaviour
    {
        protected Entity setting;
        public abstract CurrentSetting GetCurrentSetting();
        public abstract void OnBGM();
        public abstract void OffBGM();
        public abstract void OnVFX();
        public abstract void OffVFX();
    }
}
