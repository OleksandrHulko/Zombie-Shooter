using System;
using UnityEngine;

public class BulletsPool : MonoBehaviour
{
    #region Public Fields
    [NonSerialized]
    public GameObject[] bullets = new GameObject[COUNT];

    public static BulletsPool instance = null;
    #endregion

    #region Private Fields
    private const int COUNT = 64;
    private int counter = 0;
    #endregion

    #region Private Methods
    private void Start()
    {
        instance = this;
        
        GameObject bulletPrefab = ScriptableObjectsManager.instance._prefabContainer.bullet;

        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i] = Instantiate(bulletPrefab, transform);
            bullets[i].SetActive(false);
        }
    }
    #endregion

    #region Public Methods
    public GameObject getBullet( Vector3 position, Quaternion rotation )
    {
        GameObject bullet = bullets[counter];
        
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.SetActive(true);
        
        counter++;
        
        if (counter == COUNT)
            counter = 0;
        
        return bullet;
    }
    #endregion
}
