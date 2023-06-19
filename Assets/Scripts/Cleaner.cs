using System.Collections;
using UnityEngine;

public class Cleaner : MonoBehaviour
{
    #region Serialoze Fields
    [Header("Destroy after seconds")]
    [Range(1, 120)]
    [SerializeField]
    private int _seconds = 0;
    
    [SerializeField]
    private bool has_sprite = false;
    #endregion
    

    #region Private Methods
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_seconds);

        if (has_sprite)
            yield return TrySpriteMakeTransparent();
        
        Destroy(gameObject);

        IEnumerator TrySpriteMakeTransparent()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer)
            {
                Color color = spriteRenderer.color;

                while (color.a > 0)
                {
                    color.a -= Time.deltaTime;
                    spriteRenderer.color = color;
                    yield return null;
                }
            }
        }
    }
    #endregion
}
