using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Common
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private float _spawnInterval = 1f;
        [SerializeField] private float _spawnRadius = 1f;

        private float _spawnTime;
        
        private void Update()
        {
            _spawnTime -= Time.deltaTime;

            if (_spawnTime <= 0)
            {
                SpawnEnemy();
                _spawnTime = _spawnInterval;
            }
        }

        private void SpawnEnemy()
        {
            var prefab = GameManager.Instance.EnemyPrefab;
            var parent = GameManager.Instance.EnemyPool;
            
            var spawnPos = (Vector3)(Random.insideUnitCircle * _spawnRadius) + transform.position;

            var obj = Instantiate(prefab, parent);
            obj.transform.position = spawnPos;
        }

        private void OnDrawGizmos()
        {
            const float myTheta = 0.001f;

            // 设置矩阵
            Matrix4x4 defaultMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.yellow;

            // 绘制圆环
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;

            for (float theta = 0; theta < 2 * Mathf.PI; theta += myTheta)
            {
                float x = _spawnRadius * Mathf.Cos(theta);
                float y = _spawnRadius * Mathf.Sin(theta);
                Vector3 endPoint = new Vector3(x, y, 0);

                if (theta == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }

                beginPoint = endPoint;
            }

            // 绘制最后一条线段
            Gizmos.DrawLine(firstPoint, beginPoint);

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;
        }
    }
}