using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] float _destroyTime;
    [SerializeField] Vector3 _offset;
    Vector3 _ramdomizeIntensity = new Vector3(0.5f, 0, 0);

    private void Start()
    {
       Destroy(gameObject, _destroyTime);
       transform.position += _offset;
       Vector3 randomPos = new Vector3(Random.Range(-_ramdomizeIntensity.x, _ramdomizeIntensity.x), Random.Range(-_ramdomizeIntensity.x, _ramdomizeIntensity.x), 0);
       transform.position += randomPos;
    }
}

