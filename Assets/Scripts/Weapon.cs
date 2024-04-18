using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] LayerMask _layerMask;
    [SerializeField] int _damage;
    [SerializeField] private float _fireRate;
    private float _nextFireTime = -1f;
    private int _shotsFired;

    public void Fire()
    {
        if (Time.time > _nextFireTime)
        {
            _shotsFired++;
            UIManager.Instance.UpdateShoftsFiredText(_shotsFired);

            Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hitInfo;
            if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, _layerMask))
            {
                Hitbox hitbox = hitInfo.collider.GetComponent<Hitbox>();
                if (hitbox != null)
                {
                    UIManager.Instance.UpdateHitBoxText(hitbox.GetDamageType().ToString());
                    Hitbox.CollisionType type = hitbox.GetDamageType();
                    switch (type)
                    {
                        case Hitbox.CollisionType.Head:
                            hitbox.Hit(_damage * 2);
                            break;
                        case Hitbox.CollisionType.Body:
                            hitbox.Hit(_damage);
                            break;
                    }
                }
                else
                {
                    Debug.Log("Hit box is null");
                }
            }
            _nextFireTime = Time.time + _fireRate;
        }
    }
}
