using UnityEngine;

public class EnemyController : Character
{
    [SerializeField] int scoreValue;
    

    private void Awake()
    {
        scoreValue = 100;
            
    }
}
