using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;

    public Vector3 Direction { get; set; }
    public float Damage { get; set; }
    
    private void Update()
    {
        transform.Translate(Direction * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<ITakeDamage>() != null)
        {
            other.GetComponent<ITakeDamage>().TakeDamage(1f);
        }
        
        Destroy(gameObject);
    }
}
