using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //----------Components----------
    //Player Components
    private Rigidbody rb;

    //Camera Components
    [Header("Cameras")] public CinemachineCamera vcam3D;
    public CinemachineCamera vcam2D;

    public CinemachineCamera vcam2DOther;

    //Colliders Components
    public Collider meleeCollider;

    //Animator Components
    public Animator animator;

    //----------Movement and Rotation----------
    //Player Movement
    private Vector2 inputVector;

    private float moveSpeed = 5f;

    //----------Interactions----------
    public float meleeDuration = 0.2f;

    //----------Camera variables----------
    private bool is2DMode = false;
    private bool is2DModeOther = false;

    public Transform cameraPivot;

    //----------First Person----------
    public Transform cameraTransform;
    public float sensitivity = 0.5f;
    public float minLimit = -80f;
    public float maxLimit = 80f;
    private PlayerInputActions _inputAction;
    private CharacterController _characterController;
    private Vector2 _look;
    private float _currentRotationY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        meleeCollider.enabled = false;

        vcam3D.enabled = true;
        vcam2D.enabled = false;
        vcam2DOther.enabled = false;
    }


    void FixedUpdate()
    {
        Vector3 moveDirection = default;

        MovePlayer(moveDirection);
        if (!is2DMode)
        {
            Look();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            inputVector = context.ReadValue<Vector2>();

            bool isMoving = inputVector.sqrMagnitude > 0.01f;

            animator.SetBool("IsWalkingForward", isMoving);

            Debug.Log("isMoving: " + isMoving);
        }
    }
    
    private void MovePlayer(Vector3 moveDirection)
    {
        if (is2DMode && !is2DModeOther)
        {
            moveDirection = new Vector3(0f, 0f, inputVector.x);
        }
        else if (is2DMode && is2DModeOther)
        {
            moveDirection = new Vector3(-inputVector.x, 0f, 0f);
        }
        else
        {
            moveDirection = (transform.forward * inputVector.y) + (transform.right * inputVector.x);
        }

        rb.MovePosition(rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime));
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _look = context.ReadValue<Vector2>();
        }

        if (context.canceled)
        {
            _look = Vector2.zero;
        }
    }

    private void Look()
    {
        Vector2 mouseNormalized = _look * sensitivity;

        _currentRotationY = Mathf.Clamp(_currentRotationY - mouseNormalized.y, minLimit, maxLimit);
        cameraTransform.localRotation = Quaternion.Euler(_currentRotationY, 0, 0);

        transform.Rotate(Vector3.up * _look.x);
    }
    
    public void OnChangeMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            is2DMode = !is2DMode;
            if (is2DMode)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX |
                                 RigidbodyConstraints.FreezeRotationZ;
                vcam3D.Priority.Enabled = false;
                vcam2D.Priority.Enabled = true;
                vcam2DOther.Priority.Enabled = false;
                transform.position = new Vector3(11.33f, 1.9f, 1.51f);
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                vcam3D.Priority.Enabled = true;
                vcam2D.Priority.Enabled = false;
                vcam2DOther.Priority.Enabled = false;
                transform.position = new Vector3(1.28f, 7.49f, 1.51f);
            }
        }
    }

    public void OnChangeMovement2D(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (is2DMode)
            {
                is2DModeOther = !is2DModeOther;
                if (is2DModeOther)
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                    transform.rotation = Quaternion.Euler(0f, 270f, 0f);
                    vcam3D.Priority.Enabled = false;
                    vcam2D.Priority.Enabled = false;
                    vcam2DOther.Priority.Enabled = true;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
                    vcam3D.Priority.Enabled = false;
                    vcam2D.Priority.Enabled = true;
                    vcam2DOther.Priority.Enabled = false;
                }
            }
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GunRaycast gunRaycast = GetComponentInChildren<GunRaycast>();
            gunRaycast.showMuzzleFlash();
            if (gunRaycast != null && gunRaycast.enemyTarget != null && gunRaycast.hasHitEnemy)
            {
                Vector3 hitDirection = (gunRaycast.enemyTarget.transform.position - transform.position).normalized;
                gunRaycast.enemyTarget.GetHit(25f, hitDirection);
            }
        }
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartCoroutine(DoMelee());
        }
    }

    private System.Collections.IEnumerator DoMelee()
    {
        meleeCollider.enabled = true;
        yield return new WaitForSeconds(meleeDuration);
        meleeCollider.enabled = false;
    }
}