using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Знищуємо снаряд, якщо він відлетів занадто далеко від центру карти
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    // Метод запуску снаряда (викликається зі скрипта PlayerController)
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    // Обробка влучання у ворога чи інші об'єкти
    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController controller = other.GetComponent<EnemyController>();

        if (controller != null)
        {
            controller.Fix();
        }

        Destroy(gameObject);
    }
}