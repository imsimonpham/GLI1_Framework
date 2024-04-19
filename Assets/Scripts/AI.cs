using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    private List<GameObject> _wayPoints = new List<GameObject>();
    private int _currentWaypointIndex;
    private GameObject _endPoint;
    private GameObject[] _totalCovers;
    private List<int> _coverIndexes = new List<int>();
    private List<List<int>> _coverMasterList = new List<List<int>>();

    [Header("Animation")]
    [SerializeField] private Animator _anim;

    [Header("AI ")]
    [SerializeField] private int _health;
    private enum AIState
    {
        Run,
        Hide,
        Die
    }
    [SerializeField] private AIState _currentState;
    
    private NavMeshAgent _agent;
    private bool _isHiding = false;
    private bool _isDead = false;
    
    private int _score = 50;
    private int _fullHealth = 200;

    //Player
    private Player _player;

    [Header("UI")]
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private GameObject _healthBarContainer;
    [SerializeField] private Image _healthBar;
    private float _timeBetweenKillIndications = 0.5f;
    private GameObject _killInd;
    private Camera _mainCam;

    private void Awake()
    { 
        _endPoint = GameObject.FindGameObjectWithTag("End Point");
        _coverMasterList.Add(new List<int> { 0, 6, 11 }); 
        _coverMasterList.Add(new List<int> { 1, 7, 13 }); 
        _coverMasterList.Add(new List<int> { 2, 9, 15 }); 
        _coverMasterList.Add(new List<int> { 3, 8, 12 }); 
        _coverMasterList.Add(new List<int> { 4, 10, 14 });
        _coverMasterList.Add(new List<int> { 3, 5, 13 });
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("Nav Mesh Agent is null");
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if( _player == null)
        {
            Debug.LogError("Player is null");
        }
        _killInd = GameObject.FindGameObjectWithTag("Kill Indicator");
        if (_killInd == null)
        {
            Debug.LogError("Kill Indicator is null");
        }

        _mainCam = Camera.main;
    }


    private void Update()
    {
        if(_health <= 0)
        {
            _currentState = AIState.Die;
        }

        switch (_currentState)
        {
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
        if(transform.name == "Test") { return; }
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
    }

    void Resume()
    {  
        _agent.isStopped = false;
        _anim.SetBool("isHiding", false);
        _currentState = AIState.Run;
        _isHiding = false;
    }

    void Die()
    {
        _agent.isStopped = true;
        _anim.SetBool("isDead", true);
        _coverIndexes.Clear();
        _wayPoints.Clear();
        _player.GainScore(_score);
        _player.IncreaseEnemiesKilled();
        _isDead = true;
        Invoke("ResetAI", 2.5f);
    }

    public void SetupAI()
    {
        GenerateWaypointList();
        _currentWaypointIndex = 0;      
    }

    void ResetAI()
    {
        _isDead = false;
        _health = _fullHealth;
        UpdateHealthBar();
        _currentState = AIState.Run;
        gameObject.SetActive(false);
    }

    public void TakeDamage(int dmgAmount)
    {
        _health -= dmgAmount;
        UpdateHealthBar();
        ShowDamageText(dmgAmount);
        if(_health <= 0 )
        {
            _currentState = AIState.Die;
            StartCoroutine(KillIndicatingRoutine());
        }
    }

    void ShowDamageText(int dmgAmount)
    {
        TextMesh text = Instantiate(_damageTextPrefab, transform.position, Quaternion.Euler(0, -180, 0), transform).GetComponent<TextMesh>();
        if(text != null )
        {
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
        _healthBar.fillAmount = (float) _health / _fullHealth;
    }
}
