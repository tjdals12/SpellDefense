using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Wave {
    using Model.InGame;

    public abstract class IRepository : MonoBehaviour
    {
        public Wave[] waves;
        public abstract Wave FindByWave(int wave);
        public abstract Wave[] FindByWaveRange(int startWave, int endWave);
    }
}
