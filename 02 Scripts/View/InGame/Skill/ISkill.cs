using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View.InGame.Skill {
    using DebuffType = Repository.SkillBook.DebuffType;
    using CastingSkillSpec = Model.InGame.CastingSkillSpec;

    public class Debuff {
        public DebuffType type { get; private set; }
        public float duration { get; private set; }
        public Debuff(DebuffType type, float duration) {
            this.type = type;
            this.duration = duration;
        }
    }

    public abstract class ISkill : MonoBehaviour
    {
        protected int level;
        protected int attackPower;
        protected bool isCritical;
        protected int criticalRate;
        protected int criticalDamage;
        protected Enemy.Instance target;
        protected Vector3 position;
        protected CastingSkillSpec skillSpec;

        protected Action onSuccess;
        protected Action onCancel;

        #region Unity Method
        void OnDestroy() {
            this.onSuccess?.Invoke();
        }
        #endregion

        public ISkill SetLevel(int level) {
            this.level = level;
            return this;
        }

        public ISkill SetAttackPower(int attackPower) {
            this.attackPower = attackPower;
            return this;
        }
        public ISkill SetCriticalRate(int criticalRate) {
            this.criticalRate = criticalRate;
            return this;
        }
        public ISkill SetCriticalDamage(int criticalDamage) {
            this.criticalDamage = criticalDamage;
            return this;
        }
        public ISkill SetTarget(Enemy.Instance target) {
            this.target = target;
            return this;
        }
        public ISkill SetPosition(Vector3 position) {
            this.position = position;
            return this;
        }
        public ISkill SetSkillSpec(CastingSkillSpec skillSpec) {
            this.skillSpec = skillSpec;
            return this;
        }

        public ISkill OnSuccess(Action callback) {
            this.onSuccess = callback;
            return this;
        }

        public ISkill OnCancel(Action callback) {
            this.onCancel = callback;
            return this;
        }

        public virtual void Use() {}

        protected int GetDamage() {
            this.isCritical = Random.Range(0f, 100f) >= this.criticalRate;
            float damage = (this.skillSpec.damage + (this.skillSpec.damagePerLevel * this.level)) * this.attackPower;
            if (this.isCritical) {
                damage = (damage * this.criticalDamage) / 100;
            }
            return Mathf.RoundToInt(damage);
        }
    }
}
