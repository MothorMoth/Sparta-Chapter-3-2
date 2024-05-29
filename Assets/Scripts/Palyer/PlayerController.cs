using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    public float jumpCost;
    public LayerMask groundLayerMask;
    private Vector2 _moveInput;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXRotation;
    public float maxXRotation;
    public float lookSensitivity;
    private float _cameraXRotation;
    private Vector2 _lookInput;

    private Rigidbody _rigidbody;
    private Vector3 _resetPosition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _resetPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position.y <= -15f)
        {
            ResetPosition();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _moveInput = Vector2.zero;
        }
    }

    private void Move()
    {
        Vector3 direction = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        direction *= moveSpeed;
        direction.y = _rigidbody.velocity.y;

        _rigidbody.velocity = direction;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void CameraLook()
    {
        _cameraXRotation += _lookInput.y * lookSensitivity;
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, minXRotation, maxXRotation);
        cameraContainer.localEulerAngles = new Vector3(-_cameraXRotation, 0, 0);

        transform.eulerAngles += new Vector3(0, _lookInput.x * lookSensitivity, 0);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            PlayerManager.Instance.Player.condition.UseStamina(jumpCost);
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    private void ResetPosition()
    {
        transform.position = _resetPosition;
        PlayerManager.Instance.Player.condition.TakeDamage(1);
    }
}
