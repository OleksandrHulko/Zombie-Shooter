using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private NavMeshAgent _navMeshAgent = null;

    [SerializeField]
    private Animator _animator = null;
    
    [SerializeField]
    private AudioSource _audioSource = null;
    
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip _audioIdle = null;
    
    [SerializeField]
    private AudioClip _audioDamage = null;
    
    [SerializeField]
    private AudioClip _audioAttack = null;
    
    [SerializeField]
    private AudioClip _audioDead = null;
    
    [Space]
    
    [SerializeField]
    private List<Rigidbody> _allRigidbodies = null;
    #endregion

    #region Private Fields
    private Transform _destination = null;
    
    private static readonly int _attack  = Animator.StringToHash("Attack");
    private static readonly int _walking = Animator.StringToHash("Walking");

    private const int SYNCHRONIZED_SPEED = 2;
    private const int DETECTION_DISTANCE = 3;
    private const int MAX_HEALTH         = 200;
    private const int SECONDS_TO_DESPAWN = 3;
    
    private static int _playerLayer = 0;
    private static int _headLayer   = 0;
    private static int _bodyLayer   = 0;
    private static int _handLayer   = 0;
    private static int _legLayer    = 0;

    private int _health = MAX_HEALTH;
    
    private float _speed = 0.0f;

    private State _currentState = State.IDLE;

    private bool _is_old_state = false;
    #endregion

    #region Public Fields
    public static Action onDie = null;
    #endregion

    #region Private Methods
    private void Start()
    {
        _playerLayer = LayerMask.NameToLayer("Player"); // TODO initialize in other script
        _headLayer   = LayerMask.NameToLayer("Head");
        _bodyLayer   = LayerMask.NameToLayer("Body");
        _handLayer   = LayerMask.NameToLayer("Hand");
        _legLayer    = LayerMask.NameToLayer("Leg");

        _destination = Player.instance.transform;
        _speed = _navMeshAgent.speed;
    }

    private void Update()
    {
        if (isDead())
            return;
        
        SetDestination();
        SetState();
        ChekState();
    }

    private void SetDestination()
    {
        if (_navMeshAgent.enabled)
            _navMeshAgent.destination = _destination.position;
    }

    private void SetState()
    {
        State oldState = _currentState;
        bool is_move = GetCurrentSpeed() > 0;
        
        if (CanAttackPlayer())
            _currentState = State.ATTACK;
        else if (is_move)
        {
            _currentState = State.MOVE;
            SetAnimatorSpeed(GetCurrentSpeed() / SYNCHRONIZED_SPEED);
        }
        else
            _currentState = State.IDLE;

        _is_old_state = oldState == _currentState;
    }

    private void ChekState()
    {
        if (_is_old_state)
            return;
        
        switch (_currentState)
        {
            case State.IDLE:   IdleState();   break;
            case State.MOVE:   MoveState();   break;
            case State.ATTACK: AttackState(); break;
        }
    }

    private void IdleState()
    {
        if (!_audioSource.isPlaying)
            PlayAudioClip(_audioIdle, true);
        
        SetAnimatorSpeed();

        _animator.SetInteger(_walking, 0);
        _animator.SetInteger(_attack, 0);
    }

    private void MoveState()
    {
        PlayAudioClip(_audioIdle, true);
        
        _animator.SetInteger(_walking, 1);
        _animator.SetInteger(_attack, 0);
    }

    private void AttackState()
    {
        PlayAudioClip(_audioAttack);
        
        SetAnimatorSpeed();
        
        _animator.SetInteger(_attack, 1);
        _animator.SetInteger(_walking, 0);
    }

    private void SetAnimatorSpeed(float speed = 1.0f)
    {
        _animator.speed = speed;
    }

    private void MakePhysical()
    {
        _navMeshAgent.enabled = false;
        _animator.enabled = false;

        foreach (Rigidbody rigidbody in _allRigidbodies)
            rigidbody.isKinematic = false;
    }

    private void PlayAudioClip(AudioClip audioClip, bool is_loop = false)
    {
        _audioSource.clip = audioClip;
        _audioSource.loop = is_loop;
        _audioSource.Play();
    }

    private IEnumerator Die()
    {
        onDie?.Invoke();
        
        MakePhysical();
        PlayAudioClip(_audioDead);

        yield return new WaitForSeconds(SECONDS_TO_DESPAWN);

        SpawnBonus();
        
        Destroy(gameObject);

        void SpawnBonus()
        {
            Instantiate(ScriptableObjectsManager.instance._prefabContainer.bonuses, transform.position, Quaternion.identity);
        }
    }

    private float GetCurrentSpeed() => _navMeshAgent.velocity.magnitude;

    private bool CanAttackPlayer()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * DETECTION_DISTANCE, Color.blue);
        return Physics.Raycast(ray, DETECTION_DISTANCE, 1 << _playerLayer);

    }

    private bool isDead() => _health == 0;
    
    private void AddHealth( int health )
    {
        if (isDead())
            return;
        
        if (health < 0)
            PlayAudioClip(_audioDamage);
        
        _health += health;
        _health = Mathf.Clamp(_health, 0, MAX_HEALTH);

        if (isDead())
            StartCoroutine(Die());
    }
    #endregion

    #region Public Methods
    public void GetDamaged(int damage, int layer)
    {
        float coefficient = getDamageCoef();
        
        AddHealth((int) (-damage * coefficient));
        StartCoroutine(reactionOnDamage());

        float getDamageCoef()
        {
            if (layer == _headLayer)
                return 2.0f;
            if (layer == _bodyLayer)
                return 0.8f;
            if (layer == _handLayer)
                return 0.4f;
            if (layer == _legLayer)
                return 0.6f;

            return 1.0f;
        }

        IEnumerator reactionOnDamage()
        {
            if (_health > MAX_HEALTH / 4 )
            {
                _navMeshAgent.enabled = false;
                yield return new WaitForSeconds(coefficient);
                if (!isDead())
                    _navMeshAgent.enabled = true;
            }
            else
                _navMeshAgent.speed = _speed * 2.0f;
        }
    }
    #endregion

    #region enum
    private enum State : byte
    {
        IDLE = 0 ,
        MOVE ,
        ATTACK
    }
    #endregion
}
