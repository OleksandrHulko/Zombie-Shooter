using UnityEngine;
[CreateAssetMenu(fileName = "PrefabContainer", menuName = "ScriptableObjects/PrefabContainer")]
public class PrefabContainer : ScriptableObject
{
    #region Serialize Fields
    [Header("Bullet")]
    public GameObject bullet = null;
    
    [Header("BulletHoleWithParticles")]
    public GameObject bulletHole = null;
    
    [Header("Bonuses")]
    public GameObject bonuses = null;
    
    [Header("Zombie")]
    public GameObject zombie = null;
    #endregion
}
