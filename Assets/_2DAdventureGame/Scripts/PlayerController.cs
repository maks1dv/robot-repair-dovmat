using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Рух та параметри гравця
    public float speed = 3.0f;
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    private int currentHealth;

    // Невразливість після удару
    public float timeInvincible = 2.0f;
    private bool isInvincible;
    private float damageCooldown;

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

        // Відлік таймера невразливості
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        // Постріл на клавішу C
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        // Взаємодія з NPC на клавішу X
        if (Input.GetKeyDown(KeyCode.X))
        {
            int characterMask = LayerMask.GetMask("Character");

            // 1. Пошук променем у напрямку погляду
            RaycastHit2D hit = Physics2D.Raycast(
                rigidbody2d.position + Vector2.up * 0.2f,
                lookDirection,
                2.5f,
                characterMask
            );

            NonPlayerCharacter character = null;

            if (hit.collider != null)
            {
                character = hit.collider.GetComponent<NonPlayerCharacter>()
                         ?? hit.collider.GetComponentInParent<NonPlayerCharacter>();
            }

            // 2. Запасний пошук у радіусі 2.0f навколо гравця
            if (character == null)
            {
                Collider2D[] nearby = Physics2D.OverlapCircleAll(rigidbody2d.position, 2.0f, characterMask);
                foreach (var col in nearby)
                {
                    character = col.GetComponent<NonPlayerCharacter>()
                             ?? col.GetComponentInParent<NonPlayerCharacter>();
                    if (character != null) break;
                }
            }

            if (character != null)
            {
                if (Beginner2D.UIHandler.instance != null)
                {
                    Beginner2D.UIHandler.instance.DisplayDialogue();
                    OnTalkedToNPC?.Invoke();
                }
                else
                {
                    Debug.LogError("ПОМИЛКА: Beginner2D.UIHandler.instance дорівнює null!");
                }
            }
            else
            {
                Debug.LogWarning("УВАГА: NPC поруч не знайдено!");
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
            if (isInvincible)
                return;

            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (Beginner2D.UIHandler.instance != null)
        {
            Beginner2D.UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        }
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