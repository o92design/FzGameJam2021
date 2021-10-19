using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct ShootingData
{
  public bool Shooting;
  public Transform m_SpawnPoint;
  public GameObject m_BulletHole;
  public float m_ShootingCooldown;
}

public class Shooting : MonoBehaviour
{
  public ShootingData m_Data;
  private bool m_canShoot = true;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(m_Data.Shooting && m_canShoot)
    {
      Debug.Log("Shooting");
      ShootRayFromSpawnPoint();
      CooldownStart();
    }
  }
  public void CooldownStart()
  {
    StartCoroutine(CooldownCoroutine());
  }
  IEnumerator CooldownCoroutine()
  {
    m_canShoot = false;
    yield return new WaitForSeconds(m_Data.m_ShootingCooldown);
    m_canShoot = true;
  }

  private void ShootRayFromSpawnPoint()
  {
    RaycastHit hit;
    Debug.DrawRay(m_Data.m_SpawnPoint.position, m_Data.m_SpawnPoint.forward, Color.yellow, 1f);
    if (Physics.Raycast(m_Data.m_SpawnPoint.position, m_Data.m_SpawnPoint.forward, out hit, 200f))
    {
      Debug.DrawLine(m_Data.m_SpawnPoint.position, hit.point, Color.green, 1f);
      Debug.Log("Hit " + hit.collider.gameObject.name);
      Vector3 position = hit.point;
      GameObject decal = Instantiate(m_Data.m_BulletHole);
      decal.transform.position = position;
      decal.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
      decal.transform.position -= decal.transform.forward * 0.02f;
      Destroy(decal, 3f);
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
