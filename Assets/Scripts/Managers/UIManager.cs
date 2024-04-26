using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _enemiesKilledText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _weaponChargeText;
    [SerializeField] TextMeshProUGUI _waveIndexText;
    [SerializeField] TextMeshProUGUI _nextWaveText;
    [SerializeField] TextMeshProUGUI _playerLifePointText;
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _WinLossMenu;
    [SerializeField] TextMeshProUGUI _winText;
    [SerializeField] TextMeshProUGUI _lossText;

    private void Update()
    {
        float t = Time.timeSinceLevelLoad;

        if (t <= 0)
            t = 0;

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

    public void UpdateWeaponChargePercentageText(float amount)
    {
        _weaponChargeText.text = amount.ToString("f0") + "%";
    }

    public void ToggleWeaponChargeText(bool show)
    {
        if (show)
            _weaponChargeText.gameObject.SetActive(true);
        else
            _weaponChargeText.gameObject.SetActive(false);
    }

    public void UpdateWaveText(int waveIndex, int totalWaves)
    {
        _waveIndexText.text = waveIndex + " / " + totalWaves;
    }

    public void ShowNextWaveText(bool show)
    {
        if (show)
            _nextWaveText.gameObject.SetActive(true);
        else
            _nextWaveText.gameObject.SetActive(value: false);
    }

    public void UpdatePlayerLifePointText(int lifepoint)
    {
        _playerLifePointText.text = lifepoint.ToString();
    }

    public void TogglePauseMenu(bool show)
    {
        if (show)
            _pauseMenu.gameObject.SetActive(true);
        else
            _pauseMenu.gameObject.SetActive(false);
    }

    public void ToggleWinLossMenu(string conditionText)
    {
        _WinLossMenu.SetActive(true);
        if (conditionText == "win")
        {
            _winText.gameObject.SetActive(true);
            _lossText.gameObject.SetActive(false);
        }
        else if (conditionText == "loss")
        {
            _winText.gameObject.SetActive(false);
            _lossText.gameObject.SetActive(true);
        }
    }
}
