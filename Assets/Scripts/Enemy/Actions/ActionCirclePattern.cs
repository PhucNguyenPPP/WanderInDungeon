using UnityEngine;

public class ActionCirclePattern : FSMAction
{
    [SerializeField] private float projectileAmount;
    [SerializeField] private float timeBtwAttacks;

    private EnemyPattern enemyPattern;
    private float timer;

    private void Awake()
    {
        enemyPattern = GetComponent<EnemyPattern>();
    }

    private void Start()
    {
        timer = timeBtwAttacks;
    }

    public override void Act()
    {
        throw new System.NotImplementedException();
    }

    private void Attack()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            float angle = 360f / projectileAmount;
            for (int i = 0; i < projectileAmount; i++)
            {
                float projectAngle = angle * i;
                Projectile projectile = enemyPattern.GetProjectile();
                projectile.transform.rotation =
                    Quaternion.Euler(new Vector3(0f, 0f, projectAngle));
            }

            timer = timeBtwAttacks;
        }
    }
}
