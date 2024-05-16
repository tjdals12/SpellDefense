using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Repository.SkillBookShop {
    public class DataTableRepository : IRepository
    {
        #region Unity Method
        void Awake() {
            this.skillBookShops = this.Load();
            var obj = GameObject.FindObjectsOfType<IRepository>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        #endregion

        Dictionary<int, Entity[]> Load() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/SkillBookShop");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "SkillBookShop.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            Dictionary<int, List<Entity>> items = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                Builder builder = new Builder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "index") {
                        builder.SetIndex(int.Parse(value));
                    } else if (name == "itemIds") {
                        string[] rangesList = value.Split(",");
                        List<int> itemIds = new();
                        foreach (string ranges in rangesList) {
                            string[] range = ranges.Split("-");
                            if (range.Length == 1) {
                                itemIds.Add(int.Parse(range[0]));
                            } else {
                                int start = int.Parse(range[0]);
                                int end = (int.Parse(range[1]) - start) + 1;
                                itemIds.AddRange(Enumerable.Range(start, end));
                            }
                        }
                        builder.SetItemIds(itemIds.ToArray());
                    } else if (name == "itemAmount") {
                        builder.SetItemAmount(int.Parse(value));
                    } else if (name == "costId") {
                        builder.SetCostId(int.Parse(value));
                    } else if (name == "costAmount") {
                        builder.SetCostAmount(int.Parse(value));
                    } else if (name == "probability") {
                        builder.SetProbability(float.Parse(value));
                    }
                }
                Entity item = builder.Build();
                if (items.ContainsKey(item.index)) {
                    items[item.index].Add(item);
                } else {
                    items[item.index] = new() { item };
                }
            }
            Dictionary<int, Entity[]> newItems = new();
            foreach (var kv in items) {
                List<Entity> value = kv.Value;
                value.Sort((a, b) => a.probability > b.probability ? 1 : -1);
                Entity[] newValue = new Entity[value.Count];
                for (int i = 0; i < value.Count; i++) {
                    Entity current = value[i];
                    float accumulatedProbability = current.probability;
                    if (i > 0) {
                        Entity prev = newValue[i - 1];
                        accumulatedProbability += prev.probability;
                    }
                    newValue[i] = new Entity(
                        index: current.index,
                        itemIds: current.itemIds,
                        itemAmount: current.itemAmount,
                        costId: current.costId,
                        costAmount: current.costAmount,
                        probability: accumulatedProbability
                    );
                }
                newItems[kv.Key] = newValue;
            }
            return newItems;
        }

        public override Entity[] FindAllByIndex(int index)
        {
            return this.skillBookShops[index];
        }
    }
}