using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    //AI States
    private List<GameObject> _wayPoints = new List<GameObject>();
    private int _currentWaypointIndex;
    private GameObject _endPoint;
    private GameObject[] _totalCovers;
    private List<int> _coverIndexes = new List<int>();
    private List<List<int>> _coverMasterList = new List<List<int>>();
    float _distanceToEndPoint;
    private bool _executedFinishedAITasks;

    [Header("Animation")]
    [SerializeField] private Animator _anim;
    [SerializeField] private int _floor;

    [Header("AI ")]
    [SerializeField] private int _health;
    private enum AIState
    {
        Idle,
        Run,
        Hide,
        Die
    }
    [SerializeField] private AIState _currentState;
    [SerializeField] private int _enemyWaveIndex;
    [SerializeField] private bool _isBurning;

    private NavMeshAgent _agent;
    private bool _isHiding = false;
    private bool _isDead = false;

    private int _score = 50;
    private int _fullHealth = 200;

    //Player
    private Player _player;

    [Header("Audio")]
    [SerializeField] private AudioClip _unscopeKill;
    [SerializeField] private AudioClip _scopeKill;
    [SerializeField] private AudioClip _trackCompletion;
    [SerializeField] private AudioSource _audioSource;

    public enum KillType
    {
        Unscope,
        Scope
    }
    [SerializeField] private KillType _killType;

    [Header("UI")]
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private GameObject _healthBarContainer;
    [SerializeField] private Image _healthBar;
    private float _timeBetweenKillIndications = 0.3f;
    private GameObject _killInd;
    private Camera _mainCam;

    private void Awake()
    {
        _endPoint = GameObject.FindGameObjectWithTag("End Point");
        _coverMasterList.Add(new List<int> { 1, 9, 14 });
        _coverMasterList.Add(new List<int> { 5, 10, 15 });
        _coverMasterList.Add(new List<int> { 2, 11, 14 });
        _coverMasterList.Add(new List<int> { 4, 7, 12 });
        _coverMasterList.Add(new List<int> { 0, 8, 11 });
        _coverMasterList.Add(new List<int> { 3, 6, 13 });
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("Nav Mesh Agent is null");
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null");
        }
        _killInd = GameObject.FindGameObjectWithTag("Kill Indicator");
        if (_killInd == null)
        {
            Debug.LogError("Kill Indicator is null");
        }

        _mainCam = Camera.main;
        _currentState = AIState.Run;
    }


    private void Update()
    {
        if (_health <= 0)
        {
            _currentState = AIState.Die;
        }

        _distanceToEndPoint = Vector3.Distance(_endPoint.transform.position, transform.position);

        if (_distanceToEndPoint < 1f)
        {
            if (!_executedFinishedAITasks)
            {
                PlayFinishedAISound();
                SpawnManager.Instance.UpdateEnemiesAlive();
                _player.ReduceLifePoint();
                Invoke(methodName: "ReuseFinishedAI", 1f);
                _executedFinishedAITasks = true;
            }
        }

        switch (_currentState)
        {
            case AIState.Idle:
                break;
            case AIState.Run:
                MoveForward();
                break;
            case AIState.Hide:
                if (!_isHiding)
                {
                    StartCoroutine(HideRoutine());
                    _isHiding = true;
                }
                break;
            case AIState.Die:
                if (!_isDead)
                {
                    Die();
                }
                break;
        }

        //ensure health bar always looks at camera
        _healthBarContainer.transform.rotation = Quaternion.LookRotation(_healthBarContainer.transform.position - _mainCam.transform.position);    
    }

    void MoveForward()
    {
        if (transform.name.Contains("Test")) { return; }
        _agent.SetDestination(_wayPoints[_currentWaypointIndex].transform.position);
        float distanceToCurrentWaypoint = Vector3.Distance(_wayPoints[_currentWaypointIndex].transform.position, _agent.transform.position);
        if (distanceToCurrentWaypoint < 1f && _currentWaypointIndex < _wayPoints.Count - 1)
        {
            _currentState = AIState.Hide;
            _currentWaypointIndex++;
        }
    }

    void GenerateWaypointList()
    {
        //get all the covers
        _totalCovers = GameObject.FindGameObjectsWithTag("Cover");
        //select a random cover list
        _coverIndexes = Utilities.GetRandomCoverList(_coverMasterList);
        //add covers to waypoint list if their indexes were selected
        for (var i = 0; i < _totalCovers.Length; i++)
        {
            if (_coverIndexes.Contains(i))
            {
                _wayPoints.Add(_totalCovers[i]);
            }
        }
        _wayPoints.Add(_endPoint);
    }

    IEnumerator HideRoutine()
    {
        Stop();
        yield return new WaitForSeconds(3f);
        Resume();
    }

    void Stop()
    {
        _agent.isStopped = true;
        _anim.SetBool("isHiding", true);
        AdjustAgentRadius(0);
        AdjustRotation();
    }

    void Resume()
    {
        _agent.isStopped = false;
        _anim.SetBool("isHiding", false);
        _currentState = AIState.Run;
        AdjustAgentRadius(0.5f);
        _isHiding = false;
    }

    void Die()
    {
        SpawnManager.Instance.UpdateEnemyKilledCount(this);
        SpawnManager.Instance.UpdateEnemiesAlive();
        _agent.isStopped = true;
        _anim.SetBool("isDead", true);
        _coverIndexes.Clear();
        _wayPoints.Clear();
        _player.GainScore(_score);
        _player.IncreaseEnemiesKilled();
        _isDead = true;
        AdjustAgentRadius(0);
        PlayDeathSound();
        Invoke(methodName: "ReuseDeadAI", 2.5f);
    }

    public void SetupAI()
    {
        GenerateWaypointList();
        _currentWaypointIndex = 0;
        _currentState = AIState.Run;
    }

    void ReuseDeadAI()
    {
        _isDead = false;
        _health = _fullHealth;
        UpdateHealthBar();
        _currentState = AIState.Idle;
        AdjustAgentRadius(0.5f);
        gameObject.SetActive(false);
    }

    void ReuseFinishedAI()
    {
        _health = _fullHealth;
        UpdateHealthBar();
        _coverIndexes.Clear();
        _wayPoints.Clear();
        _currentState = AIState.Idle;
        gameObject.SetActive(false);
    }

    public void TakeDamage(int dmgAmount)
    {
        _health -= dmgAmount;
        UpdateHealthBar();
        ShowDamageText(dmgAmount);
        if (_health <= 0)
        {
            _currentState = AIState.Die;
            StartCoroutine(KillIndicatingRoutine());
        }
    }

    void ShowDamageText(int dmgAmount)
    {
        TextMesh text = Instantiate(_damageTextPrefab, transform.position, Quaternion.Euler(0, -180, 0), transform).GetComponent<TextMesh>();
        if (text != null)
        {
            text.GetComponent<DamageText>().SetParent(transform);
            text.text = dmgAmount.ToString();
        }
    }

    IEnumerator KillIndicatingRoutine()
    {
        _killInd.GetComponent<Image>().enabled = true;
        yield return new WaitForSeconds(_timeBetweenKillIndications);
        _killInd.GetComponent<Image>().enabled = false;
    }
    void UpdateHealthBar()
    {
        _healthBar.fillAmount = (float)_health / _fullHealth;
    }

    void AdjustRotation()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, -180f, transform.rotation.z);
    }

    public void SetKillType(KillType killType)
    {
        _killType = killType;
    }

    void PlayDeathSound()
    {
        if(_killType == KillType.Scope)
        {
            _audioSource.PlayOneShot(_scopeKill);
        } else
        {
            _audioSource.PlayOneShot(_unscopeKill, 1f);
            _audioSource.pitch = 1f;
        }
    }
    
    public void AdjustDeathSound(float currentChargePercentage)
    {
        if(currentChargePercentage < 50f)
        {
            _audioSource.volume = 0.3f;
        }else if (currentChargePercentage < 80f)
        {
            _audioSource.volume = 0.5f;
        }
        else
        {
            _audioSource.volume = 1f;
        }
    }

    private void AdjustAgentRadius(float radius)
    {
        _agent.radius = radius;
    }

    void PlayFinishedAISound()
    {
        _audioSource.PlayOneShot(_trackCompletion, 1f);
    }

    public void SetEnemyWaveIndex(int index)
    {
        _enemyWaveIndex = index;
    }

    public int GetEnemyWaveIndex()
    {
        return _enemyWaveIndex;
    }

    public bool IsBurning()
    {
        return _isBurning;
    }

    public void SetIsBurning(bool burn)
    {
        if (burn)
        {
            _isBurning = true;
        }else
        {
            _isBurning = false;
        }
    }

    public int GetHealth()
    {
        return _health;
    }
}