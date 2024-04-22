using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum CollisionType
    {
        Head, 
        Body
    }

    [SerializeField] private CollisionType _damageType;
    [SerializeField] private AI _AI;

    public void Hit(int dmgAmount)
    {
        _AI.TakeDamage(dmgAmount);   
    }
    
    public CollisionType GetDamageType()
    {
        return _damageType;
    }

    public AI GetAIComponent()
    {
        return _AI;
    }
}
