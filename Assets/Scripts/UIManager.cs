using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] TextMeshProUGUI _scoreText;

    public void UpdateScore(int amount)
    {
        _scoreText.text = amount.ToString();
    }
}
