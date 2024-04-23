using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private Weapon _weapon;
    [SerializeField] Animator _weaponAnim;
    [SerializeField] GameObject _weaponCamera;

    [Header("Player's Stats")]
    [SerializeField] private int _score;
    [SerializeField] private int _enemiesKilled;
    [SerializeField] private int _lifePoint;

    [Header("UI")]
    [SerializeField] GameObject _scopeOverlay;
    [SerializeField] GameObject _scopedOutCursor;

    [Header("Controller")]
    [SerializeField] FPS_Controller _FPSController;

    [Header("Camera")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _scopedInFOV = 10f;
    [SerializeField] private float _scopedOutFOV = 55f;


    private bool _isCharging = false;
    private bool _isScopingIn = false;
    private bool _enabled = true;

    private void Start()
    {
        UIManager.Instance.UpdatePlayerLifePointText(_lifePoint);
    }

    private void Update()
    {
        if (!_enabled) { return; }
        //Fire a shot
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            _weapon.Fire();
            //if fired a scoped shot, disable weapon for 1 sec
            if (_isScopingIn)
            {
                _weapon.DiableWeapon();
                Invoke("WeaponChargeRecovery", 1f);
            }
        }

        //Scope in
        if (Mouse.current.rightButton.isPressed)
        {
            UIManager.Instance.ToggleWeaponChargeText(true);
            _weaponAnim.SetBool("isScopedIn", true);
            if (!_isScopingIn)
            {
                StartCoroutine(OnScopedInRoutine());
                _isScopingIn = true;
            }

            if(!_isCharging)
            {
                _weapon.StartCoroutine(_weapon.WeaponChargeRoutine());
                _isCharging = true;
            }
        }
        //Scope out
        else
        {
            UIManager.Instance.ToggleWeaponChargeText(false);
            _weaponAnim.SetBool("isScopedIn", false);
            OnScopedOut();
            _weapon.StopWeaponCharge();
            _isCharging = false;
        }

        /*if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            _scopedOutCursor.SetActive(!_scopedOutCursor.activeSelf);
        }*/
    }

    public void GainScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }

    public void IncreaseEnemiesKilled()
    {
        _enemiesKilled++;
        UIManager.Instance.UpdateEnemeiesKilled(_enemiesKilled);
    }

    void OnScopedOut()
    {
        _isScopingIn = false;
        _scopeOverlay.SetActive(false);
        _weaponCamera.SetActive(true);
        _scopedOutCursor.SetActive(true);
        _mainCamera.fieldOfView = _scopedOutFOV;
        _FPSController.AdjustLookSensitity(2f);
    } 

    IEnumerator OnScopedInRoutine()
    {
        yield return new WaitForSeconds(0.15f);
        _scopeOverlay.SetActive(true);
        _weaponCamera.SetActive(false);
        _scopedOutCursor.SetActive(false);
        _mainCamera.fieldOfView = _scopedInFOV;
        _FPSController.AdjustLookSensitity(0.5f);
    }

    void WeaponChargeRecovery()
    {
        _isCharging = false;
        _weapon.EnableWeapon();
    }

    public void ReduceLifePoint()
    {
        _lifePoint--;
        if (_lifePoint < 0)
        {
            _lifePoint = 0;
        }
        UIManager.Instance.UpdatePlayerLifePointText(_lifePoint);
    }

    public int GetLifePoint()
    {
        return _lifePoint;
    }

    public void TogglePlayerEnablement(bool enable)
    {
        if(enable)
        {
            _enabled = true;
        }else
        {
            _enabled = false;
        }
    }
}
