using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.OutGame.Part {
    using InventoryPart = Model.OutGame.IInventoryPart;
    using Stats = Model.Common.Stats;

    public class PartSpec : MonoBehaviour
    {
        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject panel;

        [Header("Prefab")]
        [Space(4)]
        [SerializeField]
        GameObject attackPowerStatPrefab;
        [SerializeField]
        GameObject attackSpeedStatPrefab;
        [SerializeField]
        GameObject criticalRateStatPrefab;
        [SerializeField]
        GameObject criticalDamageStatPrefab;

        InventoryPart inventoryPart;

        Stat attackPowerStat;
        Stat attackSpeedStat;
        Stat criticalRateStat;
        Stat criticalDamageStat;

        public void Initialize(InventoryPart inventoryPart) {
            this.inventoryPart = inventoryPart;
            var stats = inventoryPart.GetStats();
            this.UpdateAttackPower(stats.attackPower);
            this.UpdateAttackSpeed(stats.attackSpeed);
            this.UpdateCriticalRate(stats.criticalRate);
            this.UpdateCriticalDamage(stats.criticalDamage);
        }
        public void UpdateView(int earnExp) {
            var stats = this.inventoryPart.GetStats();
            int totalExp = this.inventoryPart.exp + earnExp;
            var upgradeSpec = Array.Find(this.inventoryPart.part.upgradeSpecs, (e) => e.requiredExp > totalExp);
            int upLevel = (upgradeSpec.level - 1) - this.inventoryPart.level;
            this.UpdateAttackPower(stats.attackPower, upLevel);
            this.UpdateAttackSpeed(stats.attackSpeed, upLevel);
            this.UpdateCriticalRate(stats.criticalRate, upLevel);
            this.UpdateCriticalDamage(stats.criticalDamage, upLevel);
        }

        public void UpdateView(InventoryPart inventoryPart) {
            var stats = inventoryPart.GetStats();
            this.UpdateAttackPower(stats.attackPower);
            this.UpdateAttackSpeed(stats.attackSpeed);
            this.UpdateCriticalRate(stats.criticalRate);
            this.UpdateCriticalDamage(stats.criticalDamage);
        }

        void UpdateAttackPower(int attackPower, int upLevel) {
            if (upLevel > 0) {
                var spec = this.inventoryPart.part.spec;
                int upAttackPower = spec.attackPowerPerLevel * upLevel;
                if (upAttackPower > 0) {
                    if (this.attackPowerStat == null) {
                        this.attackPowerStat = this.CreateStat(this.attackPowerStatPrefab, $"{attackPower.ToString()} <color=green>(+{upAttackPower.ToString()})</color>");
                    } else {
                        this.attackPowerStat.Initialize($"{attackPower.ToString()} <color=green>(+{upAttackPower.ToString()})</color>");
                    }
                }
            } else {
                if (this.attackPowerStat != null) {
                    this.attackPowerStat.Initialize(attackPower.ToString());
                }
            }
        }

        void UpdateAttackPower(int attackPower) {
            if (attackPower > 0) {
                if (this.attackPowerStat == null) {
                    this.attackPowerStat = this.CreateStat(this.attackPowerStatPrefab, attackPower.ToString());
                } else {
                    this.attackPowerStat.Initialize(attackPower.ToString());
                }
            } else {
                if (this.attackPowerStat != null) {
                    Destroy(this.attackPowerStat.gameObject);
                }
            }
        }

        void UpdateAttackSpeed(float attackSpeed, int upLevel) {
            if (upLevel > 0) {
                var spec = this.inventoryPart.part.spec;
                float upAttackSpeed = spec.attackSpeedPerLevel * upLevel;
                if (upAttackSpeed > 0) {
                    if (this.attackSpeedStat == null) {
                        this.attackSpeedStat = this.CreateStat(this.attackSpeedStatPrefab, $"{attackSpeed.ToString("0.00")} <color=green>(+{upAttackSpeed.ToString("0.00")})</color>");
                    } else {
                        this.attackSpeedStat.Initialize($"{attackSpeed.ToString("0.00")} <color=green>(+{upAttackSpeed.ToString("0.00")})</color>");
                    }
                }
            } else {
                if (this.attackSpeedStat != null) {
                    this.attackSpeedStat.Initialize(attackSpeed.ToString("0.0"));
                }
            }
        }

        void UpdateAttackSpeed(float attackSpeed) {
            if (attackSpeed > 0) {
                if (this.attackSpeedStat == null) {
                    this.attackSpeedStat = this.CreateStat(this.attackSpeedStatPrefab, attackSpeed.ToString("0.00"));
                } else {
                    this.attackSpeedStat.Initialize(attackSpeed.ToString("0.00"));
                }
            } else {
                if (this.attackSpeedStat != null) {
                    Destroy(this.attackSpeedStat.gameObject);
                }
            }
        }

        void UpdateCriticalRate(int criticalRate, int upLevel) {
            if (upLevel > 0) {
                var spec = this.inventoryPart.part.spec;
                int upCriticalRate = spec.criticalRatePerLevel * upLevel;
                if (upCriticalRate > 0) {
                    if (this.criticalRateStat == null) {
                        this.criticalRateStat = this.CreateStat(this.criticalRateStatPrefab, $"{criticalRate}% <color=green>(+{upCriticalRate})</color>");
                    } else {
                        this.criticalRateStat.Initialize($"{criticalRate}% <color=green>(+{upCriticalRate}%)</color>");
                    }
                }
            } else {
                if (this.criticalRateStat != null) {
                    this.criticalRateStat.Initialize($"{criticalRate}%");
                }
            }
        }

        void UpdateCriticalRate(int criticalRate) {
            if (criticalRate > 0) {
                if (this.criticalRateStat == null) {
                    this.criticalRateStat = this.CreateStat(this.criticalRateStatPrefab, $"{criticalRate}%");
                } else {
                    this.criticalRateStat.Initialize($"{criticalRate}%");
                }
            } else {
                if (this.criticalRateStat != null) {
                    Destroy(this.criticalRateStat.gameObject);
                }
            }
        }

        void UpdateCriticalDamage(int criticalDamage, int upLevel) {
            if (upLevel > 0) {
                var spec = this.inventoryPart.part.spec;
                int upCriticalDamage = spec.criticalDamagePerLevel * upLevel;
                if (upCriticalDamage > 0) {
                    if (this.criticalDamageStat == null) {
                        this.criticalDamageStat = this.CreateStat(this.criticalDamageStatPrefab, $"{criticalDamage}% <color=green>(+{upCriticalDamage}%)</color>");
                    } else {
                        this.criticalDamageStat.Initialize($"{criticalDamage}% <color=green>(+{upCriticalDamage}%)</color>");
                    }
                }
            } else {
                if (this.criticalDamageStat != null) {
                    this.criticalDamageStat.Initialize($"{criticalDamage}%");
                }
            }
        }

        void UpdateCriticalDamage(int criticalDamage) {
            if (criticalDamage > 0) {
                if (this.criticalDamageStat == null) {
                    this.criticalDamageStat = this.CreateStat(this.criticalDamageStatPrefab, $"{criticalDamage}%");
                } else {
                    this.criticalDamageStat.Initialize($"{criticalDamage}%");
                }
            } else {
                if (this.criticalDamageStat != null) {
                    Destroy(this.criticalDamageStat.gameObject);
                }
            }
        }

        Stat CreateStat(GameObject prefab, string value) {
            GameObject clone = Instantiate(prefab, this.panel.transform);
            Stat stat = clone.GetComponent<Stat>();
            stat.Initialize(value);
            return stat;
        }
    }
}
