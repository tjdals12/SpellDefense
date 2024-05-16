using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using DamageNumbersPro;

namespace View.InGame {
    using DebuffType = Repository.SkillBook.DebuffType;
    using PlayModel = Model.InGame.PlayModel;
    using InGameController = Controller.InGame.InGameController;
    using LoadingPopup = View.Common.LoadingPopup;
    using Model.InGame;

    public class InGameManager : MonoBehaviour
    {
        PlayModel playModel;
        PlayerModel playerModel;
        InGameController inGameController;

        [Header("Sound")]
        [Space(4)]
        [SerializeField]
        SoundManager soundManager;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        RectTransform rootCanvasTransform;
        [SerializeField]
        RectTransform hpDamageCanvasTransform;
        [SerializeField]
        WavePopup wavePopup;
        [SerializeField]
        GameOverPopup gameOverPopup;

        [Header("Enemy")]
        [Space(4)]
        [SerializeField]
        Enemy.InstancePool enemyInstancePool;
        [SerializeField]
        Transform[] spawningPoints;
        [SerializeField]
        Enemy.HpBarPool enemyHpBarPool;
        [SerializeField]
        DamageNumber enemyDamageNumber;
        [SerializeField]
        DamageNumber enemyCriticalDamageNumber;
        [SerializeField]
        DamageNumber enemyPoisonDamageNumber;
        [SerializeField]
        DamageNumber enemyElectricDamageNumber;

        [Header("Player")]
        [Space(4)]
        [SerializeField]
        Player.Instance playerInstance;
        [SerializeField]
        DamageNumber playerDamageNumber;

        Camera mainCamera;

        Dictionary<string, Enemy.Instance> spawnedEnemies = new();
        bool isSpawning;
        Coroutine startWaveCoroutine;
        WaitForSeconds startWaveDelay;
        WaitForSeconds startSpawnDelay;

        LoadingPopup loadingPopup;

        #region Unity Method
        void Awake() {
            this.playModel = GameObject.FindObjectOfType<PlayModel>();
            this.playerModel = GameObject.FindObjectOfType<PlayerModel>();
            this.inGameController = GameObject.FindObjectOfType<InGameController>();
            this.loadingPopup = GameObject.FindObjectOfType<LoadingPopup>();
            this.mainCamera = Camera.main;

            this.startWaveDelay = new WaitForSeconds(0.5f);
            this.startSpawnDelay = new WaitForSeconds(2f);
        }
        void OnEnable() {
            this.playModel.OnInitialize += this.OnInitialize;
            this.playModel.OnNextWave += this.OnNextWave;
            this.playModel.OnGameOver += this.OnGameOver;
            this.playerModel.OnDead += this.OnDeadPlayer;
        }
        void OnDisable() {
            this.playModel.OnInitialize -= this.OnInitialize;
            this.playModel.OnNextWave -= this.OnNextWave;
            this.playModel.OnGameOver -= this.OnGameOver;
            this.playerModel.OnDead -= this.OnDeadPlayer;
        }
        #endregion

        #region Unity Method
        void OnInitialize() {
            if (this.startWaveCoroutine != null) {
                StopCoroutine(this.startWaveCoroutine);
            }
            this.startWaveCoroutine = StartCoroutine(this.StartWave());
        }
        void OnNextWave() {
            if (this.startWaveCoroutine != null) {
                StopCoroutine(this.startWaveCoroutine);
            }
            this.startWaveCoroutine = StartCoroutine(this.StartWave());
        }
        void OnDeadPlayer() {
            this.loadingPopup.Open();
            if (this.startWaveCoroutine != null) {
                StopCoroutine(this.startWaveCoroutine);
            }
            foreach (var enemy in this.spawnedEnemies.Values) {
                Destroy(enemy.gameObject);
            }
            this.inGameController.GameOver();
        }
        void OnGameOver() {
            this.loadingPopup.Close();
            this.gameOverPopup.Open(
                wave: this.playModel.currentWaveNumber,
                rewardGold: this.playModel.rewardGold,
                rewardSilverKey: this.playModel.rewardSilverKey,
                playSeconds: (int)(this.playModel.endTime - this.playModel.startTime).TotalSeconds,
                depeatedEnemy: this.playModel.depeatedEnemy
            );
        }
        #endregion

