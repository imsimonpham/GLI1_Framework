using System.Collections;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Barrier")]
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] BoxCollider _collider;
    [SerializeField] int _health;
    private bool _isInRecovery = false;

    [Header(header: "Breakables")]
    [SerializeField] private GameObject _shatteredBarrierPrefab;
    [SerializeField] private float _upwardModifier = 1f;
    [SerializeField] private float _explosionRadius = 2;
    [SerializeField] private float _explosionForce = 500;


    private void Update()
    {
        if (_isInRecovery)
        {
            StartCoroutine(RecoverRoutine());
            _isInRecovery = false;
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            Shatter();
            _isInRecovery = true;
        }
    }

    public void Shatter()
    {
        GameObject shatteredBarrier = Instantiate(_shatteredBarrierPrefab, transform.position, Quaternion.identity);
        var surroundingObjects = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (var obj in surroundingObjects)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _upwardModifier);
        }
        _renderer.enabled = false;
        _collider.enabled = false;
        Destroy(shatteredBarrier, 2f);
    }

    IEnumerator RecoverRoutine()
    {      
        yield return new WaitForSeconds(5f);
        _renderer.enabled = true;
        _collider.enabled = true;
        _health = 50;
    }
}
