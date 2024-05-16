using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.SkillBook {
    using LocalizationRepository = Repository.Localization.IRepository;
    using SO;
    using Model.Common;

    public class DataTableRepository : IRepository
    {
        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IRepository>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            StartCoroutine(this.LoadAfterResolveDependencies());
        }
        #endregion

        IEnumerator LoadAfterResolveDependencies() {
            LocalizationRepository localizationRepository = GameObject.FindObjectOfType<LocalizationRepository>();
            while (localizationRepository.isLoaded == false) {
                yield return null;
            }
            this.itemImageSO = Resources.Load<ItemImageSO>("SO/ItemImage");
            this.skillPrefabSO = Resources.Load<SkillPrefabSO>("SO/SkillPrefab");
            SkillBookUpgradeSpecEntity[] skillBookUpgradeSpecEntities = this.LoadSkillBookUpgradeSpec();
            List<SkillBookUpgradeSpec> skillBookUpgradeSpecs = new();
            foreach (var entity in skillBookUpgradeSpecEntities) {
                SkillBookUpgradeSpec skillBookUpgradeSpec = new SkillBookUpgradeSpec(
                    grade: entity.grade,
                    level: entity.level,
                    requiredGold: entity.requiredGold,
                    requiredAmount: entity.requiredAmount
                );
                skillBookUpgradeSpecs.Add(skillBookUpgradeSpec);
            }
            SkillSpecEntity[] skillSpecEntities = this.LoadSkillSpec();
            List<SkillSpec> skillSpecs = new();
            foreach (var entity in skillSpecEntities) {
                SkillSpec skillSpec = new SkillSpec(
                    id: entity.id,
                    duration: entity.duration,
                    targetCount: entity.targetCount,
                    damage: entity.damage,
                    damagePerLevel: entity.damagePerLevel,
                    hitCount: entity.hitCount,
                    delayPerHit: entity.delayPerHit,
                    debuffType: entity.debuffType,
                    debuffDuration: entity.debuffDuration,
                    coolTime: entity.coolTime
                );
                skillSpecs.Add(skillSpec);
            }
            SkillEntity[] skillEntities = this.LoadSkill(localizationRepository);
            List<Skill> skills = new();
            foreach (var entity in skillEntities) {
                SkillPrefab skillPrefab = this.skillPrefabSO.FindById(entity.id);
                SkillSpec spec = skillSpecs.Find((e) => e.id == entity.id);
                Skill skill = new Skill(
                    id: entity.id,
                    name: entity.name,
                    type: entity.type,
                    spec: spec,
                    prefab: skillPrefab.prefab
                );
                skills.Add(skill);
            }
            SkillBookEntity[] skillBookEntities = this.LoadSkillBook(localizationRepository);
            List<SkillBook> skillBooks = new();
            foreach (var entity in skillBookEntities) {
                ItemImage itemImage = this.itemImageSO.FindById(entity.id);
                SkillBookUpgradeSpec[] currentUpgradeSpecs = skillBookUpgradeSpecs.FindAll((e) => e.grade == (SkillBookGrade)entity.grade).ToArray();
                Skill[] currentSkills = skills.FindAll((e1) => Array.Exists(entity.skills, (e2) => e2 == e1.id)).ToArray();
                SkillBook skillBook = new SkillBook(
                    id: entity.id,
                    grade: entity.grade,
                    name: entity.name,
                    description: entity.description,
                    gradeName: entity.gradeName,
                    image: itemImage.image,
                    upgradeSpecs: currentUpgradeSpecs,
                    skills: currentSkills
                );
                skillBooks.Add(skillBook);
            }
            this.skillBooks = skillBooks.ToArray();
        }

        SkillBookEntity[] LoadSkillBook(LocalizationRepository localizationRepository) {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/SkillBook");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "SkillBook.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<SkillBookEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                SkillBookEntityBuilder builder = new SkillBookEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        builder.SetId(int.Parse(value));
                    } else if (name == "name") {
                        string localizedName = localizationRepository.GetLocalizedText(value);
                        builder.SetName(localizedName);
                    } else if (name == "description") {
                        string localizedDescription = localizationRepository.GetLocalizedText(value);
                        builder.SetDescription(localizedDescription);
                    } else if (name == "gradeName") {
                        string localizedGradeName = localizationRepository.GetLocalizedText(value);
                        builder.SetGradeName(localizedGradeName);
                    } else if (name == "grade") {
                        builder.SetGrade((SkillBookGrade)int.Parse(value));
                    } else if (name == "skill1") {
                        builder.SetSkill1(int.Parse(value));
                    } else if (name == "skill2") {
                        builder.SetSkill2(int.Parse(value));
                    } else if (name == "skill3") {
                        builder.SetSkill3(int.Parse(value));
                    } else if (name == "skill4") {
                        builder.SetSkill4(int.Parse(value));
                    } else if (name == "skill5") {
                        builder.SetSkill5(int.Parse(value));
                    }
                }
                SkillBookEntity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }
        SkillBookUpgradeSpecEntity[] LoadSkillBookUpgradeSpec() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/SkillBookUpgradeSpec");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "SkillBookUpgradeSpec.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<SkillBookUpgradeSpecEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                SkillBookUpgradeSpecEntityBuilder builder = new SkillBookUpgradeSpecEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "grade") {
                        builder.SetGrade((SkillBookGrade)int.Parse(value));
                    } else if (name == "level") {
                        builder.SetLevel(int.Parse(value));
                    } else if (name == "requiredGold") {
                        builder.SetRequiredGold(int.Parse(value));
                    } else if (name == "requiredAmount") {
                        builder.SetRequiredAmount(int.Parse(value));
                    }
                }
                SkillBookUpgradeSpecEntity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }
        SkillEntity[] LoadSkill(LocalizationRepository localizationRepository) {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/Skill");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "Skill.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<SkillEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                SkillEntityBuilder builder = new SkillEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        builder.SetId(int.Parse(value));
                    } else if (name == "name") {
                        string localizedName = localizationRepository.GetLocalizedText(value);
                        builder.SetName(localizedName);
                    } else if (name == "type") {
                        builder.SetType((SkillType)int.Parse(value));
                    }
                }
                SkillEntity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }

        SkillSpecEntity[] LoadSkillSpec() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/SkillSpec");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "SkillSpec.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<SkillSpecEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                SkillSpecEntityBuilder builder = new SkillSpecEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        builder.SetId(int.Parse(value));
                    } else if (name == "duration") {
                        builder.SetDuration(float.Parse(value));
                    } else if (name == "targetCount") {
                        builder.SetTargetCount(int.Parse(value));
                    } else if (name == "damage") {
                        builder.SetDamage(float.Parse(value));
                    } else if (name == "damagePerLevel") {
                        builder.SetDamagePerLevel(float.Parse(value));
                    } else if (name == "hitCount") {
                        builder.SetHitCount(int.Parse(value));
                    } else if (name == "delayPerHit") {
                        builder.SetDelayPerHit(float.Parse(value));
                    } else if (name == "debuffType") {
                        builder.SetDebuffType((DebuffType)int.Parse(value));
                    } else if (name == "debuffDuration") {
                        builder.SetDebuffDuration(float.Parse(value));
                    } else if (name == "coolTime") {
                        builder.SetCoolTime(int.Parse(value));
                    }
                }
                SkillSpecEntity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }

        public override SkillBook FindById(int id)
        {
            return Array.Find(this.skillBooks, (e) => e.id == id);
        }
    }
}
