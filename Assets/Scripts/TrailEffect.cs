using UnityEngine;
using System.Collections;

public class TrailEffect : MonoBehaviour
{
    [SerializeField] private float _trailSpeed; // Speed of the trail effect
    [SerializeField] private float _trailDuration; // Duration of the trail effect in seconds
    [SerializeField] private float _trailAccelerator;
    [SerializeField] private float yOffset;

    private LineRenderer _lineRenderer;
    private Vector3 _startPoint;
    private Vector3 _endPoint;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _startPoint = transform.position;
        Vector3 shootDir = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f + yOffset, 0)).direction;
        _endPoint = _startPoint + shootDir * _trailSpeed * _trailDuration;
        StartCoroutine(UpdateTrail());
    }


    private IEnumerator UpdateTrail()
    {
        float timer = 0f;
        while (timer < _trailDuration)
        {
            timer += Time.deltaTime;
            UpdateTrailPosition(timer / _trailAccelerator);
            yield return null;
        }

        Destroy(gameObject); // Destroy the trail effect GameObject after its duration
    }

    private void UpdateTrailPosition(float progress)
    {
        Vector3 currentPoint = Vector3.Lerp(_startPoint, _endPoint, progress);
        _lineRenderer.SetPosition(0, _startPoint);
        _lineRenderer.SetPosition(1, currentPoint);
    }

    public void SetTrailDirection(Vector3 direction)
    {
        _endPoint = _startPoint + direction * _trailSpeed * _trailDuration;
    }

    public void SetTrailSpeed(float speed)
    {
        _trailSpeed = speed;
    }
}
