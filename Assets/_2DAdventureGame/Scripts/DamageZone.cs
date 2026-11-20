using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 1.5f;

    private float nextDamageTime = 0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.ChangeHealth(-damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null && Time.time >= nextDamageTime)
        {
            controller.ChangeHealth(-damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }
}