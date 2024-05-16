using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.InGame {
    using Common;

    public abstract class IPlayerModel : MonoBehaviour
    {
        public int maxHp { get; protected set; }
        public int currentHp { get; protected set; }
        public int attackPower { get; protected set; }
        public float attackSpeed { get; protected set; }
        public int criticalRate { get; protected set; }
        public int criticalDamage { get; protected set; }
        public bool isDead { get; protected set; }

        public Action OnInitialize;
        public Action OnDamage;
        public Action OnDead;
    }

    public class PlayerModel : IPlayerModel {
        public void Initialize(User user) {
            this.maxHp = 100;
            this.currentHp = this.maxHp;
            EquippedPart equippedArmor = user.equippedParts.armor;
            EquippedPart equippedWeapon = user.equippedParts.weapon;
            EquippedPart equippedJewelry = user.equippedParts.jewelry;
            Stats summaryStats = this.SummarizeStats(new Stats[] {
                new Stats(attackPower: 30, attackSpeed: 1f, criticalDamage: 120, criticalRate: 0),
                equippedArmor.GetStats(),
                equippedWeapon.GetStats(),
                equippedJewelry.GetStats()
            });
            this.attackPower = summaryStats.attackPower;
            this.attackSpeed = summaryStats.attackSpeed;
            this.criticalDamage = summaryStats.criticalDamage;
            this.criticalRate = summaryStats.criticalRate;
            this.OnInitialize?.Invoke();
        }

        Stats SummarizeStats(Stats[] statsList) {
            int attackPower = 0;
            float attackSpeed = 0;
            int criticalDamage = 0;
            int criticalRate = 0;
            foreach (var stats in statsList) {
                attackPower += stats.attackPower;
                attackSpeed += stats.attackSpeed;
                criticalDamage += stats.criticalDamage;
                criticalRate += stats.criticalRate;
            }
            return new Stats(attackPower, attackSpeed, criticalDamage, criticalRate);
        }

        public void Damage(int amount) {
            if (this.isDead) return;
            this.currentHp -= amount;
            this.OnDamage?.Invoke();
            if (0 >= this.currentHp) {
                this.isDead = true;
                this.OnDead?.Invoke();
            }
        }
    }
}
