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
    [Header("Cameras")]
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

    public Transform playerMesh;

    //----------First Person----------
    public Transform cameraTransform;
    public float sensitivity = 0.5f;
    public float minLimit = -80f;
    public float maxLimit = 80f;
    private PlayerInputActions _inputAction;
    private CharacterController _characterController;
    private Vector2 _look;
    private float _currentRotationY;

    private bool _invert2DMovement = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        meleeCollider.enabled = false;

        vcam2D.Priority.Enabled = false;
        vcam2D.gameObject.SetActive(false);

        vcam2D.Priority.Enabled = false;
        vcam2DOther.gameObject.SetActive(false);
        
        transform.position = new Vector3(-10.99936f, 4.489f, 12.96298f);
        transform.rotation = Quaternion.Euler(0, 135.848f, 0);
    }


    void FixedUpdate()
    {
        Vector3 moveDirection = default;

        MovePlayer(moveDirection);
        if (!is2DMode)
        {
            Look();
        }

        if (rb.position.Equals(new Vector3(11.325f, 0.937f, 3.01f)))
        {
            transform.rotation = Quaternion.Euler(0, 270f, 0);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            inputVector = context.ReadValue<Vector2>();

            bool isMoving = inputVector.sqrMagnitude > 0.01f;

            animator.SetBool("IsWalkingForward", isMoving);
        }
    }
    
    private void MovePlayer(Vector3 moveDirection)
    {
        if (is2DMode)
        {
            float dir = _invert2DMovement ? -1f : 1f;
            moveDirection = transform.forward * (inputVector.x * dir);
            
            if (inputVector.x > 0)         // derecha
                playerMesh.localRotation = Quaternion.Euler(0, (_invert2DMovement) ? 180f : 0f, 0);
            else if (inputVector.x < 0)    // izquierda
                playerMesh.localRotation = Quaternion.Euler(0, (_invert2DMovement) ? 0 : 180f, 0);
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
        if (!Application.isFocused) return;
        
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
                rb.constraints = RigidbodyConstraints.FreezeRotation;

                vcam2D.Priority.Enabled = true;
                vcam2D.gameObject.SetActive(true);
                cameraTransform.gameObject.SetActive(false);
                transform.position = new Vector3(11.325f, 0.937f, 12.96298f);
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                vcam2D.Priority.Enabled = false;
                vcam2D.gameObject.SetActive(false);
                cameraTransform.gameObject.SetActive(true);
                transform.position = new Vector3(-10.99936f, 4.489f, 12.96298f);
                transform.rotation = Quaternion.Euler(0, 135.848f, 0);
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
                    
                    vcam2D.Priority.Enabled = false;
                    vcam2D.gameObject.SetActive(false);
                    
                    vcam2DOther.Priority.Enabled = true;
                    vcam2DOther.gameObject.SetActive(true);
                    
                    _invert2DMovement = !_invert2DMovement;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                    
                    vcam2D.Priority.Enabled = true;
                    vcam2D.gameObject.SetActive(true);
                    
                    vcam2DOther.Priority.Enabled = false;
                    vcam2DOther.gameObject.SetActive(false);
                    
                    _invert2DMovement = !_invert2DMovement;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ChangeView"))
        {
            ChangeViewController trigger = other.gameObject.GetComponent<ChangeViewController>();
            
            if (trigger != null) 
            {
                Debug.Log(transform.forward);
                trigger.RotatePlayer(transform.forward);
            }
        }
    }
}