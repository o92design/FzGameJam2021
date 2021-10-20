using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ZoneData
{
  public int Team;
  
  public int TakeOverValue;
  public int MaxTakeOverValue;
  public float TakeOverSeconds;
  
  public int CurrentTakeOverRate;

  public Color TeamColor;
}

public class ZoneArea : MonoBehaviour, IZoneArea
{
  public ZoneData m_Data;
  private TakeOverData m_TeamTakeOver;

  [SerializeField]
  private Dictionary<int, List<TakeOverData>> m_TakeOvers = new Dictionary<int, List<TakeOverData>>();
  private bool m_initiatedCount = false;

  public void Awake()
  {
    m_Data.TeamColor = GetComponent<Renderer>().material.color;
  }

  public void LateUpdate()
  {
    CalculateTakeOverRate();

    if(BeingOvertaken() && !m_initiatedCount)
    {
      StartCoroutine(TakeOver());
    }
  }

  /// <summary>
  /// TODO(Oskar): Implement 3+ team takeover
  /// </summary>
  public void CalculateTakeOverRate()
  {
    if(m_TakeOvers.Count == 0)
    {
      m_Data.CurrentTakeOverRate = 0;
      return;
    }

    Dictionary<int, int> teamTakeOverRate = new Dictionary<int, int>();
    foreach (int team in m_TakeOvers.Keys)
    {
      teamTakeOverRate.Add(team, 0);
      if(m_Data.Team != team)
      {
        foreach (TakeOverData data in m_TakeOvers[team])
        {
          teamTakeOverRate[team] += m_Data.Team == 0 ? data.TakeOverRate :
                                    m_Data.Team != data.Team ? data.TakeOverRate : -data.TakeOverRate;

          m_TeamTakeOver = data;
        }
      }

      m_Data.CurrentTakeOverRate = teamTakeOverRate[team];
    }
  }

  IEnumerator TakeOver()
  {
    if(m_Data.TakeOverValue >=  m_Data.MaxTakeOverValue)
    {
      m_Data.TakeOverValue = m_Data.MaxTakeOverValue;
      ChangeOwner(m_TeamTakeOver);
      yield return null;
    }

    m_initiatedCount = true;
    ChangeZoneColor(m_TeamTakeOver.Color);
    yield return new WaitForSeconds(m_Data.TakeOverSeconds * 0.5f);
   
    m_Data.TakeOverValue += m_Data.CurrentTakeOverRate;
    ChangeZoneColor(m_Data.TeamColor);
    yield return new WaitForSeconds(m_Data.TakeOverSeconds * 0.5f);
    m_initiatedCount = false;
  }

  private void ChangeZoneColor(Color p_color)
  {
    p_color.a = 0.5f;
    GetComponent<Renderer>().material.color = p_color;
  }

  public bool BeingOvertaken()
  {
    return m_Data.CurrentTakeOverRate != 0;
  }

  public void ChangeOwner(TakeOverData p_takeOverData)
  {
    m_Data.Team = p_takeOverData.Team;
    Color color = p_takeOverData.Color;
    m_Data.TeamColor = color;
    ChangeZoneColor(color);
  }

  public bool AddTakeOverData(TakeOverData p_takeOverData)
  {
    bool addedData = false;

    // Note(Oskar): IF the team is brand new Add key and list with take over data
    if (!m_TakeOvers.ContainsKey(p_takeOverData.Team))
    {
      m_TakeOvers.Add(p_takeOverData.Team, new List<TakeOverData>());
      m_TakeOvers[p_takeOverData.Team].Add(p_takeOverData);
      addedData = true;
    }
    // Note(Oskar): IF the team exist then add only take over data
    else if (!m_TakeOvers[p_takeOverData.Team].Contains(p_takeOverData))
    {
      m_TakeOvers[p_takeOverData.Team].Add(p_takeOverData);
      addedData = true;
    }

    return addedData;
  }

  public bool RemoveTakeOverData(TakeOverData p_takeOverData)
  {
    bool removedData = false;
    if(m_TakeOvers.ContainsKey(p_takeOverData.Team))
    {
      if(m_TakeOvers[p_takeOverData.Team].Contains(p_takeOverData))
      {
        m_TakeOvers[p_takeOverData.Team].Remove(p_takeOverData);
        removedData = true;
        Debug.Log("Removed Takeover");
      }
    }

    return removedData;
  }

  public void OnTriggerEnter(Collider other)
  {
    TakeOverData enteredTakeOver = other.GetComponent<TakeOverData>();
    AddTakeOverData(enteredTakeOver);
  }

  public void OnTriggerExit(Collider other)
  {
    TakeOverData exitedTakeOver = other.GetComponent<TakeOverData>();
    RemoveTakeOverData(exitedTakeOver);
  }

  public int GetTeamOwner()
  {
    return m_Data.Team;
  }
}
