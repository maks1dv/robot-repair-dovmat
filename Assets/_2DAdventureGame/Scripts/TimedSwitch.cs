using System.Collections;
using UnityEngine;

public class TimedSwitch : MonoBehaviour
{
    public bool active = true;
    public GameObject door;
    public float openTime = 4.0f;

    private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active || door == null)
        {
            return;
        }

        if (other.GetComponent<PlayerController>() == null)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(OpenDoorFor(openTime));
    }

    IEnumerator OpenDoorFor(float seconds)
    {
        door.SetActive(false);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
        }

        yield return new WaitForSeconds(seconds);

        door.SetActive(true);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColor;
        }
    }
}