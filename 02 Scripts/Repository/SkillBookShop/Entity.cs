using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.SkillBookShop {
    public class Entity
    {
        public int index { get; private set; }
        public int[] itemIds { get; private set; }
        public int itemAmount { get; private set; }
        public int costId { get; private set; }
        public int costAmount { get; private set; }
        public float probability { get; private set; }
        public Entity(int index, int[] itemIds, int itemAmount, int costId, int costAmount, float probability) {
            this.index = index;
            this.itemIds = itemIds;
            this.itemAmount = itemAmount;
            this.costId = costId;
            this.costAmount = costAmount;
            this.probability = probability;
        }
    }

    public class Builder {
        int index;
        int[] itemIds;
        int itemAmount;
        int costId;
        int costAmount;
        float probability;

        public Builder SetIndex(int index) {
            this.index = index;
            return this;
        }
        public Builder SetItemIds(int[] itemIds) {
            this.itemIds = itemIds;
            return this;
        }
        public Builder SetItemAmount(int itemAmount) {
            this.itemAmount = itemAmount;
            return this;
        }
        public Builder SetCostId(int costId) {
            this.costId = costId;
            return this;
        }
        public Builder SetCostAmount(int costAmount) {
            this.costAmount = costAmount;
            return this;
        }
        public Builder SetProbability(float probability) {
            this.probability = probability;
            return this;
        }
        public Entity Build() {
            return new Entity(this.index, this.itemIds, this.itemAmount, this.costId, this.costAmount, this.probability);
        }
    }
}
