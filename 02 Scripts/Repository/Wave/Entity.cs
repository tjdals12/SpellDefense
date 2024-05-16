using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Wave {
    public class SpawningEnemyEntity {
        public int id { get; private set; }
        public int level { get; private set; }
        public int count { get; private set; }
        public SpawningEnemyEntity(int id, int count, int level) {
            this.id = id;
            this.count = count;
            this.level = level;
        }
    }

    public class ClearRewardEntity {
        public int itemId { get; private set; }
        public int itemAmount { get; private set; }
        public ClearRewardEntity(int itemId, int itemAmount) {
            this.itemId = itemId;
            this.itemAmount = itemAmount;
        }
    }

    public class WaveEntity
    {
        public int startWave { get; private set; }
        public int endWave { get; private set; }
        public int minCountPerSpawn { get; private set; }
        public int maxCountPerSpawn { get; private set; }
        public float minSpawnDelay { get; private set; }
        public float maxSpawnDelay { get; private set; }
        public SpawningEnemyEntity[] spawningEnemies { get; private set; }
        public ClearRewardEntity[] clearRewards { get; private set; }
        public WaveEntity(int startWave, int endWave, int minCountPerSpawn, int maxCountPerSpawn, float minSpawnDelay, float maxSpawnDelay, SpawningEnemyEntity[] spawningEnemies, ClearRewardEntity[] clearRewards)
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

    public class WaveEntityBuilder {
        int startWave;
        int endWave;
        int minCountPerSpawn;
        int maxCountPerSpawn;
        float minSpawnDelay;
        float maxSpawnDelay;
        SpawningEnemyEntity[] spawningEnemies;
        ClearRewardEntity[] clearRewards;
        public WaveEntityBuilder SetStartWave(int startWave) {
            this.startWave = startWave;
            return this;
        }
        public WaveEntityBuilder SetEndWave(int endWave) {
            this.endWave = endWave;
            return this;
        }
        public WaveEntityBuilder SetMinCountPerSpawn(int minCountPerSpawn) {
            this.minCountPerSpawn = minCountPerSpawn;
            return this;
        }
        public WaveEntityBuilder SetMaxCountPerSpawn(int maxCountPerSpawn) {
            this.maxCountPerSpawn = maxCountPerSpawn;
            return this;
        }
        public WaveEntityBuilder SetMinSpawnDelay(float minSpawnDelay) {
            this.minSpawnDelay = minSpawnDelay;
            return this;
        }
        public WaveEntityBuilder SetMaxSpawnDelay(float maxSpawnDelay) {
            this.maxSpawnDelay = maxSpawnDelay;
            return this;
        }
        public WaveEntityBuilder SetSpawnEnemies(SpawningEnemyEntity[] spawningEnemies) {
            this.spawningEnemies = spawningEnemies;
            return this;
        }
        public WaveEntityBuilder SetClearRewards(ClearRewardEntity[] clearRewards) {
            this.clearRewards = clearRewards;
            return this;
        }
        public WaveEntity Build() {
            return new WaveEntity(
                this.startWave,
                this.endWave,
                this.minCountPerSpawn,
                this.maxCountPerSpawn,
                this.minSpawnDelay,
                this.maxSpawnDelay,
                this.spawningEnemies,
                this.clearRewards
            );
        }
    }
}
