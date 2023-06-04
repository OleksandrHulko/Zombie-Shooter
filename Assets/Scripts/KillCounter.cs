using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour , IUserInterface
{
    #region Serialize Fields
    [SerializeField]
    private Text _text = null;
    #endregion

    #region Private Fields
    private int counter = 0;
    private int normalFontSize = 35;
    private int maxFontSize = 45;
    #endregion
    
    #region Public Methods
    public void Init()
    {
        SetText();
        Zombie.onDie += SetText;
    }

    public void Deinit()
    {
        Zombie.onDie -= SetText;
    }
    #endregion

    #region Private Methods
    private void SetText()
    {
        _text.text = $"{Localization.ENEMY_KILLED} {counter++}";

        StartCoroutine(growEffect());

        IEnumerator growEffect()
        {
            int fontSize = normalFontSize;

            while (fontSize < maxFontSize)
                yield return grow(1);

            while (fontSize > normalFontSize)
                yield return grow(-1);

            IEnumerator grow( int value )
            {
                fontSize += value;
                _text.fontSize = fontSize;

                yield return new WaitForFixedUpdate();
            }
        }
    }
    #endregion
}
