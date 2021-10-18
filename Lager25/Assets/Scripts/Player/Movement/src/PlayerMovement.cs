using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[Serializable]
public struct MovementData
{
  public float Speed;
  public Vector3 Movement;
  public CharacterController CharacterController;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
  public MovementData m_data;

  private int m_forward;
  private int m_strafe;

  public void Start()
  {
    m_data.CharacterController = GetComponent<CharacterController>();
    m_data.Movement = new Vector3();
  }

  public void FixedUpdate()
  {
    m_data.Movement = (transform.forward * m_forward + transform.right * m_strafe) * m_data.Speed;
    m_data.CharacterController.SimpleMove(m_data.Movement * Time.deltaTime);
  }

  public void Move(InputAction.CallbackContext p_action)
  {
    // TODO(oskar): Make this branchless bound check - see casey muratori lecture on performance
    float directionY = p_action.ReadValue<Vector2>().y;
    float directionX = p_action.ReadValue<Vector2>().x;
    m_forward = directionY > 0 ? 1 :
                directionY < 0 ? -1 :
                0;
    m_strafe = directionX > 0 ? 1 :
               directionX < 0 ? -1 :
               0;

  }
}
