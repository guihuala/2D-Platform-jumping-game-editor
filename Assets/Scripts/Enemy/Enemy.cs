using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Patrol, Jumper, Shooter }
    public EnemyType Type;

    public void SetEnemyType(EnemyType type)
    {
        Type = type;
    }

    void Update()
    {
        
    }
}
