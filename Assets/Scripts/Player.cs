using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _score;

    public void GainScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }
}
