using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bonus : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private GameObject _healthObject = null;
    [SerializeField]
    private GameObject _bulletsObject = null;
    [SerializeField]
    private AudioSource _audioSource = null;
    [SerializeField]
    private AudioClip _audioHealth = null;
    [SerializeField]
    private AudioClip _audioBullets = null;
    #endregion

    #region Private Fields
    private const string PLAYER_TAG = "Player";
    
    private const int HEALTH_BONUS  = 15;
    private const int BULLETS_BONUS = 30;
    
    private BonusType _bonusType = BonusType.None;
    #endregion
    

    #region Private Methods
    private void Start()
    {
        SetBonusType((BonusType) Random.Range(1, 3));
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(PLAYER_TAG))
            AddBonus();
    }
    
    private void SetBonusType( BonusType bonusType )
    {
        _bonusType = bonusType;
        
        if(_bonusType == BonusType.Health)
            _healthObject.SetActive(true);
        else if (_bonusType == BonusType.Bullets)
            _bulletsObject.SetActive(true);
    }

    private void AddBonus()
    {
        if (_bonusType == BonusType.Health)
            AddHealth();
        else if (_bonusType == BonusType.Bullets)
            AddBullets();
        
        Hide();

        void AddHealth()
        {
            Player.instance.pickUpHealth(HEALTH_BONUS, out bool relevant);

            if (relevant)
                PlayAudio(_audioHealth);
        }

        void AddBullets()
        {
            AutomaticRifle.instance.pickUpBullets(BULLETS_BONUS, out bool relevant);
            
            if (relevant)
                PlayAudio(_audioBullets);
        }

        void PlayAudio( AudioClip audioClip )
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        void Hide()
        {
            transform.position = new Vector3(0.0f, -10.0f, 0.0f); // For play audio. Destroy by Cleaner.
        }
    }
    #endregion
}

#region Enum
public enum BonusType : byte
{
    None = 0 ,
    Health ,
    Bullets
}
#endregion
