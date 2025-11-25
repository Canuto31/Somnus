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

    //Player Rotation
    private float mouseSensitivity = 150f;
    private float xRotation = 0f;
    private Vector2 lookInput;

    //----------Interactions----------
    public float meleeDuration = 0.2f;

    //----------Camera variables----------
    private bool is2DMode = false;
    private bool is2DModeOther = false;

    public Transform cameraPivot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        meleeCollider.enabled = false;

        vcam3D.enabled = true;
        vcam2D.enabled = false;
        vcam2DOther.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!is2DMode)
        {
            HandleLook();
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = default;

        MovePlayer(moveDirection);
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

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotación horizontal del jugador (izquierda-derecha)
        transform.Rotate(Vector3.up * mouseX);

        // Rotación vertical SOLO del pivot de la cámara
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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