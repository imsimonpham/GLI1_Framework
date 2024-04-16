using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    private Transform _startPoint;
    private Transform _endPoint;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _startPoint = GameObject.FindGameObjectWithTag("Start Point").transform;
        _endPoint = GameObject.FindGameObjectWithTag("End Point").transform;
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
        {
            _agent.destination = _endPoint.position;
        }else
        {
            Debug.LogError("Nav Mesh Agent is null");
        }
    }
}
