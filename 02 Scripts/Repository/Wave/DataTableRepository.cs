using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Wave {
    using ItemRepository = Repository.Item.IRepository;
    using EnemyRepository = Repository.Enemy.IRepository;
    using Model.InGame;
    using Model.Common;

    public class DataTableRepository : IRepository
    {
        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IRepository>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            StartCoroutine(this.LoadAfterResolveDependencies());
        }
        #endregion

        IEnumerator LoadAfterResolveDependencies() {
            ItemRepository itemRepository = GameObject.FindObjectOfType<ItemRepository>();
            EnemyRepository enemyRepository = GameObject.FindObjectOfType<EnemyRepository>();
            while (enemyRepository.isLoaded == false || itemRepository.isLoaded == false) {
                yield return null;
            }
            WaveEntity[] entities = this.Load();
            List<Wave> waves = new();
            foreach (var entity in entities) {
                List<SpawningEnemy> spawningEnemies = new();
                foreach (var spawningEnemyEntity in entity.spawningEnemies) {
                    Enemy enemy = enemyRepository.FindById(spawningEnemyEntity.id);
                    SpawningEnemy spawningEnemy = new SpawningEnemy(
                        enemy: enemy,
                        level: spawningEnemyEntity.level,
                        count: spawningEnemyEntity.count
                    );
                    spawningEnemies.Add(spawningEnemy);
                }
                List<RewardItem> clearRewards = new();
                foreach (var clearRewardEntity in entity.clearRewards) {
                    Item item = itemRepository.FindById(clearRewardEntity.itemId);
                    RewardItem clearReward = new RewardItem(
                        item: item,
                        amount: clearRewardEntity.itemAmount
                    );
                    clearRewards.Add(clearReward);
                }
                Wave wave = new Wave(
                    startWave: entity.startWave,
                    endWave: entity.endWave,
                    minCountPerSpawn: entity.minCountPerSpawn,
                    maxCountPerSpawn: entity.maxCountPerSpawn,
                    minSpawnDelay: entity.minSpawnDelay,
                    maxSpawnDelay: entity.maxSpawnDelay,
                    spawningEnemies: spawningEnemies.ToArray(),
                    clearRewards: clearRewards.ToArray()
                );
                waves.Add(wave);
            }
            this.waves = waves.ToArray();
        }

        WaveEntity[] Load() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/Wave");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "Wave.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<WaveEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                WaveEntityBuilder builder = new WaveEntityBuilder();
                List<SpawningEnemyEntity> spawnEnemies = new();
                List<ClearRewardEntity> clearRewards = new();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "startWave") {
                        builder.SetStartWave(int.Parse(value));
                    } else if (name == "endWave") {
                        builder.SetEndWave(int.Parse(value));
                    } else if (name == "minCountPerSpawn") {
                        builder.SetMinCountPerSpawn(int.Parse(value));
                    } else if (name == "maxCountPerSpawn") {
                        builder.SetMaxCountPerSpawn(int.Parse(value));
                    } else if (name == "minSpawnDelay") {
                        builder.SetMinSpawnDelay(float.Parse(value));
                    } else if (name == "maxSpawnDelay") {
                        builder.SetMaxSpawnDelay(float.Parse(value));
                    } else if (name == "enemy1" || name == "enemy2" || name == "enemy3" || name == "enemy4" || name == "enemy5") {
                        if (string.IsNullOrEmpty(value)) continue;
                        string[] values = value.Split("|");
                        SpawningEnemyEntity spawnEnemyEntity = new SpawningEnemyEntity(
                            id: int.Parse(values[0]),
                            count: int.Parse(values[1]),
                            level: int.Parse(values[2])
                        );
                        spawnEnemies.Add(spawnEnemyEntity);
                    } else if (name == "reward1" || name == "reward2" || name == "reward3") {
                        if (string.IsNullOrEmpty(value)) continue;
                        string[] values = value.Split("|");
                        ClearRewardEntity clearRewardEntity = new ClearRewardEntity(
                            itemId: int.Parse(values[0]),
                            itemAmount: int.Parse(values[1])
                        );
                        clearRewards.Add(clearRewardEntity);
                    }
                }
                WaveEntity waveEntity = builder
                    .SetSpawnEnemies(spawnEnemies.ToArray())
                    .SetClearRewards(clearRewards.ToArray())
                    .Build();
                list.Add(waveEntity);
            }
            return list.ToArray();
        }

        public override Model.InGame.Wave FindByWave(int wave)
        {
            return Array.Find(this.waves, (e) => wave >= e.startWave && e.endWave >= wave);
        }

        public override Model.InGame.Wave[] FindByWaveRange(int startWave, int endWave)
        {
            List<Model.InGame.Wave> waves = new();
            for (int currentWave = startWave; currentWave <= endWave; currentWave++) {
                Model.InGame.Wave wave = this.FindByWave(currentWave);
                if (wave == null) continue;
                waves.Add(wave);
            }
            return waves.ToArray();
        }
    }
}
