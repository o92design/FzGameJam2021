using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct RotationData
{
  public Camera Camera;
  public float LookX;
  public float LookY;
  public float sensitivity;

  public float MinY;
  public float MaxY;
}

public class PlayerRotation : MonoBehaviour
{
  public RotationData m_data;

  public void FixedUpdate()
  {
    Look();
  }

  public void LookUpdate(InputAction.CallbackContext p_input)
  {
    m_data.LookX += p_input.ReadValue<Vector2>().x * m_data.sensitivity;
    m_data.LookY -= p_input.ReadValue<Vector2>().y * m_data.sensitivity;
  }

  void Look()
  {
    m_data.LookY = Mathf.Clamp(m_data.LookY, m_data.MinY, m_data.MaxY);
    transform.localEulerAngles = new Vector3(0, m_data.LookX, 0);
    m_data.Camera.transform.localEulerAngles = new Vector3(m_data.LookY, 0, 0);
  }
}
