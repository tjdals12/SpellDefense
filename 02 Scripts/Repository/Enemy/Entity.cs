using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Enemy {
    public class Entity
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public int hp { get; private set; }
        public int hpPerLevel { get; private set; }
        public int attackPower { get; private set; }
        public int attackPowerPerLevel { get; private set; }
        public float attackSpeed { get; private set; }
        public float moveSpeed { get; private set; }
        public int dropSp { get; private set; }
        public Entity(int id, string name, int hp, int hpPerLevel, int attackPower, int attackPowerPerLevel, float attackSpeed, float moveSpeed, int dropSp)
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
        }
    }
    public class Builder {
        int id;
        string name;
        int hp;
        int hpPerLevel;
        int attackPower;
        int attackPowerPerLevel;
        float attackSpeed;
        float moveSpeed;
        int dropSp;
        public Builder SetId(int id) {
            this.id = id;
            return this;
        }
        public Builder SetName(string name) {
            this.name = name;
            return this;
        }
        public Builder SetHp(int hp) {
            this.hp = hp;
            return this;
        }
        public Builder SetHpPerLevel(int hpPerLevel) {
            this.hpPerLevel = hpPerLevel;
            return this;
        }
        public Builder SetAttackPower(int attackPower) {
            this.attackPower = attackPower;
            return this;
        }
        public Builder SetAttackPowerPerLevel(int attackPowerPerLevel) {
            this.attackPowerPerLevel = attackPowerPerLevel;
            return this;
        }
        public Builder SetAttackSpeed(float attackSpeed) {
            this.attackSpeed = attackSpeed;
            return this;
        }
        public Builder SetMoveSpeed(float moveSpeed) {
            this.moveSpeed = moveSpeed;
            return this;
        }
        public Builder SetDropSp(int dropSp) {
            this.dropSp = dropSp;
            return this;
        }
        public Entity Build() {
            return new Entity(
                id,
                name,
                hp,
                hpPerLevel,
                attackPower,
                attackPowerPerLevel,
                attackSpeed,
                moveSpeed,
                dropSp
            );
        }
    }
}