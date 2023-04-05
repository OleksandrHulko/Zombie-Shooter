using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform _spawnTransform_1 = null;
    [SerializeField]
    private Transform _spawnTransform_2 = null;
    [SerializeField]
    private Transform _spawnTransform_3 = null;
    [SerializeField]
    private Transform _spawnTransform_4 = null;
    #endregion
    
    #region Private Fields
    private const int MAX_ENEMY_COUNT_ON_SCENE = 50;
    private const int TIME_TO_SPAWN = 2;

    private int enemy_count_on_scene = 0;
    
    private Transform _playerTransform = null;
    
    private Vector3 _spawnVector_1 = Vector3.zero;
    private Vector3 _spawnVector_2 = Vector3.zero;
    private Vector3 _spawnVector_3 = Vector3.zero;
    private Vector3 _spawnVector_4 = Vector3.zero;
    #endregion
    
    #region Private Methods
    private IEnumerator Start()
    {
        _spawnVector_1 = _spawnTransform_1.position;
        _spawnVector_2 = _spawnTransform_2.position;
        _spawnVector_3 = _spawnTransform_3.position;
        _spawnVector_4 = _spawnTransform_4.position;
        
        _playerTransform = Player.instance.transform;
        
        Zombie.onDie += OnDie;

        yield return SpawnEnemy();
    }

    private void OnDestroy()
    {
        Zombie.onDie -= OnDie;
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(TIME_TO_SPAWN);

            if (enemy_count_on_scene < MAX_ENEMY_COUNT_ON_SCENE)
                Spawn();
        }

        void Spawn()
        {
            Instantiate(ScriptableObjectsManager.instance._prefabContainer.zombie, GetSpawnVector(), Quaternion.identity, transform);
            enemy_count_on_scene++;
        }
    }

    private void OnDie()
    {
        enemy_count_on_scene--;
    }

    private Vector3 GetSpawnVector()
    {
        Vector3 position = _playerTransform.position;
        
        float x = position.x;
        float z = position.z;

        if (x >= 0 && z >= 0)
            return _spawnVector_3;
        if (x >= 0 && z < 0)
            return _spawnVector_4;
        if (x < 0 && z < 0)
            return _spawnVector_1;

        return _spawnVector_2;
    }
    #endregion
}
