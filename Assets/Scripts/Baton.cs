using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Baton : MonoBehaviour
{
    #region Private Fields
    private static readonly string _playerTag = "Player";
    private const int MIN_DAMAGE = 6;
    private const int MAX_DAMAGE = 24;
    #endregion
    
    #region Private Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
            Player.instance.AddHealth(-Random.Range(MIN_DAMAGE, MAX_DAMAGE + 1));
    }
    #endregion
}
