using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour , IUserInterface
{
    #region Serialize Fields
    [SerializeField]
    private Text _text = null;
    [SerializeField]
    private Image _image = null;
    #endregion

    #region Public Methods
    public void Init()
    {
        Player.onHealthChanged += SetHealth;
    }

    public void Deinit()
    {
        Player.onHealthChanged -= SetHealth;
    }
    #endregion

    #region Private Methods
    private void SetHealth(int health)
    {
        float ratio = (float) health / Player.MAX_HEALTH;
        
        _text.text = health.ToString();
        _image.fillAmount = ratio;
        _image.color = new Color(1.0f - ratio, ratio, 0.0f);
    }
    #endregion
}
