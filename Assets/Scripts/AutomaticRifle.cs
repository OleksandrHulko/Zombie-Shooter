using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AutomaticRifle : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField] 
    private Animator _animator = null;
    
    [Header("Audio")]
    
    [SerializeField]
    private AudioSource _shootAudio = null;
    
    [SerializeField]
    private AudioSource _reloadAudio = null;
    
    [Header("Shoot Audio")]
    [SerializeField]
    private AudioClip _audioShoot = null;
    
    [SerializeField]
    private AudioClip _audioBlankShoot = null;
    
    [Space]
    
    [SerializeField]
    private ParticleSystem _shootParticleSystem = null;

    [SerializeField]
    private Transform _spawnPointTransform = null;

    [Header("Bullet speed m/s")]
    [SerializeField]
    private float _bulletSpeed = 0.0f;
    #endregion
    
    #region Private Fields
    private static readonly int _aim    = Animator.StringToHash("Aim");
    private static readonly int _reload = Animator.StringToHash("Reload");
    private static readonly int _fire   = Animator.StringToHash("Fire");

    private const string AIM       = "Aim";
    private const string IDLE      = "Idle";
    private const string RELOAD    = "Reload";
    private const string N_A_SHOOT = "NotAimedShoot";
    private const string A_SHOOT   = "AimedShoot";
    
    private const int MAGAZINE_CASE_CAPACITY     = 30;
    private const int MAX_BULLETS_COUNT_IN_STOCK = 120;

    private int _insertedBullets = 0;
    private int _inStockBullets = 0;

    private bool _is_shooting = false;
    private bool _is_reload   = false;
    #endregion

    #region Public Fields
    public static Action<int> onInsertedBulletsCountChanged = null;
    public static Action<int> onInStockBulletsCountChanged  = null;

    public static AutomaticRifle instance = null;
    #endregion

    #region Unity Methods
    private void Start()
    {
        instance = this;
        
        AddInsertedBullets(MAGAZINE_CASE_CAPACITY);
        AddInStockBullets(MAX_BULLETS_COUNT_IN_STOCK);
    }
    #endregion

    #region Public Methods
    public void Aiming(bool is_finish_aiming = false)
    {
        _animator.SetInteger(_aim, is_finish_aiming ? 2 : 1);
    }

    public void Reload()
    {
        bool isMagazineCaseFull   = _insertedBullets == MAGAZINE_CASE_CAPACITY;
        bool isInStockBulletsNone = _inStockBullets == 0;

        if (_is_shooting || isMagazineCaseFull || isInStockBulletsNone)
            return;
        
        StartCoroutine(reload());

        IEnumerator reload()
        {
            if (IsAnimationPlaying(RELOAD))
                yield break;
            
            _is_reload = true;

            _animator.SetInteger(_aim, 2);

            yield return new WaitUntil((() => IsAnimationPlaying(IDLE)));

            _animator.SetInteger(_reload, 1);

            yield return new WaitUntil((() => IsAnimationPlaying(RELOAD)));
            
            _reloadAudio.Play();

            calcBullets();
            
            _animator.SetInteger(_reload, 0);
            _animator.SetInteger(_fire, 0);

            _is_reload = false;

            void calcBullets()
            {
                int reloadCountNeeded = MAGAZINE_CASE_CAPACITY - _insertedBullets;
                int reloadCount = _inStockBullets >= reloadCountNeeded ? reloadCountNeeded : _inStockBullets;

                AddInStockBullets(-reloadCount);
                AddInsertedBullets(reloadCount);
            }
        }
    }

    public void Shooting()
    {
        if ( _insertedBullets == 0)
        {
            if (!_shootAudio.isPlaying)
            {
                _shootAudio.clip = _audioBlankShoot;
                _shootAudio.Play();
            }

            return;
        }
        
        if (_is_shooting || _is_reload || IsInTransition())
            return;
        
        StartCoroutine(shoot());

        IEnumerator shoot()
        {
            _is_shooting = true;
            
            bool isIdleAnimation = IsAnimationPlaying(IDLE);
            bool isAimAnimation  = IsAnimationPlaying(AIM);
            
            if (isIdleAnimation || isAimAnimation)
            {
                _animator.SetInteger(_fire, 1);
                
                yield return new WaitUntil((() => ClipInfos().Any(x => x.clip.name == N_A_SHOOT || x.clip.name == A_SHOOT)));

                playAudio();
                playParticleSystem();
                AddInsertedBullets(-1);
                spawnBullet();
                
                _animator.SetInteger(_fire, 0);
            }
            
            _is_shooting = false;
            
            void spawnBullet()
            {
                GameObject bullet = BulletsPool.instance.getBullet(_spawnPointTransform.position, _spawnPointTransform.rotation);
                
                bullet.GetComponent<Rigidbody>().velocity = _spawnPointTransform.forward * _bulletSpeed;
            }

            void playAudio()
            {
                _shootAudio.clip = _audioShoot;
                _shootAudio.pitch = Random.Range(0.9f, 1.1f);
                _shootAudio.Play();
            }

            void playParticleSystem()
            {
                _shootParticleSystem.Play();
            }
        }
    }

    public void pickUpBullets(int count, out bool relevant)
    {
        relevant = false;
        
        int freeSpaceForBullets = MAX_BULLETS_COUNT_IN_STOCK - _inStockBullets;

        if (freeSpaceForBullets == 0)
            return;
        
        count = freeSpaceForBullets > count ? count : freeSpaceForBullets;
        AddInStockBullets(count);

        relevant = true;
    }
    #endregion
    
    #region Private Methods
    private void AddInsertedBullets(int count)
    {
        _insertedBullets += count;
        onInsertedBulletsCountChanged?.Invoke(_insertedBullets);
    }
    
    private void AddInStockBullets(int count)
    {
        _inStockBullets += count;
        onInStockBulletsCountChanged?.Invoke(_inStockBullets);
    }

    private bool IsAnimationPlaying(string name) => ClipInfos().Any(x => x.clip.name == name);
    
    private bool IsInTransition() => _animator.IsInTransition(0);

    private AnimatorClipInfo[] ClipInfos() => _animator.GetCurrentAnimatorClipInfo(0);
    #endregion
}
