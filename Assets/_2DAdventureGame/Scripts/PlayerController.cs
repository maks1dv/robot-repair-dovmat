using System;
using System.Collections;
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

    // Снаряди та аудіо
    public GameObject projectilePrefab;
    public AudioClip hitSound;
    public AudioClip throwSound;
    public AudioClip healSound;

    // Компоненти та фізика
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    // Напрямок руху та погляду
    private Vector2 move;
    private Vector2 lookDirection = new Vector2(1, 0);

    // Події
    public event Action OnTalkedToNPC;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }
        currentHealth = maxHealth;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(moveInput.x, 0.0f) || !Mathf.Approximately(moveInput.y, 0.0f))
        {
            lookDirection.Set(moveInput.x, moveInput.y);
            lookDirection.Normalize();
        }

        move = moveInput;

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", moveInput.magnitude);

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
            PlaySound(hitSound);
            StartCoroutine(FlashColor(Color.red));
        }
        else if (amount > 0)
        {
            PlaySound(healSound);
            StartCoroutine(FlashColor(Color.green));
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (Beginner2D.UIHandler.instance != null)
        {
            Beginner2D.UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        }
    }

    IEnumerator FlashColor(Color flashColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = defaultColor;
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
        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}