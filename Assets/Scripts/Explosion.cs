using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    private int _explosionDamage = 100;
    private float _explosionRadius = 5f;
    private int _AIHitboxLayerMaskIndex = 10;
    private int _overtimeBurnDamage = 10;
    private float _burnDuration = 3f;
    private float _burnTickInterval = 1f;
    private float _timer = 0f;
    [SerializeField] private List<AI> _burningEnemies = new List<AI>();
    [SerializeField] private List<int> _burningEnemiesIDList = new List<int>();

    private void Start()
    {
        Destroy(gameObject, 4.5f);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    public void ExplosionDamage()
    {
        RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, _explosionRadius, transform.forward, 0f, 1 << _AIHitboxLayerMaskIndex);

        for (var i = 0; i < hitInfo.Length; i++)
        {
            Hitbox hitbox = hitInfo[i].collider.GetComponent<Hitbox>();
            AI enemy = hitbox.GetAIComponent();
            if (enemy != null)
            {
                int ID = enemy.GetID();
                if (!_burningEnemiesIDList.Contains(ID))
                {
                    enemy.SetIsBurning(true);
                    _burningEnemiesIDList.Add(ID);
                    _burningEnemies.Add(enemy);
                }
            }
            else
                Debug.LogError("enemy is null");

        }

        foreach (AI enemy in _burningEnemies)
        {
            enemy.TakeDamage(_explosionDamage);
            StartCoroutine(BurnRoutine(enemy));
        }
    }

    IEnumerator BurnRoutine(AI enemy)
    {
        while (_timer < _burnDuration && enemy.IsBurning())
        {
            yield return new WaitForSeconds(_burnTickInterval);
            enemy.TakeDamage(_overtimeBurnDamage);
        }
        enemy.SetIsBurning(false);
    }
}
