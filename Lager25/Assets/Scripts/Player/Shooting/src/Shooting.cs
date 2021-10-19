using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct ShootingData
{
  public bool Shooting;
}

public class Shooting : MonoBehaviour
{
  public ShootingData m_Data;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(m_Data.Shooting)
    {
      Debug.Log("Shooting");
    }
  }

  public void Fire(InputAction.CallbackContext p_context)
  {
    m_Data.Shooting = p_context.performed;
  }

  public void Fire()
  {
    m_Data.Shooting = true;
  }

  public void StopFire()
  {
    m_Data.Shooting = false;
  }
}
