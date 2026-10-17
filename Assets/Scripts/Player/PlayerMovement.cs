using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D body;
    private Animator animator;

    private Vector2 movement;
    private Vector2 facingDirection = Vector2.down;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    private static readonly int DirectionX = Animator.StringToHash("DirectionX");

    private static readonly int DirectionY = Animator.StringToHash("DirectionY");
    public bool MovementEnabled { get; private set; } = true;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 input = ReadInput();
        movement = Vector2.ClampMagnitude(input, 1f);

        bool isMoving = movement.sqrMagnitude > 0f;

        if (isMoving)
        {
            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            {
                facingDirection = new Vector2(Mathf.Sign(input.x), 0f);
            }
            else
            {
                facingDirection = new Vector2(0f, Mathf.Sign(input.y));
            }
        }

        animator.SetBool(IsMoving, isMoving);
        animator.SetFloat(DirectionX, facingDirection.x);
        animator.SetFloat(DirectionY, facingDirection.y);
    }

    private void FixedUpdate()
    {
        body.linearVelocity = MovementEnabled ? movement * moveSpeed : Vector2.zero;
    }

    private Vector2 ReadInput()
    {
        Vector2 keyboardInput = Vector2.zero;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed) keyboardInput.x -= 1f;
            if (keyboard.dKey.isPressed) keyboardInput.x += 1f;
            if (keyboard.sKey.isPressed) keyboardInput.y -= 1f;
            if (keyboard.wKey.isPressed) keyboardInput.y += 1f;
        }

        //keyboard has priority over gamepad input
        if (keyboardInput.sqrMagnitude > 0f)
            return keyboardInput;

        Gamepad gamepad = Gamepad.current;

        if (gamepad == null)
            return Vector2.zero;

        Vector2 dpadInput = gamepad.dpad.ReadValue();

        if (dpadInput.sqrMagnitude > 0f)
            return dpadInput;

        return gamepad.leftStick.ReadValue();
    }

    public void SetMovementEnabled(bool value)
    {
        MovementEnabled = value;
        movement = Vector2.zero;
        body.linearVelocity = Vector2.zero;
        animator.SetBool(IsMoving, false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
