using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Item {
    public enum ItemType {
        Currency = 0,
        Key,
        Chest,
        SkillBook = 10,
        Part = 20
    }

    public class Entity
    {
        public int id { get; private set; }
        public ItemType type { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public Sprite image { get; private set; }
        public Entity(int id, ItemType type, string name, Sprite image) {
            this.id = id;
            this.type = type;
            this.name = name;
            this.image = image;
        }
    }

    public class Builder {
        int id;
        ItemType type;
        string name;
        string description;
        Sprite image;
        public Builder SetId(int id) {
            this.id = id;
            return this;
        }
        public Builder SetType(ItemType type) {
            this.type = type;
            return this;
        }
        public Builder SetName(string name) {
            this.name = name;
            return this;
        }
        public Builder SetDescription(string description) {
            this.description = description;
            return this;
        }
        public Builder SetImage(Sprite image) {
            this.image = image;
            return this;
        }
        public Entity Build() {
            return new Entity(this.id, this.type, this.name, this.image);
        }
    }
}