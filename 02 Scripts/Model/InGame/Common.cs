using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.InGame {
    using SkillBookGrade = Repository.SkillBook.SkillBookGrade;
    using SkillType = Repository.SkillBook.SkillType;
    using DebuffType = Repository.SkillBook.DebuffType;
    using Common;

    public class EquippedPart {
        public Part part { get; private set; }
        public int level { get; private set; }
        public EquippedPart(Part part, int level) {
            this.part = part;
            this.level = level;
        }
        public Stats GetStats() {
            PartSpec spec = this.part.spec;
            int attackPower = spec.attackPower + (spec.attackPowerPerLevel * (this.level - 1));
            float attackSpeed = spec.attackSpeed + (spec.attackSpeedPerLevel * (this.level - 1));
            int criticalRate = spec.criticalRate + (spec.criticalRatePerLevel * (this.level - 1));
            int criticalDamage = spec.criticalDamage + (spec.criticalDamage * (this.level - 1));
            return new Stats(attackPower, attackSpeed, criticalRate, criticalDamage);
        }
    }

    public class EquippedParts {
        public EquippedPart armor { get; private set; }
        public EquippedPart weapon { get; private set; }
        public EquippedPart jewelry { get; private set; }
        public EquippedParts(EquippedPart armor, EquippedPart weapon, EquippedPart jewelry) {
            this.armor = armor;
            this.weapon = weapon;
            this.jewelry = jewelry;
        }
    }

    public class SkillBookAdditionalUpgradeSpec {
        public int level { get; private set; }
        public int requiredSp { get; private set; }
        public SkillBookAdditionalUpgradeSpec(int level, int requiredSp) {
            this.level = level;
            this.requiredSp = requiredSp;
        }
    }

    public abstract class IEquippedSkillBook {
        public SkillBook skillBook { get; protected set; }
        public int level { get; protected set; }
        public SkillBookAdditionalUpgradeSpec[] upgradeSpecs { get; protected set; }
        public int upgradeLevel { get; protected set; }
        public IEquippedSkillBook(SkillBook skillBook, int level, SkillBookAdditionalUpgradeSpec[] upgradeSpecs) {
            this.skillBook = skillBook;
            this.level = level;
            this.upgradeSpecs = upgradeSpecs;
            this.upgradeLevel = 0;
        }
        public bool CanUpgrade() {
            SkillBookAdditionalUpgradeSpec upgradeSpec = this.GetCurrentUpgradeSpec();
            return upgradeSpec != null;
        }
        public SkillBookAdditionalUpgradeSpec GetCurrentUpgradeSpec() {
            SkillBookAdditionalUpgradeSpec upgradeSpec = Array.Find(this.upgradeSpecs, (e) => e.level == this.upgradeLevel + 1);
            return upgradeSpec;
        }
    }

    public class EquippedSkillBook : IEquippedSkillBook
    {
        public EquippedSkillBook(SkillBook skillBook, int level, SkillBookAdditionalUpgradeSpec[] upgradeSpecs) : base(skillBook, level, upgradeSpecs)
        {
        }
        public void Upgrade() {
            this.upgradeLevel = Mathf.Min(this.upgradeLevel + 1, this.upgradeSpecs.Length);
        }
    }

    public class CastingSkillSpec {
        public int id { get; private set; }
        public int coolTime { get; private set; }
        public float duration { get; private set; }
        public int targetCount { get; private set; }
        public float damage { get; private set; }
        public float damagePerLevel { get; private set; }
        public int hitCount { get; private set; }
        public float delayPerHit { get; private set; }
        public DebuffType debuffType { get; private set; }
        public float debuffDuration { get; private set; }
        public CastingSkillSpec(int id, int coolTime, float duration, int targetCount, float damage, float damagePerLevel, int hitCount, float delayPerHit, DebuffType debuffType, float debuffDuration) {
            this.id = id;
            this.coolTime = coolTime;
            this.duration = duration;
            this.targetCount = targetCount;
            this.damage = damage;
            this.damagePerLevel = damagePerLevel;
            this.hitCount = hitCount;
            this.delayPerHit = delayPerHit;
            this.debuffType = debuffType;
            this.debuffDuration = debuffDuration;
        }
    }

    public class CastingSkill {
        public GameObject prefab { get; private set; }
        public SkillType skillType { get; private set; }
        public CastingSkillSpec skillSpec { get; private set; }
        public CastingSkill(GameObject prefab, SkillType skillType, CastingSkillSpec skillSpec) {
            this.prefab = prefab;
            this.skillType = skillType;
            this.skillSpec = skillSpec;
        }
    }

    public class SpawnedSkillBook {
        public int id { get; private set; }
        public SkillBookGrade grade { get; private set; }
        public Sprite image { get; private set; }
        public int mergeCount { get; private set; }
        public CastingSkill skill { get; private set; }
        public SpawnedSkillBook(int id, SkillBookGrade grade, Sprite image, int mergeCount, CastingSkill skill) {
            this.id = id;
            this.grade = grade;
            this.image = image;
            this.mergeCount = mergeCount;
            this.skill = skill;
        }
        public bool CanMerge() {
            return 4 > this.mergeCount;
        }
    }

    public class User {
        public EquippedParts equippedParts { get; private set; }
        public EquippedSkillBook[] equippedSkillBooks { get; private set; }
        public User(EquippedParts equippedParts, EquippedSkillBook[] equippedSkillBooks) {
            this.equippedParts = equippedParts;
            this.equippedSkillBooks = equippedSkillBooks;
        }
    }

    public class Enemy {
        public int id { get; private set; }
        public string name { get; private set; }
        public int hp { get; private set; }
        public int hpPerLevel { get; private set; }
        public int attackPower { get; private set; }
        public int attackPowerPerLevel { get; private set; }
        public float attackSpeed { get; private set; }
        public float moveSpeed { get; private set; }
        public int dropSp { get; private set; }
        public GameObject prefab { get; private set; }
        public Enemy(int id, string name, int hp, int hpPerLevel, int attackPower, int attackPowerPerLevel, float attackSpeed, float moveSpeed, int dropSp, GameObject prefab)
        {
            this.id = id;
            this.name = name;
            this.hp = hp;
            this.hpPerLevel = hpPerLevel;
            this.attackPower = attackPower;
            this.attackPowerPerLevel = attackPowerPerLevel;
            this.attackSpeed = attackSpeed;
            this.moveSpeed = moveSpeed;
            this.dropSp = dropSp;
            this.prefab = prefab;
        }
    }

    public class SpawningEnemy {
        public Enemy enemy { get; private set; }
        public int level { get; private set; }
        public int count { get; private set; }
        public SpawningEnemy(Enemy enemy, int level, int count) {
            this.enemy = enemy;
            this.level = level;
            this.count = count;
        }
    }

    public class Wave {
        public int startWave { get; private set; }
        public int endWave { get; private set; }
        public int minCountPerSpawn { get; private set; }
        public int maxCountPerSpawn { get; private set; }
        public float minSpawnDelay { get; private set; }
        public float maxSpawnDelay { get; private set; }
        public SpawningEnemy[] spawningEnemies { get; private set; }
        public RewardItem[] clearRewards { get; private set; }
        public Wave(int startWave, int endWave, int minCountPerSpawn, int maxCountPerSpawn, float minSpawnDelay, float maxSpawnDelay, SpawningEnemy[] spawningEnemies, RewardItem[] clearRewards)
        {
            this.startWave = startWave;
            this.endWave = endWave;
            this.minCountPerSpawn = minCountPerSpawn;
            this.maxCountPerSpawn = maxCountPerSpawn;
            this.minSpawnDelay = minSpawnDelay;
            this.maxSpawnDelay = maxSpawnDelay;
            this.spawningEnemies = spawningEnemies;
            this.clearRewards = clearRewards;
        }
    }
}
