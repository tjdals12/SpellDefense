using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.SkillBook {
    public enum SkillBookGrade {
        Common = 0,
        Uncommon,
        Rare,
        Epic
    }

    public class SkillBookEntity {
        public int id { get; private set; }
        public string name { get; private set; } 
        public string description { get; private set; }
        public string gradeName { get; private set; }
        public SkillBookGrade grade { get; private set; }
        public int[] skills { get; private set; }
        public SkillBookEntity(int id, string name, string gradeName, string description, SkillBookGrade grade, int[] skills) {
            this.id = id;
            this.name = name;
            this.gradeName = gradeName;
            this.description = description;
            this.grade = grade;
            this.skills = skills;
        }
    }

    public class SkillBookEntityBuilder {
        int id;
        string name;
        string description;
        string gradeName;
        SkillBookGrade grade;
        int skill1;
        int skill2;
        int skill3;
        int skill4;
        int skill5;
        public SkillBookEntityBuilder SetId(int id) {
            this.id = id;
            return this;
        }
        public SkillBookEntityBuilder SetName(string name) {
            this.name = name;
            return this;
        }
        public SkillBookEntityBuilder SetDescription(string description) {
            this.description = description;
            return this;
        }
        public SkillBookEntityBuilder SetGradeName(string gradeName) {
            this.gradeName = gradeName;
            return this;
        }
        public SkillBookEntityBuilder SetGrade(SkillBookGrade grade) {
            this.grade = grade;
            return this;
        }
        public SkillBookEntityBuilder SetSkill1(int skill) {
            this.skill1 = skill;
            return this;
        }
        public SkillBookEntityBuilder SetSkill2(int skill) {
            this.skill2 = skill;
            return this;
        }
        public SkillBookEntityBuilder SetSkill3(int skill) {
            this.skill3 = skill;
            return this;
        }
        public SkillBookEntityBuilder SetSkill4(int skill) {
            this.skill4 = skill;
            return this;
        }
        public SkillBookEntityBuilder SetSkill5(int skill) {
            this.skill5 = skill;
            return this;
        }
        public SkillBookEntity Build() {
            int[] skills = new int[] { this.skill1, this.skill2, this.skill3, this.skill4, this.skill5 };
            return new SkillBookEntity(this.id, this.name, this.gradeName, this.description, this.grade, skills);
        }
    }

    public class SkillBookUpgradeSpecEntity {
        public SkillBookGrade grade { get; private set; }
        public int level { get; private set; }
        public int requiredGold { get; private set; }
        public int requiredAmount { get; private set; }
        public SkillBookUpgradeSpecEntity(SkillBookGrade grade, int level, int requiredGold, int requiredAmount) {
            this.grade = grade;
            this.level = level;
            this.requiredGold = requiredGold;
            this.requiredAmount = requiredAmount;
        }
    }

    public class SkillBookUpgradeSpecEntityBuilder {
        SkillBookGrade grade;
        int level;
        int requiredGold;
        int requiredAmount;
        public SkillBookUpgradeSpecEntityBuilder SetGrade(SkillBookGrade grade) {
            this.grade = grade;
            return this;
        }
        public SkillBookUpgradeSpecEntityBuilder SetLevel(int level) {
            this.level = level;
            return this;
        }
        public SkillBookUpgradeSpecEntityBuilder SetRequiredGold(int requiredGold) {
            this.requiredGold = requiredGold;
            return this;
        }
        public SkillBookUpgradeSpecEntityBuilder SetRequiredAmount(int requiredAmount) {
            this.requiredAmount = requiredAmount;
            return this;
        }
        public SkillBookUpgradeSpecEntity Build() {
            return new SkillBookUpgradeSpecEntity(this.grade, this.level, this.requiredGold, this.requiredAmount);
        }
    }

    public enum SkillType {
        Attack = 0,
        RangeAttack,
    }

    public class SkillEntity {
        public int id { get; private set; }
        public string name { get; private set; }
        public SkillType type { get; private set; }
        public SkillEntity(int id, string name, SkillType type) {
            this.id = id;
            this.name = name;
            this.type = type;
        }
    }

    public class SkillEntityBuilder {
        int id;
        string name;
        SkillType type;
        public SkillEntityBuilder SetId(int id) {
            this.id = id;
            return this;
        }
        public SkillEntityBuilder SetName(string name) {
            this.name = name;
            return this;
        }
        public SkillEntityBuilder SetType(SkillType type) {
            this.type = type;
            return this;
        }
        public SkillEntity Build() {
            return new SkillEntity(this.id, this.name, this.type);
        }
    }

    public enum DebuffType {
        None = 0,
        Slow,
        Freeze,
        Poisoned,
        Electrified,
    }

    public class SkillSpecEntity
    {
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
        public SkillSpecEntity(int id, float duration, int targetCount, float damage, float damagePerLevel, int hitCount, float delayPerHit, DebuffType debuffType, float debuffDuration, int coolTime) {
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

    public class SkillSpecEntityBuilder {
        int id;
        float duration;
        int targetCount;
        float damage;
        float damagePerLevel;
        int hitCount;
        float delayPerHit;
        DebuffType debuffType;
        float debuffDuration;
        int coolTime;
        public SkillSpecEntityBuilder SetId(int id) {
            this.id = id;
            return this;
        }
        public SkillSpecEntityBuilder SetDuration(float duration) {
            this.duration = duration;
            return this;
        }
        public SkillSpecEntityBuilder SetTargetCount(int targetCount) {
            this.targetCount = targetCount;
            return this;
        }
        public SkillSpecEntityBuilder SetDamage(float damage) {
            this.damage = damage;
            return this;
        }
        public SkillSpecEntityBuilder SetDamagePerLevel(float damagePerLevel) {
            this.damagePerLevel = damagePerLevel;
            return this;
        }
        public SkillSpecEntityBuilder SetHitCount(int hitCount) {
            this.hitCount = hitCount;
            return this;
        }
        public SkillSpecEntityBuilder SetDelayPerHit(float delayPerHit) {
            this.delayPerHit = delayPerHit;
            return this;
        }
        public SkillSpecEntityBuilder SetDebuffType(DebuffType debuffType) {
            this.debuffType = debuffType;
            return this;
        }
        public SkillSpecEntityBuilder SetDebuffDuration(float debuffDuration) {
            this.debuffDuration = debuffDuration;
            return this;
        }
        public SkillSpecEntityBuilder SetCoolTime(int coolTime) {
            this.coolTime = coolTime;
            return this;
        }
        public SkillSpecEntity Build() {
            return new SkillSpecEntity(this.id, this.duration, this.targetCount, this.damage, this.damagePerLevel, this.hitCount, this.delayPerHit, this.debuffType, this.debuffDuration, this.coolTime);
        }
    }
}
