using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction; 

    void Start()
    {
        MoveAction.Enable(); 
    }

    void Update()
    {
        Vector2 move = MoveAction.ReadValue<Vector2>();
        Vector2 position = transform.position;

      
        position.x += move.x * 3.0f * Time.deltaTime;
        position.y += move.y * 3.0f * Time.deltaTime;

        transform.position = position;
    }
}