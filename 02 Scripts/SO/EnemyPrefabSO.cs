using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO {
    [Serializable]
    public class EnemyPrefab {
        [HideInInspector]
        public string name;
        [SerializeField]
        int _id;
        public int id {
            get { return this._id; }
            private set { this._id = value; }
        }
        [SerializeField]
        GameObject _prefab;
        public GameObject prefab {
            get { return this._prefab; }
            private set { this._prefab = value; }
        }
    }

    [CreateAssetMenu(fileName = "EnemyPrefab", menuName = "SO/EnemyPrefab")]
    public class EnemyPrefabSO : ScriptableObject
    {
        [SerializeField]
        EnemyPrefab[] enemyPrefabs;

        #region Unity Method
        void OnValidate() {
            for (int i = 0; i < this.enemyPrefabs.Length; i++) {
                var enemyPrefab = this.enemyPrefabs[i];
                enemyPrefab.name = $"{enemyPrefab.id}";
            }
        }
        #endregion

        public EnemyPrefab FindById(int id) {
            return Array.Find(this.enemyPrefabs, (e) => e.id == id);
        }
    }
}
