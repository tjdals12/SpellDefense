using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Model.InGame {
    public abstract class IBoardModel : MonoBehaviour
    {
        protected EquippedSkillBook[] _skillBooks;
        public IEquippedSkillBook[] skillBooks { get => this._skillBooks; }
        public int currentSp { get; protected set; }
        public int requiredSp { get; protected set; }
        protected int increasingSpPerSpawn;
        protected List<int> remainingSlots;
        public Dictionary<int, SpawnedSkillBook> usingSlots { get; protected set; }

        public Action OnInitialize;
        public Action OnChangeCurrentSp;
        public Action OnChangeRequiredSp;
        public Action<int> OnSpawnSkillBook;
        public Action<int, int> OnMergeSkillBook;
        public Action<int> OnUpgradeSkillBook;
        public Action<int> OnCastSkillBook;
        public Action<int> OnCompleteCastingSkillBook;

        public bool HasEnoughSp() {
            return this.currentSp >= this.requiredSp;
        }

        public bool HasRemainingSlot() {
            return this.remainingSlots.Count > 0;
        }

        public bool CanMerge(int slot1, int slot2) {
            if (slot1 == slot2) return false;
            SpawnedSkillBook skillBook1 = this.usingSlots.GetValueOrDefault(slot1);
            if (skillBook1 == null) return false;
            SpawnedSkillBook skillBook2 = this.usingSlots.GetValueOrDefault(slot2);
            if (skillBook2 == null) return false;
            if (skillBook1.CanMerge() == false || skillBook2.CanMerge() == false) return false;
            if (skillBook1.mergeCount != skillBook2.mergeCount) return false;
            if (skillBook1.id != skillBook2.id) return false;
            return true;
        }

        public bool CanUpgrade(int skillBookId) {
            EquippedSkillBook equippedSkillBook = Array.Find(this._skillBooks, (e) => e.skillBook.id == skillBookId);
            SkillBookAdditionalUpgradeSpec skillBookUpgradeSpec = equippedSkillBook.GetCurrentUpgradeSpec();
            if (skillBookUpgradeSpec == null) return false;
            if (skillBookUpgradeSpec.requiredSp > this.currentSp) return false;
            return true;
        }
    }
    public class BoardModel : IBoardModel {
        public void Initialize(User user) {
            this._skillBooks = user.equippedSkillBooks;
            this.currentSp = 100;
            this.requiredSp = 10;
            this.increasingSpPerSpawn = 10;
            this.remainingSlots = new();
            for (int i = 0; i < 15; i++) {
                this.remainingSlots.Add(i);
            }
            this.usingSlots = new();
            this.OnInitialize?.Invoke();
        }

        public void SpawnSkillBook() {
            this.currentSp -= this.requiredSp;
            this.requiredSp += this.increasingSpPerSpawn;
            int slot = this.GetRandomSlot();
            SpawnedSkillBook skillBook = this.GetRandomSkillBook(mergeCount: 0);
            this.usingSlots[slot] = skillBook;
            this.OnChangeCurrentSp?.Invoke();
            this.OnChangeRequiredSp?.Invoke();
            this.OnSpawnSkillBook?.Invoke(slot);
        }

        public void MergeSkillBook(int slot1, int slot2) {
            SpawnedSkillBook prevSkillBook = this.usingSlots[slot1];
            SpawnedSkillBook skillBook = this.GetRandomSkillBook(mergeCount: prevSkillBook.mergeCount + 1);
            this.usingSlots[slot1] = skillBook;
            this.usingSlots[slot2] = null;
            this.remainingSlots.Add(slot2);
            this.OnMergeSkillBook?.Invoke(slot1, slot2);
        }

        public void UpgradeSkillBook(int skillBookId) {
            EquippedSkillBook equippedSkillBook = Array.Find(this._skillBooks, (e) => e.skillBook.id == skillBookId);
            SkillBookAdditionalUpgradeSpec skillBookUpgradeSpec = equippedSkillBook.GetCurrentUpgradeSpec();
            this.currentSp -= skillBookUpgradeSpec.requiredSp;
            equippedSkillBook.Upgrade();
            this.OnChangeCurrentSp?.Invoke();
            this.OnUpgradeSkillBook?.Invoke(skillBookId);
        }

        public void CastSkillBook(int slot) {
            this.OnCastSkillBook?.Invoke(slot);
        }

        public void CompleteCastingSkillBook(int slot) {
            this.OnCompleteCastingSkillBook?.Invoke(slot);
        }

        public void EarnSp(int sp) {
            this.currentSp += sp;
            this.OnChangeCurrentSp?.Invoke();
        }

        int GetRandomSlot() {
            int index = Random.Range(0, this.remainingSlots.Count);
            int slot = this.remainingSlots[index];
            this.remainingSlots.RemoveAt(index);
            return slot;
        }

        SpawnedSkillBook GetRandomSkillBook(int mergeCount) {
            EquippedSkillBook equippedSkillBook = this._skillBooks[Random.Range(0, this._skillBooks.Length)];
            var skill = equippedSkillBook.skillBook.skills[Mathf.Clamp(mergeCount, 0, equippedSkillBook.skillBook.skills.Length - 1)];
            var skillSpec = skill.spec;
            CastingSkillSpec castingSkillSpec = new CastingSkillSpec(
                id: skillSpec.id,
                coolTime: skillSpec.coolTime,
                duration: skillSpec.duration,
                targetCount: skillSpec.targetCount,
                damage: skillSpec.damage,
                damagePerLevel: skillSpec.damagePerLevel,
                hitCount: skillSpec.hitCount,
                delayPerHit: skillSpec.delayPerHit,
                debuffType: skillSpec.debuffType,
                debuffDuration: skillSpec.debuffDuration
            );
            CastingSkill castingSkill = new CastingSkill(
                prefab: skill.prefab,
                skillType: skill.type,
                skillSpec: castingSkillSpec
            );
            SpawnedSkillBook spawnedSkillBook = new SpawnedSkillBook(
                id: equippedSkillBook.skillBook.id,
                grade: equippedSkillBook.skillBook.grade,
                image: equippedSkillBook.skillBook.image,
                mergeCount,
                skill: castingSkill
            );
            return spawnedSkillBook;
        }
    }
}
