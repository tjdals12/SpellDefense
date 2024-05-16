using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.InGame.Enemy {
    using WaveRepository = Repository.Wave.IRepository;
    using Model.InGame;

    public class InstancePool : MonoBehaviour
    {
        PlayModel playModel;
        WaveRepository waveRepository;

        int groupingCount = 10;
        Dictionary<int, int> poolSizeByEnemy = new();
        Dictionary<int, GameObject> prefabByEnemy = new();
        Dictionary<int, Queue<Instance>> poolByEnemy = new();

        #region Unity Method
        void Awake() {
            this.playModel = GameObject.FindObjectOfType<PlayModel>();
            this.waveRepository = GameObject.FindObjectOfType<WaveRepository>();
        }
        #endregion

        public void Initialize() {
            if (this.playModel.currentWaveNumber % this.groupingCount > 1) return;

            foreach (var pool in this.poolByEnemy.Values) {
                while (pool.Count > 0) {
                    Instance instance = pool.Dequeue();
                    Destroy(instance.gameObject);
                }
            }

            int startWave = this.playModel.currentWaveNumber;
            int endWave = startWave + this.groupingCount;
            Wave[] waves = this.waveRepository.FindByWaveRange(startWave, endWave);
            foreach (var wave in waves) {
                SpawningEnemy[] spawningEnemies = wave.spawningEnemies;
                foreach (var spawningEnemy in spawningEnemies) {
                    Enemy enemy = spawningEnemy.enemy;
                    int count = spawningEnemy.count;
                    if (this.poolSizeByEnemy.ContainsKey(enemy.id)) {
                        this.poolSizeByEnemy[enemy.id] = Mathf.Max(this.poolSizeByEnemy[enemy.id], count);
                    } else {
                        this.poolSizeByEnemy.Add(enemy.id, count);
                        this.prefabByEnemy.Add(enemy.id, enemy.prefab);
                        this.poolByEnemy.Add(enemy.id, new Queue<Instance>());
                    }
                }
            }

            foreach (var kv in this.poolSizeByEnemy) {
                int enemyId = kv.Key;
                Queue<Instance> pool = this.poolByEnemy[enemyId];

                int poolSize = kv.Value;
                for (int i = 0; i < poolSize; i++) {
                    Instance instance = this.CreateObject(enemyId);
                    pool.Enqueue(instance);
                }
            }
        }

        Instance CreateObject(int enemyId) {
            GameObject prefab = this.prefabByEnemy[enemyId];
            GameObject clone = Instantiate(prefab);
            Instance instance = clone.GetComponent<Instance>();
            clone.SetActive(false);
            return instance;
        }

        public Instance GetObject(int enemyId) {
            Queue<Instance> pool = this.poolByEnemy[enemyId];
            Instance instance = pool.Count > 0
                ? pool.Dequeue()
                : this.CreateObject(enemyId);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void ReturnObject(int enemyId, Instance instance) {
            Queue<Instance> pool = this.poolByEnemy[enemyId];
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }
}
