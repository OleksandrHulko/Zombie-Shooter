using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    #region Private Fields
    private const float LIFE_TIME = 5.0f;
    private const int DAMAGE = 15;

    private string TAG_NAME = "ForBulletHoles";
    #endregion
    
    #region Private Methods
    private void OnEnable()
    {
        StartCoroutine(destroy());
        
        IEnumerator destroy()
        {
            yield return new WaitForSeconds(LIFE_TIME);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        bool is_static = other.collider.CompareTag(TAG_NAME);

        if (is_static)
            spawnHole();
        else
            tryDamageZombie();

        gameObject.SetActive(false);

        void spawnHole()
        {
            ContactPoint contactPoint = other.contacts[0];
            Vector3 position = contactPoint.point + contactPoint.normal.normalized * 1e-3f;
            Quaternion rotation = Quaternion.LookRotation(contactPoint.normal) * Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));

            Instantiate(ScriptableObjectsManager.instance._prefabContainer.bulletHole, position, rotation);
        }

        void tryDamageZombie()
        {
            Zombie zombie = (Zombie) other.gameObject.GetComponentInParent(typeof(Zombie));

            if (zombie)
                zombie.GetDamaged(DAMAGE, other.gameObject.layer);
        }
    }
    #endregion
}
