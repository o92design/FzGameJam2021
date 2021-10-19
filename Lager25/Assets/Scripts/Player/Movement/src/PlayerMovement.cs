using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/* Code built upon https://github.com/IsaiahKelly/quake3-movement-for-unity/blob/master/Quake3Movement/Scripts/Q3PlayerController.cs */
[Serializable]
public struct MovementData
{
  public CharacterController m_Character;
  public Camera m_Camera;
  public bool Jump;
  public float m_Friction;
  public float m_Gravity;
  public float m_JumpForce;
  [Tooltip("How precise air control is")]
  public float m_AirControl;
  [Tooltip("Automatically jump when holding jump button")]
  public bool m_AutoBunnyHop;

  public Vector3 m_MoveDirectionNorm;
  public Vector3 m_PlayerVelocity;
  public float Speed { get { return m_Character.velocity.magnitude; } }
}

[System.Serializable]
public struct MovementSettings
{
  public float MaxSpeed;
  public float Deceleration;
  public float Acceleration;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
  [Header("Movement")]
  public MovementData m_Data;
  public MovementSettings m_GroundSettings;
  public MovementSettings m_AirSettings;
  public MovementSettings m_StrafeSettings;

  // Used to queue the next jump just before hitting the ground.
  private bool m_JumpQueued = false;

  // Used to display real time friction values.
  private float m_PlayerFriction = 0;

  private Vector3 m_MoveInput;
  private Transform m_Tran;
  private Transform m_CamTran;

  private void Start()
  {
    m_Tran = transform;
    m_Data.m_Character = GetComponent<CharacterController>();

    if (!m_Data.m_Camera)
      m_Data.m_Camera = Camera.main;

    m_CamTran = m_Data.m_Camera.transform;
  }

  public void Move(InputAction.CallbackContext p_Context)
  {
    m_MoveInput = new Vector3(p_Context.ReadValue<Vector2>().x, 0, p_Context.ReadValue<Vector2>().y);
  }

  public void Jump(InputAction.CallbackContext p_context)
  {
    m_Data.Jump = p_context.performed;
  }

  private void Update()
  {
    
    QueueJump();

    // Set movement state.
    if (m_Data.m_Character.isGrounded)
    {
      GroundMove();
    }
    else
    {
      AirMove();
    }

    // Move the character.
    m_Data.m_Character.Move(m_Data.m_PlayerVelocity * Time.deltaTime);
  }

  // Queues the next jump.
  private void QueueJump()
  {
    if (m_Data.m_AutoBunnyHop)
    {
      m_JumpQueued = m_Data.Jump;
      return;
    }

    if (m_Data.Jump && !m_JumpQueued)
    {
      m_JumpQueued = true;
    }

    if (!m_Data.Jump)
    {
      m_JumpQueued = false;
    }
  }

  // Handle air movement.
  private void AirMove()
  {
    float accel;

    var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
    wishdir = m_Tran.TransformDirection(wishdir);

    float wishspeed = wishdir.magnitude;
    wishspeed *= m_AirSettings.MaxSpeed;

    wishdir.Normalize();
    m_Data.m_MoveDirectionNorm = wishdir;

    // CPM Air control.
    float wishspeed2 = wishspeed;
    if (Vector3.Dot(m_Data.m_PlayerVelocity, wishdir) < 0)
    {
      accel = m_AirSettings.Deceleration;
    }
    else
    {
      accel = m_AirSettings.Acceleration;
    }

    // If the player is ONLY strafing left or right
    if (m_MoveInput.z == 0 && m_MoveInput.x != 0)
    {
      if (wishspeed > m_StrafeSettings.MaxSpeed)
      {
        wishspeed = m_StrafeSettings.MaxSpeed;
      }

      accel = m_StrafeSettings.Acceleration;
    }

    Accelerate(wishdir, wishspeed, accel);
    if (m_Data.m_AirControl > 0)
    {
      AirControl(wishdir, wishspeed2);
    }

    // Apply gravity
    m_Data.m_PlayerVelocity.y -= m_Data.m_Gravity * Time.deltaTime;
  }

