using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Рух та параметри гравця
    public float speed = 3.0f;
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    private int currentHealth;

    // Снаряди
    public GameObject projectilePrefab;

    // Компоненти та фізика
    private Rigidbody2D rigidbody2d;
    private Animator animator;

    // Напрямок руху та погляду
    private Vector2 move;
    private Vector2 lookDirection = new Vector2(1, 0);

    // Події
    public event Action OnTalkedToNPC;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Отримання введення руху
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(moveInput.x, 0.0f) || !Mathf.Approximately(moveInput.y, 0.0f))
        {
            lookDirection.Set(moveInput.x, moveInput.y);
            lookDirection.Normalize();
        }

        move = moveInput;

        // Параметри анімації
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", moveInput.magnitude);

        // Постріл на клавішу C
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        // Взаємодія з NPC на клавішу X (Діагностичний блок)
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("1. Клавішу X натиснуто!");

            RaycastHit2D hit = Physics2D.Raycast(
                rigidbody2d.position + Vector2.up * 0.2f,
                lookDirection,
                2.0f,
                LayerMask.GetMask("NPC")
            );

            if (hit.collider != null)
            {
                Debug.Log("2. Промінь влучив у об'єкт: " + hit.collider.name);

                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    Debug.Log("3. Скрипт NonPlayerCharacter знайдено! Відкриваємо UI...");
                    if (UIHandler.instance != null)
                    {
                        UIHandler.instance.DisplayDialogue();
                        OnTalkedToNPC?.Invoke();
                    }
                    else
                    {
                        Debug.LogError("ПОМИЛКА: UIHandler.instance дорівнює null! Перевірте об'єкт UIDocument у Hierarchy.");
                    }
                }
                else
                {
                    Debug.LogWarning("УВАГА: Промінь влучив у об'єкт, але на ньому немає скрипта NonPlayerCharacter!");
                }
            }
            else
            {
                Debug.LogWarning("УВАГА: Промінь ні у кого не влучив! (Занадто далеко або не той Layer).");
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position = position + move * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Launch(lookDirection, 300);
        }
        animator.SetTrigger("Launch");
    }

    public void PlaySound(AudioClip clip)
    {
    }
}