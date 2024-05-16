using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Chest {
    public class ChestEntity {
        public int id { get; private set; }
        public string name { get; private set; }
        public int costId { get; private set; }
        public int costAmount { get; private set; }
        public ChestEntity(int id, string name, int costId, int costAmount) {
            this.id = id;
            this.name = name;
            this.costId = costId;
            this.costAmount = costAmount;
        }
    }

    public class ChestEntityBuilder {
        int id;
        string name;
        int costId;
        int costAmount;
        public ChestEntityBuilder SetId(int id) {
            this.id = id;
            return this;
        }
        public ChestEntityBuilder SetName(string name) {
            this.name = name;
            return this;
        }
        public ChestEntityBuilder SetCostId(int costId) {
            this.costId = costId;
            return this;
        }
        public ChestEntityBuilder SetCostAmount(int costAmount) {
            this.costAmount = costAmount;
            return this;
        }
        public ChestEntity Build() {
            return new ChestEntity(this.id, this.name, this.costId, this.costAmount);
        }
    }

    public class ChestItemEntity
    {
        public int chestId { get; private set; }
        public int chestItemId { get; private set; }
        public string name { get; private set; }
        public int[] itemIds { get; private set; }
        public int itemAmount { get; private set; }
        public float probability { get; private set; }
        public ChestItemEntity(int chestId, int chestItemId, string name, int[] itemIds, int itemAmount, float probability) {
            this.chestId = chestId;
            this.chestItemId = chestItemId;
            this.name = name;
            this.itemIds = itemIds;
            this.itemAmount = itemAmount;
            this.probability = probability;
        }
    }

    public class ChestItemEntityBuilder {
        int chestId;
        int chestItemId;
        string name;
        int[] itemIds;
        int itemAmount;
        float probability;

        public ChestItemEntityBuilder SetChestId(int chestId) {
            this.chestId = chestId;
            return this;
        }
        public ChestItemEntityBuilder SetChestItemId(int chestItemId) {
            this.chestItemId = chestItemId;
            return this;
        }
        public ChestItemEntityBuilder SetName(string name) {
            this.name = name;
            return this;
        }
        public ChestItemEntityBuilder SetItemIds(int[] itemIds) {
            this.itemIds = itemIds;
            return this;
        }
        public ChestItemEntityBuilder SetItemAmount(int itemAmount) {
            this.itemAmount = itemAmount;
            return this;
        }
        public ChestItemEntityBuilder SetProbability(float probability) {
            this.probability = probability;
            return this;
        }
        public ChestItemEntity Build() {
            return new ChestItemEntity(this.chestId, this.chestItemId, this.name, this.itemIds, this.itemAmount, this.probability);
        }
    }
}
