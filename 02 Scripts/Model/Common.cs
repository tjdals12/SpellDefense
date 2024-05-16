using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Common {
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;
    using SkillType = Repository.SkillBook.SkillType;
    using DebuffType = Repository.SkillBook.DebuffType;
    using PartGrade = Repository.Part.PartGrade;
    using PartType = Repository.Part.PartType;
    using ItemType = Repository.Item.ItemType;

    public class SkillBook {
        public int id { get; private set; }
        public SkillBookGrade grade { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public string gradeName { get; private set;}
        public Sprite image { get; private set; }
        public SkillBookUpgradeSpec[] upgradeSpecs { get; private set; }
        public Skill[] skills { get; private set; }
        public SkillBook(int id, SkillBookGrade grade, string name, string description, string gradeName, Sprite image, SkillBookUpgradeSpec[] upgradeSpecs, Skill[] skills) {
            this.id = id;
            this.grade = grade;
            this.name = name;
            this.description = description;
            this.gradeName = gradeName;
            this.image = image;
            this.upgradeSpecs = upgradeSpecs;
            this.skills = skills;
        }
    }

    public class SkillBookUpgradeSpec {
        public SkillBookGrade grade { get; private set; }
        public int level { get; private set; }
        public int requiredGold { get; private set; }
        public int requiredAmount { get; private set; }
        public SkillBookUpgradeSpec(SkillBookGrade grade, int level, int requiredGold, int requiredAmount) {
            this.grade = grade;
            this.level = level;
            this.requiredGold = requiredGold;
            this.requiredAmount = requiredAmount;
        }
    }

    public class Skill {
        public int id { get; private set; }
        public string name { get; private set; }
        public SkillType type { get; private set; }
        public SkillSpec spec { get; private set; }
        public GameObject prefab { get; private set; }
        public Skill(int id, string name, SkillType type, SkillSpec spec, GameObject prefab) {
            this.id = id;
            this.name = name;
            this.type = type;
            this.spec = spec;
            this.prefab = prefab;
        }
    }

    public class SkillSpec {
        public int id { get; private set; }
        public float duration { get; private set; }
        public int targetCount { get; private set; }
        public float damage { get; private set; }
        public float damagePerLevel { get; private set; }
        public int hitCount { get; private set; }
        public float delayPerHit { get; private set; }
        public DebuffType debuffType { get; private set; }
        public float debuffDuration { get; private set; }
        public int coolTime { get; private set; }
        public SkillSpec(int id, float duration, int targetCount, float damage, float damagePerLevel, int hitCount, float delayPerHit, DebuffType debuffType, float debuffDuration, int coolTime) {
            this.id = id;
            this.duration = duration;
            this.targetCount = targetCount;
            this.damage = damage;
            this.damagePerLevel = damagePerLevel;
            this.hitCount = hitCount;
            this.delayPerHit = delayPerHit;
            this.debuffType = debuffType;
            this.debuffDuration = debuffDuration;
            this.coolTime = coolTime;
        }
    }

    public class Part {
        public int id { get; private set; }
        public PartGrade grade { get; private set; }
        public string gradeName { get; private set; }
        public PartType type { get; private set; }
        public string name { get; private set; }
        public Sprite image { get; private set; }
        public PartUpgradeSpec[] upgradeSpecs { get; private set; }
        public PartSpec spec { get; private set; }
        public Part(int id, PartGrade grade, string gradeName, PartType type, string name, Sprite image, PartUpgradeSpec[] upgradeSpecs, PartSpec spec) {
            this.id = id;
            this.grade = grade;
            this.gradeName = gradeName;
            this.type = type;
            this.name = name;
            this.image = image;
            this.upgradeSpecs = upgradeSpecs;
            this.spec = spec;
        }
    }

    public class PartSpec {
        public int id { get; private set; }
        public int attackPower { get; private set; }
        public int attackPowerPerLevel { get; private set; }
        public float attackSpeed { get; private set; }
        public float attackSpeedPerLevel { get; private set; }
        public int criticalRate { get; private set; }
        public int criticalRatePerLevel { get; private set; }
        public int criticalDamage { get; private set; }
        public int criticalDamagePerLevel { get; private set; }
        public PartSpec(int id, int attackPower, int attackPowerPerLevel, float attackSpeed, float attackSpeedPerLevel, int criticalRate, int criticalRatePerLevel, int criticalDamage, int criticalDamagePerLevel) {
            this.id = id;
            this.attackPower = attackPower;
            this.attackPowerPerLevel = attackPowerPerLevel;
            this.attackSpeed = attackSpeed;
            this.attackSpeedPerLevel = attackSpeedPerLevel;
            this.criticalRate = criticalRate;
            this.criticalRatePerLevel = criticalRatePerLevel;
            this.criticalDamage = criticalDamage;
            this.criticalDamagePerLevel = criticalDamagePerLevel;
        }
    }

    public class PartUpgradeSpec {
        public PartGrade grade { get; private set; }
        public int level { get; private set; }
        public int requiredGold { get; private set; }
        public int requiredExp { get; private set; }
        public PartUpgradeSpec(PartGrade grade, int level, int requiredGold, int requiredExp) {
            this.grade = grade;
            this.level = level;
            this.requiredGold = requiredGold;
            this.requiredExp = requiredExp;
        }
    }

    public class Stats {
        public int attackPower { get; private set; }
        public float attackSpeed { get; private set; }
        public int criticalDamage { get; private set; }
        public int criticalRate { get; private set; }
        public Stats(int attackPower, float attackSpeed, int criticalDamage, int criticalRate) {
            this.attackPower = attackPower;
            this.attackSpeed = attackSpeed;
            this.criticalDamage = criticalDamage;
            this.criticalRate = criticalRate;
        }
    }

    public class Item {
        public int id { get; private set; }
        public ItemType type { get; private set; }
        public string name { get; private set; }
        public Sprite image { get; private set; }
        public Item(int id, ItemType type, string name, Sprite image) {
            this.id = id;
            this.type = type;
            this.name = name;
            this.image = image;
        }
    }

    public class Cost {
        public Item item { get; private set; }
        public int amount { get; private set; }
        public Cost(Item item, int amount) {
            this.item = item;
            this.amount = amount;
        }
    }

    public class RewardItem {
        public Item item { get; private set; }
        public int amount { get; private set; }
        public RewardItem(Item item, int amount) {
            this.item = item;
            this.amount = amount;
        }
    }
}
