using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1.0f;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool broken = true;

    // Лічильник усіх ворогів на сцені
    private static int totalEnemies = 0;
    private static int fixedEnemies = 0;
    private static int lastSceneHandle = -1;

    void Awake()
    {
        // Скидаємо лічильники при кожному запуску або перезавантаженні сцени
        int currentScene = gameObject.scene.handle;
        if (lastSceneHandle != currentScene)
        {
            totalEnemies = 0;
            fixedEnemies = 0;
            lastSceneHandle = currentScene;
        }
        totalEnemies++;
    }

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

        if (Beginner2D.UIHandler.instance != null)
        {
            Beginner2D.UIHandler.instance.SetCounter(fixedEnemies, totalEnemies);
        }
    }

    void Update()
    {
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2d.MovePosition(position);
    }

    // Замінено на OnCollisionStay2D: шкода перевіряється в кожному кадрі фізичного контакту
    void OnCollisionStay2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        if (!broken) return;

        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");

        fixedEnemies++;

        if (Beginner2D.UIHandler.instance != null)
        {
            Beginner2D.UIHandler.instance.SetCounter(fixedEnemies, totalEnemies);

            if (fixedEnemies >= totalEnemies)
            {
                Beginner2D.UIHandler.instance.DisplayWinScreen();
            }
        }
    }
}