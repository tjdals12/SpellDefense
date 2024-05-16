using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.EnergyShop {
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
            this.energyShops = this.Load();
        }
        #endregion

        Entity[] Load() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/EnergyShop");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "EnergyShop.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<Entity> items = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                Builder builder = new Builder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "itemId") {
                        builder.SetItemId(int.Parse(value));
                    } else if (name == "itemAmount") {
                        builder.SetItemAmount(int.Parse(value));
                    } else if (name == "costId") {
                        builder.SetCostId(int.Parse(value));
                    } else if (name == "costAmount") {
                        builder.SetCostAmount(int.Parse(value));
                    } else if (name == "buyCount") {
                        builder.SetBuyCount(int.Parse(value));
                    }
                }
                Entity item = builder.Build();
                items.Add(item);
            }
            return items.ToArray();
        }
    }
}
