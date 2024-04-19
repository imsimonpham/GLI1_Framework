using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    //Rotation
    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

    //Recoil
    [SerializeField] private float _recoilX;

    //Settings
    [SerializeField] private float _snappiness;
    [SerializeField] private float _returnSpeed;

    private void Update()
    {
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero,_returnSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, _snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(_currentRotation);
    }

    public void RecoilFire()
    {
        _targetRotation += new Vector3(_recoilX, 0, 0);
    }
}
