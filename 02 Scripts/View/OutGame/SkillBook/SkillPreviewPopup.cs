using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DamageNumbersPro;

namespace View.OutGame.SkillBook {
    using DebuffType = Repository.SkillBook.DebuffType;
    using InventorySkillBook = Model.OutGame.IInventorySkillBook;
    using Enemy = Model.InGame.Enemy;
    using CastingSkillSpec = Model.InGame.CastingSkillSpec;
    using EnemyInstance = InGame.Enemy.Instance;
    using ISkill = InGame.Skill.ISkill;
    using HpBar = InGame.Enemy.HpBar;

    public class SkillPreviewPopup : MonoBehaviour
    {
        [Header("Audio")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        GameObject popup;
        [SerializeField]
        Image backgroundImage;
        [SerializeField]
        GameObject window;
        [SerializeField]
        TextMeshProUGUI skillNameText;
        [SerializeField]
        RectTransform rootCanvasTransform;
        [SerializeField]
        RectTransform damagePanelTransform;
        [SerializeField]
        Button topCloseButton;
        [SerializeField]
        Button bottomCloseButton;

        [Header("Preview")]
        [Space(4)]
        [SerializeField]
        GameObject preview;
        [SerializeField]
        HpBar enemyHpBar;
        [SerializeField]
        EnemyInstance enemyInstance;
        [SerializeField]
        DamageNumber enemyDamageNumber;
        [SerializeField]
        DamageNumber enemyCriticalDamageNumber;
        [SerializeField]
        DamageNumber enemyPoisonDamageNumber;
        [SerializeField]
        DamageNumber enemyElectifiedDamageNumber;
        [SerializeField]
        Animator playerAnimator;

        Camera mainCamera;
        WaitForSeconds castingDelay;
        WaitForSeconds playerAnimationDelay;
        bool isCasting;
        bool isCastComplete;
        Coroutine castSkillCoroutine;
        GameObject currentSkill;

        #region Unity Method
        void Awake() {
            this.topCloseButton.onClick.AddListener(this.Close);
            this.bottomCloseButton.onClick.AddListener(this.Close);
            this.castingDelay = new WaitForSeconds(1f);
            this.playerAnimationDelay = new WaitForSeconds(0.5f);
            this.isCasting = false;
            this.isCastComplete = false;
            this.mainCamera = Camera.main;
        }
        #endregion

        public void Open(InventorySkillBook inventorySkillBook) {
            this.preview.SetActive(true);
            this.isCasting = true;
            this.isCastComplete = false;
            var skillInfo = inventorySkillBook.skillBook.skills[0];
            this.skillNameText.text = skillInfo.name;
            enemyInstance.Initialize(
                id: "Enemy",
                level: 1,
                enemy: new Enemy(
                    id: 0,
                    name: "Enemy",
                    hp: 100,
                    hpPerLevel: 0,
                    attackPower: 0,
                    attackPowerPerLevel: 0,
                    attackSpeed: 0,
                    moveSpeed: 0,
                    dropSp: 0,
                    prefab: null
                ),
                hpBar: this.enemyHpBar,
                onAttack: null,
                onDamage: this.OnDamage,
                onDebuffDamage: this.OnDebuffDamage,
                onDead: null,
                isSimulate: true
            );
            this.popup.SetActive(true);
            DOTween.Sequence()
                .Join(this.backgroundImage.DOFade(0.8f, 0.1f).From(0))
                .Join(this.window.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack))
                .OnComplete(() => {
                    if (this.castSkillCoroutine != null) {
                        StopCoroutine(this.castSkillCoroutine);
                    }
                    this.castSkillCoroutine = StartCoroutine(this.CastSkill(skillInfo));
                });
        }

        void Close() {
            this.soundManager.Close();
            this.isCasting = false;
            this.isCastComplete = false;
            if (this.castSkillCoroutine != null) {
                StopCoroutine(this.castSkillCoroutine);
                this.castSkillCoroutine = null;
            }
            if (this.currentSkill != null) {
                Destroy(this.currentSkill);
            }
            this.preview.SetActive(false);
            this.popup.SetActive(false);
        }

        Vector2 GetScreenPosition(Vector3 worldPosition) {
            Vector2 viewPoint = this.mainCamera.WorldToViewportPoint(worldPosition);
            Vector2 position = new Vector2(viewPoint.x * this.rootCanvasTransform.sizeDelta.x, viewPoint.y * this.rootCanvasTransform.sizeDelta.y);
            return position;
        }

        void OnDamage(string id, int damage, bool isCritical) {
            if (isCritical) {
                this.enemyCriticalDamageNumber.SpawnGUI(this.damagePanelTransform, Vector2.zero, 150);
            } else {
                this.enemyDamageNumber.SpawnGUI(this.damagePanelTransform, Vector2.zero, 100);
            }
        }

        void OnDebuffDamage(string id, DebuffType debuffType, int damage) {
            DamageNumber damageNumber = this.enemyDamageNumber;
            switch (debuffType) {
                case DebuffType.Poisoned:
                    damageNumber = this.enemyPoisonDamageNumber;
                    break;
                case DebuffType.Electrified:
                    damageNumber = this.enemyElectifiedDamageNumber;
                    break;
            }
            Vector2 position = this.GetScreenPosition(this.enemyInstance.transform.position);
            damageNumber.SpawnGUI(this.damagePanelTransform, Vector2.zero, 50);
        }

        IEnumerator CastSkill(Model.Common.Skill skillInfo) {
            while (this.isCasting) {
                this.playerAnimator.SetTrigger("Attack");
                yield return this.playerAnimationDelay;
                this.currentSkill = Instantiate(skillInfo.prefab);
                ISkill skill = this.currentSkill.GetComponent<ISkill>();
                skill
                    .SetTarget(this.enemyInstance)
                    .SetPosition(this.enemyInstance.transform.position)
                    .SetLevel(0)
                    .SetAttackPower(0)
                    .SetCriticalDamage(0)
                    .SetCriticalRate(50)
                    .SetSkillSpec(new CastingSkillSpec(
                        id: skillInfo.spec.id,
                        coolTime: skillInfo.spec.coolTime,
                        duration: skillInfo.spec.duration,
                        targetCount: skillInfo.spec.targetCount,
                        damage: skillInfo.spec.damage,
                        damagePerLevel: skillInfo.spec.damagePerLevel,
                        hitCount: skillInfo.spec.hitCount,
                        delayPerHit: skillInfo.spec.delayPerHit,
                        debuffType: skillInfo.spec.debuffType,
                        debuffDuration: skillInfo.spec.debuffDuration
                    ))
                    .OnSuccess(() => {
                        this.isCastComplete = true;
                    })
                    .Use();
                while (this.isCastComplete == false) {
                    yield return null;
                }
                this.isCastComplete = false;
                yield return this.castingDelay;
            }
        }
    }
}
