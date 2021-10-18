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

  public void Start()
  {
    m_data.CharacterController = GetComponent<CharacterController>();
    m_data.Movement = new Vector3();
  }

  public void FixedUpdate()
  {
    m_data.CharacterController.SimpleMove(m_data.Movement * Time.deltaTime);
  }

  public void OnMove(InputValue p_value)
  {
    // TODO(oskar): Make this branchless bound check - see casey muratori lecture on performance
    float directionY = p_value.Get<Vector2>().y;
    float directionX = p_value.Get<Vector2>().x;
    int directionForward = directionY > 0 ? 1 :
                           directionY < 0 ? -1 : 
                           0;
    int directionStrafe = directionX > 0 ? 1 :
                          directionX < 0 ? -1 :
                          0;

    Vector3 forward = transform.forward * directionForward;
    Vector3 strafe = transform.right * directionStrafe;

    m_data.Movement.z = forward.z;
    m_data.Movement.x = strafe.x;
    m_data.Movement *= m_data.Speed;
  }

}
