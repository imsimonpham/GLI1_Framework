using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _hitBoxText;
    [SerializeField] TextMeshProUGUI _shotsFiredText;

    public void UpdateScore(int amount)
    {
        _scoreText.text = "Score: " + amount;
    }

    public void UpdateHitBoxText(string text)
    {
        _hitBoxText.text = "Hitbox: " + text;
    }

    public void UpdateShoftsFiredText(int amount)
    {
        _shotsFiredText.text = "Shots Fired: " + amount;
    }
}
