using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Part {
    public enum PartGrade {
        Common = 0,
        Uncommon,
        Rare,
        Epic
    }

    public enum PartType {
        Armor = 0,
        Weapon,
        Jewelry
    } 

     public class PartEntity
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public string gradeName { get; private set; }
        public PartGrade grade { get; private set; }
        public PartType type { get; private set; }
        public PartEntity(int id, string name, string gradeName, PartGrade grade, PartType type) {
            this.id = id;
            this.name = name;
            this.gradeName = gradeName;
            this.grade = grade;
            this.type = type;
        }
    }

    public class PartEntityBuilder {
        int id;
        string name;
        string gradeName;
        PartGrade grade;
        PartType type;
        public PartEntityBuilder SetId(int id) {
            this.id = id;
            return this;
        }
        public PartEntityBuilder SetName(string name) {
            this.name = name;
            return this;
        }
        public PartEntityBuilder SetGradeName(string gradeName) {
            this.gradeName = gradeName;
            return this;
        }
        public PartEntityBuilder SetGrade(PartGrade grade) {
            this.grade = grade;
            return this;
        }
        public PartEntityBuilder SetType(PartType type) {
            this.type = type;
            return this;
        }
        public PartEntity Build() {
            return new PartEntity(this.id, this.name, this.gradeName, this.grade, this.type);
        }
    }

    public class PartUpgradeSpecEntity
    {
        public int grade { get; private set; }
        public int level { get; private set; }
        public int requiredGold { get; private set; }
        public int requiredExp { get; private set; }
        public PartUpgradeSpecEntity(int grade, int level, int requiredGold, int requiredExp) {
            this.grade = grade;
            this.level = level;
            this.requiredGold = requiredGold;
            this.requiredExp = requiredExp;
        }
    }

    public class PartUpgradeSpecEntityBuilder {
        int grade;
        int level;
        int requiredGold;
        int requiredExp;
        public PartUpgradeSpecEntityBuilder SetGrade(int grade) {
            this.grade = grade;
            return this;
        }
        public PartUpgradeSpecEntityBuilder SetLevel(int level) {
            this.level = level;
            return this;
        }
        public PartUpgradeSpecEntityBuilder SetRequiredGold(int requiredGold) {
            this.requiredGold = requiredGold;
            return this;
        }
        public PartUpgradeSpecEntityBuilder SetRequiredExp(int requiredExp) {
            this.requiredExp = requiredExp;
            return this;
        }
        public PartUpgradeSpecEntity Build() {
            return new PartUpgradeSpecEntity(this.grade, this.level, this.requiredGold, this.requiredExp);
        }
    }

    public class PartSpecEntity
    {
        public int id { get; private set; }
        public int attackPower { get; private set; }
        public int attackPowerPerLevel { get; private set; }
        public float attackSpeed { get; private set; }
        public float attackSpeedPerLevel { get; private set; }
        public int criticalRate { get; private set; }
        public int criticalRatePerLevel { get; private set; }
        public int criticalDamage { get; private set; }
        public int criticalDamagePerLevel { get; private set; }
        public PartSpecEntity(int id, int attackPower, int attackPowerPerLevel, float attackSpeed, float attackSpeedPerLevel, int criticalRate, int criticalRatePerLevel, int criticalDamage, int criticalDamagePerLevel) {
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

    public class PartSpecEntityBuilder {
        int id;
        int attackPower;
        int attackPowerPerLevel;
        float attackSpeed;
        float attackSpeedPerLevel;
        int criticalRate;
        int criticalRatePerLevel;
        int criticalDamage;
        int criticalDamagePerLevel;
        public PartSpecEntityBuilder SetId(int id) {
            this.id = id;
            return this;
        }
        public PartSpecEntityBuilder SetAttackPower(int attackPower) {
            this.attackPower = attackPower;
            return this;
        }
        public PartSpecEntityBuilder SetAttackPowerPerLevel(int attackPowerPerLevel) {
            this.attackPowerPerLevel = attackPowerPerLevel;
            return this;
        }
        public PartSpecEntityBuilder SetAttackSpeed(float attackSpeed) {
            this.attackSpeed = attackSpeed;
            return this;
        }
        public PartSpecEntityBuilder SetAttackSpeedPerLevel(float attackSpeedPerLevel) {
            this.attackSpeedPerLevel = attackSpeedPerLevel;
            return this;
        }
        public PartSpecEntityBuilder SetCriticalRate(int criticalRate) {
            this.criticalRate = criticalRate;
            return this;
        }
        public PartSpecEntityBuilder SetCriticalRatePerLevel(int criticalRatePerLevel) {
            this.criticalRatePerLevel = criticalRatePerLevel;
            return this;
        }
        public PartSpecEntityBuilder SetCriticalDamage(int criticalDamage) {
            this.criticalDamage = criticalDamage;
            return this;
        }
        public PartSpecEntityBuilder SetCriticalDamagePerLevel(int criticalDamagePerLevel) {
            this.criticalDamagePerLevel = criticalDamagePerLevel;
            return this;
        }
        public PartSpecEntity Build() {
            return new PartSpecEntity(
                this.id,
                this.attackPower,
                this.attackPowerPerLevel,
                this.attackSpeed,
                this.attackSpeedPerLevel,
                this.criticalRate,
                this.criticalRatePerLevel,
                this.criticalDamage,
                this.criticalDamagePerLevel
            );
        }
    }
}
