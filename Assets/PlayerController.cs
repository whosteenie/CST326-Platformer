using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    private Label _labelTime;
    private int _timeRemaining = 400;
    private float _timeAccumulator;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        var uiDocument = FindFirstObjectByType<UIDocument>();
        if (uiDocument != null)
            _labelTime = uiDocument.rootVisualElement.Q<Label>("labelTime");
        if (_labelTime != null)
            _labelTime.text = $"TIME\n {_timeRemaining}";
    }

    void OnAttack()
    {
        var cam = Camera.main;
        if (cam == null)
            return;

        var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit))
            return;

        var hitObject = hit.collider.gameObject;
        var block = hitObject.name;
        if (block.Contains("Brick"))
            Destroy(hitObject);
    }

    void Update()
    {
        if (_labelTime != null && _timeRemaining > 0)
        {
            _timeAccumulator += Time.deltaTime;
            if (_timeAccumulator >= 1f)
            {
                _timeAccumulator -= 1f;
                _timeRemaining--;
                _labelTime.text = $"TIME\n {_timeRemaining}";
            }
        }

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
