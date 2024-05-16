using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Repository.Chest {
    using LocalizationRepository = Repository.Localization.IRepository;
    using ItemRepository = Repository.Item.IRepository;
    using SO;
    using Model.OutGame;
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
            this.itemImageSO = Resources.Load<ItemImageSO>("SO/ItemImage");
            StartCoroutine(this.LoadAfterResolveDependnecies());
        }
        #endregion

        IEnumerator LoadAfterResolveDependnecies() {
            LocalizationRepository localizationRepository = GameObject.FindObjectOfType<LocalizationRepository>();
            ItemRepository itemRepository = GameObject.FindObjectOfType<ItemRepository>();
            while (itemRepository.isLoaded == false || localizationRepository == false) {
                yield return null;
            }
            Dictionary<int, ChestItemEntity[]> chestItemEntitiesDict = this.LoadChestItem(localizationRepository);
            ChestEntity[] chestEntities = this.LoadChest(localizationRepository);
            List<Chest> chests = new();
            foreach (var entity in chestEntities) {
                List<ChestItem> chestItems = new();
                if (chestItemEntitiesDict.ContainsKey(entity.id)) {
                    ChestItemEntity[] chestItemEntities = chestItemEntitiesDict[entity.id];
                    foreach (var chestItemEntity in chestItemEntities) {
                        Item[] items = Array.ConvertAll(chestItemEntity.itemIds, (itemId) => itemRepository.FindById(itemId));
                        ItemImage chestItemImage = this.itemImageSO.FindById(chestItemEntity.chestItemId);
                        ChestItem chestItem = new ChestItem(
                            chestId: chestItemEntity.chestId,
                            name: chestItemEntity.name,
                            image: chestItemImage.image,
                            items: items,
                            amount: chestItemEntity.itemAmount,
                            probability: chestItemEntity.probability
                        );
                        chestItems.Add(chestItem);
                    }
                }
                ItemImage chestImage = this.itemImageSO.FindById(entity.id);
                Item costItem = itemRepository.FindById(entity.costId);
                Cost cost = new Cost(costItem, entity.costAmount);
                Chest chest = new Chest(
                    id: entity.id,
                    name: entity.name,
                    image: chestImage.image,
                    cost: cost,
                    chestItems: chestItems.ToArray()
                );
                chests.Add(chest);
            }
            this.chests = chests.ToArray();
        }

        ChestEntity[] LoadChest(LocalizationRepository localizationRepository) {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/Chest");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "Chest.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<ChestEntity> items = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                ChestEntityBuilder builder = new ChestEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        builder.SetId(int.Parse(value));
                    } else if (name == "name") {
                        string localizedName = localizationRepository.GetLocalizedText(value);
                        builder.SetName(localizedName);
                    } else if (name == "costId") {
                        builder.SetCostId(int.Parse(value));
                    } else if (name == "costAmount") {
                        builder.SetCostAmount(int.Parse(value));
                    }
                }
                ChestEntity item = builder.Build();
                items.Add(item);
            }
            return items.ToArray();
        }

        Dictionary<int, ChestItemEntity[]> LoadChestItem(LocalizationRepository localizationRepository) {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/ChestItem");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "ChestItem.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            Dictionary<int, List<ChestItemEntity>> chestItems = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                ChestItemEntityBuilder builder = new ChestItemEntityBuilder();
                int? chestId = null;
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "chestId") {
                        chestId = int.Parse(value);
                    } else if (name == "chestItemId") {
                        builder.SetChestItemId(int.Parse(value));
                    } else if (name == "name") {
                        string localizedName = localizationRepository.GetLocalizedText(value);
                        builder.SetName(localizedName);
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
                                itemIds.AddRange(Enumerable.Range(start, end).ToArray());
                            }
                        }
                        builder.SetItemIds(itemIds.ToArray());
                    } else if (name == "itemAmount") {
                        builder.SetItemAmount(int.Parse(value));
                    } else if (name == "probability") {
                        builder.SetProbability(float.Parse(value));
                    }
                }
                if (chestId.HasValue) {
                    ChestItemEntity chestItem = builder.Build();
                    if (chestItems.ContainsKey(chestId.Value)) {
                        chestItems[chestId.Value].Add(chestItem);
                    } else {
                        chestItems[chestId.Value] = new() { chestItem };
                    }
                }
            }
            Dictionary<int, ChestItemEntity[]> dict = new();
            foreach (var kv in chestItems) {
                dict[kv.Key] = kv.Value.ToArray();
            }
            return dict;
        }

        public override Model.OutGame.Chest FindById(int id)
        {
            return Array.Find(this.chests, (e) => e.id == id);
        }
    }
}
