using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Part {
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
            PartSpecEntity[] partSpecEntities = this.LoadPartSpec();
            List<PartSpec> partSpecs = new();
            foreach (var entity in partSpecEntities) {
                PartSpec partSpec = new PartSpec(
                    id: entity.id,
                    attackPower: entity.attackPower,
                    attackPowerPerLevel: entity.attackPowerPerLevel,
                    attackSpeed: entity.attackSpeed,
                    attackSpeedPerLevel: entity.attackSpeedPerLevel,
                    criticalRate: entity.criticalRate,
                    criticalRatePerLevel: entity.criticalRatePerLevel,
                    criticalDamage: entity.criticalDamage,
                    criticalDamagePerLevel: entity.criticalDamagePerLevel
                );
                partSpecs.Add(partSpec);
            }
            PartUpgradeSpecEntity[] partUpgradeSpecEntities = this.LoadPartUpgradeSpec();
            List<PartUpgradeSpec> partUpgradeSpecs = new();
            foreach (var entity in partUpgradeSpecEntities) { PartUpgradeSpec partUpgradeSpec = new PartUpgradeSpec(
                    grade: (PartGrade)entity.grade,
                    level: entity.level,
                    requiredGold: entity.requiredGold,
                    requiredExp: entity.requiredExp
                );
                partUpgradeSpecs.Add(partUpgradeSpec);
            }
            PartEntity[] partEntities = this.LoadPart(localizationRepository);
            List<Part> parts = new();
            foreach (var entity in partEntities) {
                ItemImage itemImage = this.itemImageSO.FindById(entity.id);
                PartUpgradeSpec[] currentUpgradeSpecs = partUpgradeSpecs.FindAll((e) => e.grade == (PartGrade)entity.grade).ToArray();
                PartSpec currentSpec = partSpecs.Find((e) => e.id == entity.id);
                Part part = new Part(
                    id: entity.id,
                    grade: (PartGrade)entity.grade,
                    gradeName: entity.gradeName,
                    type: (PartType)entity.type,
                    name: entity.name,
                    image: itemImage.image,
                    upgradeSpecs: currentUpgradeSpecs,
                    spec: currentSpec
                );
                parts.Add(part);
            }
            this.parts = parts.ToArray();
        }

        PartEntity[] LoadPart(LocalizationRepository localizationRepository) {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/Part");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "Part.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<PartEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                PartEntityBuilder builder = new PartEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        builder.SetId(int.Parse(value));
                    } else if (name == "name") {
                        string localizedName = localizationRepository.GetLocalizedText(value);
                        builder.SetName(localizedName);
                    } else if (name == "gradeName") {
                        string localizedGradeName = localizationRepository.GetLocalizedText(value);
                        builder.SetGradeName(localizedGradeName);
                    } else if (name == "grade") {
                        builder.SetGrade((PartGrade)int.Parse(value));
                    } else if (name == "type") {
                        builder.SetType((PartType)int.Parse(value));
                    }
                }
                PartEntity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }

        PartUpgradeSpecEntity[] LoadPartUpgradeSpec() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/PartUpgradeSpec");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "PartUpgradeSpec.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<PartUpgradeSpecEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                PartUpgradeSpecEntityBuilder builder = new PartUpgradeSpecEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "grade") {
                        builder.SetGrade(int.Parse(value));
                    } else if (name == "level") {
                        builder.SetLevel(int.Parse(value));
                    } else if (name == "requiredGold") {
                        builder.SetRequiredGold(int.Parse(value));
                    } else if (name == "requiredExp") {
                        builder.SetRequiredExp(int.Parse(value));
                    }
                }
                PartUpgradeSpecEntity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }

        PartSpecEntity[] LoadPartSpec() {
            #if UNITY_EDITOR
                TextAsset textAsset = Resources.Load<TextAsset>("DataTable/PartSpec");
                string text = textAsset.text;
            #else
                string filePath = Path.Combine(Application.persistentDataPath, "DataTable", "PartSpec.csv");
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string text = Encoding.UTF8.GetString(fileBytes);
            #endif
            string[] rows = text.Split("\n");
            string[] names = rows[0].Split(",");
            List<PartSpecEntity> list = new();
            for (int i = 2; i < rows.Length; i++) {
                string[] columns = rows[i].Split(",");
                if (columns.Length != names.Length) continue;
                PartSpecEntityBuilder builder = new PartSpecEntityBuilder();
                for (int j = 0; j < columns.Length; j++) {
                    string name = names[j];
                    string value = columns[j];
                    if (name == "id") {
                        builder.SetId(int.Parse(value));
                    } else if (name == "attackPower") {
                        builder.SetAttackPower(int.Parse(value));
                    } else if (name == "attackPowerPerLevel") {
                        builder.SetAttackPowerPerLevel(int.Parse(value));
                    } else if (name == "attackSpeed") {
                        builder.SetAttackSpeed(float.Parse(value));
                    } else if (name == "attackSpeedPerLevel") {
                        builder.SetAttackSpeedPerLevel(float.Parse(value));
                    } else if (name == "criticalRate") {
                        builder.SetCriticalRate(int.Parse(value));
                    } else if (name == "criticalRatePerLevel") {
                        builder.SetCriticalRatePerLevel(int.Parse(value));
                    } else if (name == "criticalDamage") {
                        builder.SetCriticalDamage(int.Parse(value));
                    } else if (name == "criticalDamagePerLevel") {
                        builder.SetCriticalDamagePerLevel(int.Parse(value));
                    }
                }
                PartSpecEntity item = builder.Build();
                list.Add(item);
            }
            return list.ToArray();
        }

        public override Part FindById(int id)
        {
            return Array.Find(this.parts, (e) => e.id == id);
        }
    }
}
