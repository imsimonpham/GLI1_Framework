using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] LayerMask _layerMask;
    [SerializeField] int _unscopedDamage = 35;
    [SerializeField] int _fullyScopedDamage = 100;
    [SerializeField] private float _fireRate;
    private float _nextFireTime;
    private float _fullChargePercentage = 100f;
    private float _chargeDuration = 1f;
    private float _startChargePercentage = 0f;
    private float _currentChargePercentage;
    private bool _isCharging;
    private bool _isCurrentlyScoping;
    private int _damageDealt;
    private bool _canCharge = true;
    private int _AIHitboxLayerMaskIndex = 10;
    private int _coverLayerMaskIndex = 11;
    private int _barrierLayerMaskIndex = 7;
    private int _barrelLayerMaskIndex = 9;
    private int _headShotMultiplier = 2;

    [Header("UI")]
    [SerializeField] private GameObject _headShotInd;
    [SerializeField] private GameObject _bodyShotInd;
    private bool _shownHeadShotInd = false;
    private bool _shownBodyShotInd = false;
    private float _timeBetweenHitIndications = 0.1f;

    [Header("VFX")]
    [SerializeField] private GameObject _muzzleFlashPrefab;
    [SerializeField] private GameObject _firePoint;
    [SerializeField] private GameObject _gargabeContainer;
    [SerializeField] private GameObject _scopedShotTrailPrefab;
    [SerializeField] private GameObject _explosionPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip _unscopedShotSound;
    [SerializeField] private AudioClip _scopedShotSound;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _bulletHitBarrierSound;

    //Recoil
    private CameraRecoil _camRecoil;

    private void Start()
    {
        _camRecoil = GameObject.FindAnyObjectByType<CameraRecoil>().GetComponent<CameraRecoil>();
        if(_camRecoil == null )
            Debug.Log("Camera Recoil is null");
    }

    void Update()
    {
        if (_isCurrentlyScoping)
            _fireRate = 1f;
        else
            _fireRate = 0.2f;

        UIManager.Instance.UpdateWeaponChargePercentageText(_currentChargePercentage);
    }

    public void Fire()
    {
        if (Time.time > _nextFireTime)
        {
            _nextFireTime = Time.time + _fireRate;
            Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hitInfo;


            if (!_isCurrentlyScoping)
            {
                ShowUnscopedShotMuzzleFlash();
                _camRecoil.RecoilFire();
                PlayUnscopedShotSound();
            } else
            {
                ShowScopedShotTrail();
                _camRecoil.RecoilFire();
                PlayScopedShotSound();
            }


            if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, 1 << _AIHitboxLayerMaskIndex | 1 << _barrierLayerMaskIndex | 1 << _barrelLayerMaskIndex | 1 << _coverLayerMaskIndex))
            {
                if(hitInfo.collider.gameObject.layer == _AIHitboxLayerMaskIndex)
                {
                    Hitbox hitbox = hitInfo.collider.GetComponent<Hitbox>();
                    if (hitbox != null)
                    {

                        int damage;
                        Hitbox.CollisionType type = hitbox.GetDamageType();
                        switch (type)
                        {
                            case Hitbox.CollisionType.Head:
                                if (!_shownHeadShotInd)
                                {
                                    StartCoroutine(HitIndicatingRoutine(_headShotInd));
                                    _shownHeadShotInd = true;
                                }
                                damage = CalculateHitDamage() * _headShotMultiplier;
                                hitbox.Hit(damage);
                                _shownHeadShotInd = false;
                                break;
                            case Hitbox.CollisionType.Body:
                                if (!_shownBodyShotInd)
                                {
                                    StartCoroutine(HitIndicatingRoutine(_bodyShotInd));
                                    _shownBodyShotInd = false;
                                }
                                damage = CalculateHitDamage();
                                hitbox.Hit(damage);
                                _shownBodyShotInd = false;
                                break;
                        }

                        AI enemy = hitbox.GetAIComponent();
                        if (enemy == null)
                        {
                            Debug.LogError("AI Component is null");
                            return;
                        }
                        if (!_isCurrentlyScoping)
                            enemy.SetKillType(AI.KillType.Unscope);
                        else
                        {
                            enemy.SetKillType(AI.KillType.Scope);
                            enemy.AdjustDeathSound(_currentChargePercentage);
                        }
                    }
                    else
                        Debug.LogError("Hit box is null");
                } 
                else if (hitInfo.collider.gameObject.layer == _barrierLayerMaskIndex)
                {
                    //if layer mask == Barrier
                    _audioSource.PlayOneShot(_bulletHitBarrierSound, 1f);
                    Barrier barrier = hitInfo.collider.GetComponent<Barrier>();
                    if (barrier != null)
                    {
                        barrier.TakeDamage(CalculateHitDamage());
                    }
                }
                else if (hitInfo.collider.gameObject.layer == _barrelLayerMaskIndex)
                {
                    Explosion explostion = Instantiate(_explosionPrefab, hitInfo.collider.transform.position, Quaternion.identity).GetComponent<Explosion>();
                    if (explostion != null)
                        explostion.ExplosionDamage();
                    else
                        Debug.Log("explosion is null");

                    Destroy(hitInfo.collider.gameObject, 0.5f);
                } 
                else if (hitInfo.collider.gameObject.layer == _coverLayerMaskIndex)
                {
                    return;
                }
            }
            

        }
    }

    IEnumerator HitIndicatingRoutine(GameObject indicator)
    {
        indicator.SetActive(true);
        yield return new WaitForSeconds(_timeBetweenHitIndications);
        indicator.SetActive(false);
    }

    public IEnumerator WeaponChargeRoutine()
    {
        _isCharging = true; // Set charging flag to true
        float timer = 0f;

        while (timer < _chargeDuration && _isCharging) // Check if still charging
        {
            _isCurrentlyScoping = true;
            //Address weapon recoverry
            if (!_canCharge)
            {
                _currentChargePercentage = 0;
                break;
            } else
            {
                float progress = timer / _chargeDuration;
                _currentChargePercentage = Mathf.Lerp(_startChargePercentage, _fullChargePercentage, progress); // Start from 0%
                if (_currentChargePercentage >= 98f) // Check if charge is close to 100%
                    _currentChargePercentage = _fullChargePercentage; // Round up to 100% if close

                timer += Time.deltaTime;
            }         
            yield return null;
        }
    }

    public void StopWeaponCharge()
    {
        //stop weapon from charging and reset weapon for the next charge 
        _isCurrentlyScoping = false;
        _isCharging = false;
        _currentChargePercentage = 0;
        _canCharge = true;
    }

    int CalculateHitDamage()
    {
        if (_isCurrentlyScoping)
            _damageDealt = Mathf.RoundToInt(_fullyScopedDamage * _currentChargePercentage / 100f);
        else
            _damageDealt = _unscopedDamage;

        return _damageDealt;
    }

    public void DiableWeapon()
    {
        _currentChargePercentage = 0;
        _canCharge = false;     
    }

    public void EnableWeapon()
    {
        _canCharge = true;
    }

    void ShowUnscopedShotMuzzleFlash()
    {
       GameObject muzzleFlash = Instantiate(_muzzleFlashPrefab, _firePoint.transform.position, _firePoint.transform.rotation, _gargabeContainer.transform);
       Destroy(muzzleFlash, 0.1f );
    }

    void ShowScopedShotTrail()
    {
        Vector3 shotDirection = _firePoint.transform.forward; // Get the direction the weapon is facing

        // Instantiate the trail effect
        GameObject trailInstance = Instantiate(_scopedShotTrailPrefab, _firePoint.transform.position, _firePoint.transform.rotation, _gargabeContainer.transform);
        TrailEffect trailEffect = trailInstance.GetComponent<TrailEffect>();

        if (trailEffect != null)
        {
            // Set the direction and speed of the trail effect
            trailEffect.SetTrailDirection(shotDirection);
            trailEffect.SetTrailSpeed(10f); // Adjust speed as needed
        }
    }

    void PlayScopedShotSound()
    {
        if(_currentChargePercentage <= 50f)
        {
            _audioSource.PlayOneShot(_scopedShotSound, 0.2f);
            _audioSource.pitch = 2f;
        } else if(_currentChargePercentage <= 80f)
        {
            _audioSource.PlayOneShot(_scopedShotSound, 0.3f);
            _audioSource.pitch = 1.5f;
        } else
        {
            _audioSource.PlayOneShot(_scopedShotSound, 0.4f);
            _audioSource.pitch = 1f;
        }
    }

    void PlayUnscopedShotSound()
    {
        _audioSource.PlayOneShot(_unscopedShotSound, 0.2f);
        _audioSource.pitch = 2f;
    }
}
