using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// �÷��̾��� ��Ʈ�ѷ�
/// </summary>
public class PlayerController : MonoBehaviour
{
    // �̵� ����
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    public LayerMask grounLayerMask;

    private Vector2 _curMovementInput;

    // ������
    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    public float lookSensitivity; // �ΰ���

    private float _camCurXRot;
    private Vector2 mouseDelta;

    [HideInInspector]
    public bool canLook = true;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    /// <summary>
    ///  ������ �� �ʿ��� �� �Ҵ�
    /// </summary>
    private void Move()
    {
        // ������ �̵��ϸ� inputaction���� �޾ƿ� y��, �������ΰ��� inputaction���� �޾ƿ� x���� ���Ѵ�.
        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x; 
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    /// <summary>
    /// ���콺�� ī�޶� ������
    /// </summary>
    void CameraLook()
    {
        _camCurXRot += mouseDelta.y * lookSensitivity;
        _camCurXRot = Mathf.Clamp( _camCurXRot, minXLook, maxXLook );
        cameraContainer.localEulerAngles = new Vector3(-_camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    /// <summary>
    /// ���콺 ��Ÿ������ �þ� ���� �� InputSystem �޾ƿ�
    /// </summary>
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }


    /// <summary>
    /// InputSystem ���� ������ ����
    /// </summary>
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
        }

    }

    /// <summary>
    /// ���� InputSystem
    /// </summary>
    /// <param name="context"></param>
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if( context.phase == InputActionPhase.Started)
        {
            if(IsGround())
            {
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
                Debug.Log("«Ǫ");
               
            }
        }
    }


    /// <summary>
    /// Ground üũ
    /// </summary>
    private bool IsGround()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position +(-transform.right * 0.2f) +(Vector3.up * 0.01f), Vector3.down),
        };

        // 4�� �ϳ��� ground�� �´�Ҵٸ�
        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, grounLayerMask)) 
            {
                return true;
            }
        }

        return false;

    }

    /// <summary>
    /// Gizmos �׸���
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
       Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
       Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
       Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
       Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }
}