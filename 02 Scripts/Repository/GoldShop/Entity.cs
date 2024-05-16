using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.GoldShop {
    public class Entity
    {
        public int itemId { get; private set; }
        public int itemAmount { get; private set; }
        public int costId { get; private set; }
        public int costAmount { get; private set; }
        public int buyCount { get; private set; }
        public Entity(int itemId, int itemAmount, int costId, int costAmount, int buyCount) {
            this.itemId = itemId;
            this.itemAmount = itemAmount;
            this.costId = costId;
            this.costAmount = costAmount;
            this.buyCount = buyCount;
        }
    }

    public class Builder {
        int itemId;
        int itemAmount;
        int costId;
        int costAmount;
        int buyCount;

        public Builder SetItemId(int itemId) {
            this.itemId = itemId;
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
        public Builder SetBuyCount(int buyCount) {
            this.buyCount = buyCount;
            return this;
        }
        public Entity Build() {
            return new Entity(this.itemId, this.itemAmount, this.costId, this.costAmount, this.buyCount);
        }
    }
}
