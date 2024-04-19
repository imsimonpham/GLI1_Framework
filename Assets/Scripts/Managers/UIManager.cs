using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _enemiesKilledText;
    [SerializeField] TextMeshProUGUI _hitBoxText; // TO BE REPLACED/DELETED
    [SerializeField] TextMeshProUGUI _shotsFiredText; // TO BE REPLACED/DELETED
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _weaponChargeText;

    private float _countdownTime = 180;

    private void Update()
    {
        float t = _countdownTime - Time.time;

        if (t <= 0)
        {
            t = 0;
            // Optionally, do something when the countdown reaches zero.
        }

        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f0"); // rounded to the nearest second

        _timerText.text = minutes + ":" + seconds.PadLeft(2, '0');
    }

    public void UpdateScore(int amount)
    {
        _scoreText.text = "Score: " + amount;
    }

    public void UpdateEnemeiesKilled(int amount)
    {
        _enemiesKilledText.text = "Enemies Killed: " + amount;
    }

    public void UpdateHitBoxText(string text)
    {
        _hitBoxText.text = "Hitbox: " + text;
    }

    public void UpdateShoftsFiredText(int amount)
    {
        _shotsFiredText.text = "Shots Fired: " + amount;
    }

    void UpdateTimer(float amount)
    {
        _timerText.text = amount.ToString();
    }

    public void UpdateWeaponChargePercentageText(float amount)
    {
        _weaponChargeText.text = amount.ToString("f0") + "%";
    }

    public void ToggleWeaponChargeText(bool show)
    {
        if (show)
        {
            _weaponChargeText.gameObject.SetActive(true);
        }else
        {
            _weaponChargeText.gameObject.SetActive(false);
        }
    }
}
