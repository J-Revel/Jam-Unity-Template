using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardMovement : MonoBehaviour
{
    public InputAction horizontalAction;
    public InputAction verticalAction;
    private MovementController movementController;

    void Start()
    {
        movementController = GetComponent<MovementController>();
        horizontalAction.Enable();
        verticalAction.Enable();
    }

    void Update()
    {
        movementController.inputDirection = new Vector3(horizontalAction.ReadValue<float>(), 0, verticalAction.ReadValue<float>());
    }
}
