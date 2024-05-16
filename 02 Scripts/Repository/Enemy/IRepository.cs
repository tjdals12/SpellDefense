using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Enemy {
    using SO;
    using Model.InGame;

    public abstract class IRepository : MonoBehaviour
    {
        public bool isLoaded { get; protected set; }
        protected EnemyPrefabSO enemyPrefabSO;
        public Enemy[] enemies { get; protected set; }
        public abstract Enemy FindById(int id);
    }
}
