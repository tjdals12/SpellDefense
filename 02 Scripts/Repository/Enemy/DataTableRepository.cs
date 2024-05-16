using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Enemy {
    using LocalizationRepository = Repository.Localization.IRepository;
    using SO;
    using Model.InGame;

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
            LocalizationRepository localizationRepository = GameObject.FindObjectOfType<LocalizationRepository>();
            while (localizationRepository.isLoaded == false) {
                yield return null;
            }
            this.enemyPrefabSO = Resources.Load<EnemyPrefabSO>("SO/EnemyPrefab");
            Entity[] entities = this.Load(localizationRepository);
            List<Enemy> enemies = new();
            foreach (var entity in entities) {
                EnemyPrefab enemyPrefab = this.enemyPrefabSO.FindById(entity.id);
                Enemy enemy = new Enemy(
                    id: entity.id,
                    name: entity.name,
                    hp: entity.hp,
                    hpPerLevel: entity.hpPerLevel,
                    attackPower: entity.attackPower,
                    attackPowerPerLevel: entity.attackPowerPerLevel,
                    attackSpeed: entity.attackSpeed,
                    moveSpeed: entity.moveSpeed,
                    dropSp: entity.dropSp,
                    prefab: enemyPrefab.prefab
                );
                enemies.Add(enemy);
            }
            this.enemies = enemies.ToArray();
            this.isLoaded = true;
        }

        Entity[] Load(LocalizationRepository localizationRepository) {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/Enemy");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "Enemy.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<Entity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                Builder builder = new Builder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        builder.SetId(int.Parse(value));
                    } else if (name == "name") {
                        string localizedName = localizationRepository.GetLocalizedText(value);
                        builder.SetName(localizedName);
                    } else if (name == "hp") {
                        builder.SetHp(int.Parse(value));
                    } else if (name == "hpPerLevel") {
                        builder.SetHpPerLevel(int.Parse(value));
                    } else if (name == "attackPower") {
                        builder.SetAttackPower(int.Parse(value));
                    } else if (name == "attackPowerPerLevel") {
                        builder.SetAttackPowerPerLevel(int.Parse(value));
                    } else if (name == "attackSpeed") {
                        builder.SetAttackSpeed(float.Parse(value));
                    } else if (name == "moveSpeed")  {
                        builder.SetMoveSpeed(float.Parse(value));
                    } else if (name == "dropSp") {
                        builder.SetDropSp(int.Parse(value));
                    }
                }
                Entity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }

        public override Enemy FindById(int id)
        {
            return Array.Find(this.enemies, (e) => e.id == id);
        }
    }
}
