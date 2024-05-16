using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO {
    [Serializable]
    public class SkillPrefab {
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

    [CreateAssetMenu(fileName = "SkillPrefab", menuName = "SO/SkillPrefab")]
    public class SkillPrefabSO : ScriptableObject
    {
        [SerializeField]
        SkillPrefab[] skillPrefabs;

        #region Unity Method
        void OnValidate() {
            for (int i = 0; i < this.skillPrefabs.Length; i++) {
                var skillPrefab = this.skillPrefabs[i];
                skillPrefab.name = $"{skillPrefab.id}";
            }
        }
        #endregion

        public SkillPrefab FindById(int id) {
            return Array.Find(this.skillPrefabs, (e) => e.id == id);
        }
    }
}