  // Air control occurs when the player is in the air, it allows players to move side 
  // to side much faster rather than being 'sluggish' when it comes to cornering.
  private void AirControl(Vector3 targetDir, float targetSpeed)
  {
    // Only control air movement when moving forward or backward.
    if (Mathf.Abs(m_MoveInput.z) < 0.001 || Mathf.Abs(targetSpeed) < 0.001)
    {
      return;
    }

    float zSpeed = m_Data.m_PlayerVelocity.y;
    m_Data.m_PlayerVelocity.y = 0;
    /* Next two lines are equivalent to idTech's VectorNormalize() */
    float speed = m_Data.m_PlayerVelocity.magnitude;
    m_Data.m_PlayerVelocity.Normalize();

    float dot = Vector3.Dot(m_Data.m_PlayerVelocity, targetDir);
    float k = 32;
    k *= m_Data.m_AirControl * dot * dot * Time.deltaTime;

    // Change direction while slowing down.
    if (dot > 0)
    {
      m_Data.m_PlayerVelocity.x *= speed + targetDir.x * k;
      m_Data.m_PlayerVelocity.y *= speed + targetDir.y * k;
      m_Data.m_PlayerVelocity.z *= speed + targetDir.z * k;

      m_Data.m_PlayerVelocity.Normalize();
      m_Data.m_MoveDirectionNorm = m_Data.m_PlayerVelocity;
    }

    m_Data.m_PlayerVelocity.x *= speed;
    m_Data.m_PlayerVelocity.y = zSpeed; // Note this line
    m_Data.m_PlayerVelocity.z *= speed;
  }

  // Handle ground movement.
  private void GroundMove()
  {
    // Do not apply friction if the player is queueing up the next jump
    if (!m_JumpQueued)
    {
      ApplyFriction(1.0f);
    }
    else
    {
      ApplyFriction(0);
    }

    var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
    wishdir = m_Tran.TransformDirection(wishdir);
    wishdir.Normalize();
    m_Data.m_MoveDirectionNorm = wishdir;

    var wishspeed = wishdir.magnitude;
    wishspeed *= m_GroundSettings.MaxSpeed;

    Accelerate(wishdir, wishspeed, m_GroundSettings.Acceleration);

    // Reset the gravity velocity
    m_Data.m_PlayerVelocity.y = -m_Data.m_Gravity * Time.deltaTime;

    if (m_JumpQueued)
    {
      m_Data.m_PlayerVelocity.y = m_Data.m_JumpForce;
      m_JumpQueued = false;
    }
  }

  private void ApplyFriction(float t)
  {
    // Equivalent to VectorCopy();
    Vector3 vec = m_Data.m_PlayerVelocity;
    vec.y = 0;
    float speed = vec.magnitude;
    float drop = 0;

    // Only apply friction when grounded.
    if (m_Data.m_Character.isGrounded)
    {
      float control = speed < m_GroundSettings.Deceleration ? m_GroundSettings.Deceleration : speed;
      drop = control * m_Data.m_Friction * Time.deltaTime * t;
    }

    float newSpeed = speed - drop;
    m_PlayerFriction = newSpeed;
    if (newSpeed < 0)
    {
      newSpeed = 0;
    }

    if (speed > 0)
    {
      newSpeed /= speed;
    }

    m_Data.m_PlayerVelocity.x *= newSpeed;
    // m_Data.playerVelocity.y *= newSpeed;
    m_Data.m_PlayerVelocity.z *= newSpeed;
  }

  // Calculates acceleration based on desired speed and direction.
  private void Accelerate(Vector3 targetDir, float targetSpeed, float accel)
  {
    float currentspeed = Vector3.Dot(m_Data.m_PlayerVelocity, targetDir);
    float addspeed = targetSpeed - currentspeed;
    if (addspeed <= 0)
    {
      return;
    }

    float accelspeed = accel * Time.deltaTime * targetSpeed;
    if (accelspeed > addspeed)
    {
      accelspeed = addspeed;
    }

    m_Data.m_PlayerVelocity.x += accelspeed * targetDir.x;
    m_Data.m_PlayerVelocity.z += accelspeed * targetDir.z;
  }
}
