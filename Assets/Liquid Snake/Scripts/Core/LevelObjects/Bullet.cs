using LiquidSnake.Character;
using UnityEngine;

/// <summary>
/// Component used to destroy itself, in this case for the bullets.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage;

    public GameObject BulletOwner;

    // Use this for initialization
    /// <summary>
    /// Initialize the component to be destroy the GameObject in 2 seconds.
    /// </summary>
    void Start()
    {
        Destroy(gameObject, 2f);
    } // Start

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogFormat("Bullet hit {0}", collision.gameObject.name);
        if (BulletOwner == null)
        {
            Debug.LogWarningFormat("This bullet doesn't have an owner assigned {0}.", gameObject.name);
            if (collision.collider.TryGetComponent<Health>(out var health))
            {
                health.Damage(damage);
            }
        }
        else
        {

            if (collision.collider.TryGetComponent<Health>(out var health) && !collision.gameObject.GetInstanceID().Equals(BulletOwner.GetInstanceID()))
            {
                health.Damage(damage);
            }

        }
        Destroy(gameObject);
    }
}