using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float groundAcceleration = 15f;
    private Vector2 _velocity;

    public float jumpForce = 5f;
    public float apexHeight = 4.5f;
    public float apexTime = 0.5f;
    
    private CharacterController _controller;
    [SerializeField] private GameObject mario;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        var direction = 0f;
        if(Keyboard.current.dKey.isPressed) direction = 1f;
        if(Keyboard.current.aKey.isPressed) direction = -1f;
        var jumpPressedThisFrame = Keyboard.current.spaceKey.wasPressedThisFrame;
        var jumpHeld = Keyboard.current.spaceKey.isPressed;

        var gravityModifier = 1f;
        
        if (direction != 0f)
        {
            _velocity.x += direction * groundAcceleration * Time.deltaTime;
            var speed = Keyboard.current.shiftKey.isPressed ? runSpeed : walkSpeed;
            _velocity.x = Mathf.Clamp(_velocity.x, -speed, speed);
        }
        else
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, groundAcceleration * Time.deltaTime);
        }
        
        if (_controller.isGrounded)
        {
            if (jumpPressedThisFrame)
                _velocity.y = jumpForce * apexHeight / apexTime;
        }
        else
        {
            if (!jumpHeld)
                gravityModifier = 2f;
        }

        var gravity = 2f * apexHeight / (apexTime * apexTime);
        _velocity.y -= gravity * gravityModifier * Time.deltaTime;
        
        _velocity.y = Mathf.Clamp(_velocity.y, -gravity, 200);

        var deltaX = _velocity.x * Time.deltaTime;
        var deltaY = _velocity.y * Time.deltaTime;
        var deltaPosition = new Vector3(deltaX, deltaY, 0);
        var collisions = _controller.Move(deltaPosition);

        if ((collisions & CollisionFlags.CollidedAbove) != 0)
        {
            _velocity.y = -1f;
        }

        if ((collisions & CollisionFlags.CollidedSides) != 0)
        {
            _velocity.x = 0f;
        }
    }
}
