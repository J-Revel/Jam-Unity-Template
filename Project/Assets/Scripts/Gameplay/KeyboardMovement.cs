using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardMovement : MonoBehaviour
{
    public InputAction horizontalAction;
    public InputAction verticalAction;
    private MovementController movementController;
    private Rigidbody body;
    public SoundEvent movement_sound_event;
    private bool moving = false;
    public float max_sound_speed = 5;

    void Start()
    {
        movementController = GetComponent<MovementController>();
        horizontalAction.Enable();
        verticalAction.Enable();
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movementController.inputDirection = new Vector3(horizontalAction.ReadValue<float>(), 0, verticalAction.ReadValue<float>());
        movement_sound_event.param_value = Mathf.Clamp01(body.velocity.magnitude / max_sound_speed); 
        if (!moving && movementController.inputDirection != Vector3.zero)
        {
            moving = true;
            movement_sound_event.Trigger();
        }
        else if (moving && movementController.inputDirection == Vector3.zero)
            moving = false;
    }
}
