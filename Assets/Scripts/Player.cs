using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private int _score;
    [SerializeField] Animator _weaponAnim;
    [SerializeField] GameObject _scopeOverlay;
    [SerializeField] GameObject _weaponCamera;
    [SerializeField] GameObject _scopedOutCursor;
    [SerializeField] Camera _mainCamera;
    [SerializeField] FPS_Controller _FPSController;
    private float _scopedInFOV = 10f;
    private float _scopedOutFOV = 55f;


    private void Update()
    {
        if (Mouse.current.leftButton.isPressed) {
            _weapon.Fire();
        }

        if (Mouse.current.rightButton.isPressed)
        {
            _weaponAnim.SetBool("isScopedIn", true);
            StartCoroutine(OnScopedInRoutine());
        }
        else
        {
            _weaponAnim.SetBool("isScopedIn", false);
            OnScopedOut();
        }

        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            _scopedOutCursor.SetActive(!_scopedOutCursor.activeSelf);
        }
    }

    public void GainScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }

    void OnScopedOut()
    {
        _scopeOverlay.SetActive(false);
        _weaponCamera.SetActive(true);
        if (_scopedOutCursor.activeSelf)
        {
            _scopedOutCursor.SetActive(true);
        }
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
}
