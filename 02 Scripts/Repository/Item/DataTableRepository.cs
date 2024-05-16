using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Item {
    using LocalizationRespository = Repository.Localization.IRepository;
    using SO;
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
            LocalizationRespository localizationRespository = GameObject.FindObjectOfType<LocalizationRespository>();
            while (localizationRespository.isLoaded == false) {
                yield return null;
            }
            this.itemImageSO = Resources.Load<ItemImageSO>("SO/ItemImage");
            Entity[] entities = this.Load(localizationRespository);
            List<Item> items = new();
            foreach (var entity in entities) {
                Item item = new Item(
                    id: entity.id,
                    type: entity.type,
                    name: entity.name,
                    image: entity.image
                );
                items.Add(item);
            }
            this.items = items.ToArray();
            this.isLoaded = true;
        }

        Entity[] Load(LocalizationRespository localizationRespository) {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/Item");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "Item.csv");
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
                    if (name == "id") {
                        int id = int.Parse(value);
                        builder.SetId(id);
                        ItemImage itemImage = this.itemImageSO.FindById(id);
                        if (itemImage != null) {
                            builder.SetImage(itemImage.image);
                        }
                    } else if (name == "type") {
                        builder.SetType((ItemType)int.Parse(value));
                    } else if (name == "name") {
                        string localizedName = localizationRespository.GetLocalizedText(value);
                        builder.SetName(localizedName);
                    } else if (name == "description") {
                        string localizedDescription = localizationRespository.GetLocalizedText(value);
                        builder.SetDescription(localizedDescription);
                    }
                }
                Entity item = builder.Build();
                items.Add(item);
            }
            return items.ToArray();
        }

        public override Item FindById(int id)
        {
            return Array.Find(this.items, (e) => e.id == id);
        }

        public override Item[] FindAllByType(ItemType type)
        {
            return Array.FindAll(this.items, (e) => e.type == type);
        }
    }
}
