using System.Collections.Generic;
using Lockstep.Math;
using Logic.Managers;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic
{
    public class EnemyManager : BaseManager
    {
        public List<Spawner> Spawners = new List<Spawner>();

        public static EnemyManager Instance { get; private set; }
        public List<Enemy> allEnemy = new List<Enemy>();

        public static int maxCount = 1;
        private static int curCount = 0;

        public override void DoAwake()
        {
            Instance = this;
        }

        public override void DoStart()
        {
            foreach (var spawner in Spawners)
            {
                spawner.OnSpawnEvent += OnSpawn;
                spawner.DoStart();
            }
        }

        public override void DoUpdate(LFloat deltaTime)
        {
            if (GameManager.player != null && GameManager.player.currentHealth <= 0f)
            {
                return;
            }

            foreach (var spawner in Spawners)
            {
                spawner.DoUpdate(deltaTime);
            }

            foreach (var enemy in allEnemy)
            {
                enemy.DoUpdate(deltaTime);
            }
        }

        private static int enemyID = 0;


        private void OnSpawn(int prefabId, LVector3 position)
        {
            if (curCount >= maxCount)
            {
                return;
            }

            curCount++;
            var entity = InstantiateEntity(prefabId, position);
            Instance.AddEnemy(entity as Enemy);
        }

        private object InstantiateEntity(int prefabId, LVector3 position)
        {
            var prefab = ResourceManager.LoadPrefab(prefabId);
            object config = ResourceManager.Instance.GetEnemyConfig(prefabId);
            Debug.Trace("CreateEnemy");
            var entity = new Enemy();
            // var obj = EntityView.CreateEntity(entity, prefabId, position, prefab, config);
        }

        private void AddEnemy(Enemy entity)
        {
            throw new System.NotImplementedException();
        }
    }
}