        IEnumerator StartWave() {
            this.enemyInstancePool.Initialize();
            this.isSpawning = true;
            yield return this.startWaveDelay;
            this.wavePopup.Open(this.playModel.currentWaveNumber);
            this.soundManager.StartWave();
            yield return this.startSpawnDelay;
            Wave wave = this.playModel.currentWave;
            List<SpawningEnemy> spawningEnemies = wave.spawningEnemies.ToList();
            Dictionary<int, int> remainingCounts = new();
            for (int i = 0; i < spawningEnemies.Count; i++) {
                remainingCounts[spawningEnemies[i].enemy.id] = spawningEnemies[i].count;
            }
            int currentSpawnCount = 0;
            Transform[] currentSpawningPoints = null;
            SpawningEnemy spawningEnemy = null;
            while (remainingCounts.Count > 0) {
                int countPerSpawn = Random.Range(wave.minCountPerSpawn, wave.maxCountPerSpawn + 1);
                this.Shuffle(this.spawningPoints);
                currentSpawningPoints = this.spawningPoints.Take(countPerSpawn).ToArray();
                for (int i = 0; i < countPerSpawn; i++) {
                    if (remainingCounts.Count == 0) {
                        break;
                    }
                    spawningEnemy = spawningEnemies[Random.Range(0, spawningEnemies.Count)];
                    remainingCounts[spawningEnemy.enemy.id]--;
                    Enemy.Instance enemyInstance = this.enemyInstancePool.GetObject(spawningEnemy.enemy.id);
                    enemyInstance.transform.position = spawningPoints[i].position + (Vector3.right * Random.Range(-1f, 1f));
                    Enemy.HpBar enemyHpBar = this.enemyHpBarPool.GetObject();
                    enemyHpBar.gameObject.SetActive(false);
                    string id = $"Enemy_{++currentSpawnCount}";
                    enemyInstance.Initialize(
                        id: id,
                        level: spawningEnemy.level,
                        enemy: spawningEnemy.enemy,
                        hpBar: enemyHpBar,
                        onAttack: this.OnDamagePlayer,
                        onDamage: this.OnDamageEnemy,
                        onDebuffDamage: this.OnDebuffDamageEnemy,
                        onDead: this.OnDeadEnemy
                    );
                    this.spawnedEnemies[id] = enemyInstance;
                    if (remainingCounts[spawningEnemy.enemy.id] == 0) {
                        spawningEnemies.Remove(spawningEnemy);
                        remainingCounts.Remove(spawningEnemy.enemy.id);
                    }
                }
                yield return new WaitForSeconds(Random.Range(wave.minSpawnDelay, wave.maxSpawnDelay));
            }
            this.isSpawning = false;
        }

        void Shuffle<T>(T[] array) {
            for (int i = 0; i < array.Length; i++) {
                int a = Random.Range(0, array.Length);
                int b = Random.Range(0, array.Length);
                T temp = array[a];
                array[a] = array[b];
                array[b] = temp;
            }
        }

        Vector2 GetScreenPosition(Vector3 worldPosition) {
            Vector2 viewPoint = this.mainCamera.WorldToViewportPoint(worldPosition);
            Vector2 position = new Vector2(viewPoint.x * this.rootCanvasTransform.sizeDelta.x, viewPoint.y * this.rootCanvasTransform.sizeDelta.y);
            return position;
        }

        void OnDamageEnemy(string id, int damage, bool isCritical) {
            if (this.spawnedEnemies.ContainsKey(id)) {
                this.soundManager.HitEnenmy();
                Enemy.Instance enemyInstance = this.spawnedEnemies[id];
                Vector2 position = this.GetScreenPosition(enemyInstance.transform.position);
                if (isCritical) {
                    this.enemyCriticalDamageNumber.SpawnGUI(this.hpDamageCanvasTransform, position, damage);
                } else {
                    this.enemyDamageNumber.SpawnGUI(this.hpDamageCanvasTransform, position, damage);
                }
            }
        }

        void OnDebuffDamageEnemy(string id, DebuffType debuffType, int damage) {
            if (this.spawnedEnemies.ContainsKey(id)) {
                this.soundManager.HitEnenmy();
                DamageNumber damageNumber = this.enemyDamageNumber;
                switch (debuffType) {
                    case DebuffType.Poisoned:
                        damageNumber = this.enemyPoisonDamageNumber;
                        break;
                    case DebuffType.Electrified:
                        damageNumber = this.enemyElectricDamageNumber;
                        break;
                }
                Enemy.Instance enemyInstance = this.spawnedEnemies[id];
                Vector2 position = this.GetScreenPosition(enemyInstance.transform.position);
                damageNumber.SpawnGUI(this.hpDamageCanvasTransform, position, damage);
            }
        }

        void OnDeadEnemy(string id, int enemyId, int dropSp) {
            if (this.spawnedEnemies.TryGetValue(id, out Enemy.Instance enemyInstance)) {
                this.enemyInstancePool.ReturnObject(enemyId, enemyInstance);
                this.spawnedEnemies.Remove(id);
                this.inGameController.DepeatEnemy(dropSp);
            }
            if (0 >= this.spawnedEnemies.Count && this.isSpawning == false) {
                this.inGameController.ClearWave();
            }
        }

        void OnDamagePlayer(string id, int damage) {
            if (this.spawnedEnemies.ContainsKey(id)) {
                this.soundManager.HitPlayer();
                Enemy.Instance enemyInstance = this.spawnedEnemies[id];
                Vector2 enemyViewPoint = this.mainCamera.WorldToViewportPoint(enemyInstance.transform.position);
                Vector2 playerViewPoint = this.mainCamera.WorldToViewportPoint(this.playerInstance.transform.position);
                Vector2 position = new Vector2(enemyViewPoint.x * this.rootCanvasTransform.sizeDelta.x, playerViewPoint.y * this.rootCanvasTransform.sizeDelta.y);
                this.playerDamageNumber.SpawnGUI(this.hpDamageCanvasTransform, position, damage);
            }
        }
    }
}
