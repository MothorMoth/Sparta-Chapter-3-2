using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float moveSpeedMultiplier;
    public float jumpForce;
    public float staminaCost;
    public LayerMask groundLayerMask;
    private Vector2 _moveInput;
    private bool _isRun;
    private IEnumerator _staminaCoroutine;
    private bool _isSpeedBuffActive;
    private float _speedBuffDuration;
    private float _speedBuffValue;
    private bool _isOnMovingFloor;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXRotation;
    public float maxXRotation;
    public float lookSensitivity;
    private float _cameraXRotation;
    private Vector2 _lookInput;
    public bool canLook = true;

    private Rigidbody _rigidbody;
    private Vector3 _resetPosition;
    private Vector3 _curMovingFloorVelocity;

    public Action inventory;

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
        if (canLook)
        {
            CameraLook();
        }
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

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _isRun = true;
            _staminaCoroutine = StaminaCoroutine();
            StartCoroutine(_staminaCoroutine);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isRun = false;
        }
    }

    private void Move()
    {
        Vector3 direction = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        direction *= _isRun ? moveSpeed * moveSpeedMultiplier : moveSpeed;
        direction *= _isSpeedBuffActive ? _speedBuffValue : 1f;
        direction.y = _rigidbody.velocity.y;

        _rigidbody.velocity = _isOnMovingFloor ? direction + _curMovingFloorVelocity : direction;
    }

    private IEnumerator StaminaCoroutine()
    {
        while (_isRun)
        {
            if (!CharacterManager.Instance.Player.condition.UseStamina(staminaCost))
            {
                _isRun = false;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void SetSpeedBuff(float duration, float value)
    {
        _speedBuffDuration = duration;
        _speedBuffValue = value;
        StartCoroutine(SpeedBuff());
    }

    private IEnumerator SpeedBuff()
    {
        _isSpeedBuffActive = true;

        while (_isSpeedBuffActive)
        {
            yield return new WaitForSeconds(_speedBuffDuration);
            _isSpeedBuffActive = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingFloor"))
        {
            collision.gameObject.TryGetComponent(out Rigidbody rb);
            _curMovingFloorVelocity = rb.velocity;
            _isOnMovingFloor = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingFloor"))
        {
            _isOnMovingFloor = false;
            _curMovingFloorVelocity = Vector3.zero;
        }
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
        CharacterManager.Instance.Player.condition.TakeDamage(1);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    private void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
