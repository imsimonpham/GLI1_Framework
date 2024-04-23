using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    private int _explosionDamage = 100;
    private float _explostionRadius = 5f;
    private int _AILayerMaskIndex = 6;
    private int _overtimeBurnDamage = 10;
    private float _burnDuration = 3f;
    private float _burnTickInterval = 1f;
    [SerializeField] private List<AI> _burningEnemies = new List<AI>();

    private void Start()
    {
        Destroy(gameObject, 4.5f);
        _burningEnemies.Clear();
    }

    public void ExplostionDamage()
    {
        RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, _explostionRadius, transform.forward, 0f, 1 << _AILayerMaskIndex);

        for (var i = 0; i < hitInfo.Length; i++)
        {
            Hitbox hitbox = hitInfo[i].collider.GetComponent<Hitbox>();
            AI enemy = hitbox.GetAIComponent();
            if(enemy != null)
            {
                if (!enemy.IsBurning())
                {
                    enemy.SetIsBurning(true);
                    _burningEnemies.Add(enemy);
                }
             }else
             {
                 Debug.LogError("enemy is null");
             }

        }
        foreach (AI enemy in _burningEnemies)
        {
             enemy.TakeDamage(_explosionDamage);
             if (enemy.IsBurning())
             {
                 StartCoroutine(BurnRoutine(enemy));
                 enemy.SetIsBurning(false);
             } 
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the sphere cast in the Unity Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * _explostionRadius, _explostionRadius);
    }

    IEnumerator BurnRoutine(AI enemy)
    {
        float time = 0f;
        while (time < _burnDuration && enemy.IsBurning())
        {
            yield return new WaitForSeconds(_burnTickInterval);
            enemy.TakeDamage(_overtimeBurnDamage);
            time += Time.deltaTime;
        }
    }
}
