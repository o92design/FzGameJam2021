using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ZoneData
{
  public Team Team;
  
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
  private Dictionary<Team, List<TakeOverData>> m_TakeOvers = new Dictionary<Team, List<TakeOverData>>();
  private bool m_initiatedCount = false;

  public void Awake()
  {
    //m_Data.TeamColor = GetComponent<Renderer>().material.color;
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
    if(m_TakeOvers.Values.Count == 0)
    {
      m_Data.CurrentTakeOverRate = 0;
      return;
    }

    Dictionary<Team, int> teamTakeOverRate = new Dictionary<Team, int>();
    foreach (Team team in m_TakeOvers.Keys)
    {
      teamTakeOverRate.Add(team, 0);
      if(m_Data.Team != team)
      {
        foreach (TakeOverData data in m_TakeOvers[team])
        {
          teamTakeOverRate[team] += m_Data.Team == null ? data.TakeOverRate :
                                    m_Data.Team.GetNumber() != data.Team.GetNumber() ? -data.TakeOverRate : data.TakeOverRate;

          m_TeamTakeOver = data;
        }

        Debug.Log(team + " takeover Rate: " + teamTakeOverRate[team]);
      }

      m_Data.CurrentTakeOverRate = teamTakeOverRate[team];
    }
  }

  IEnumerator TakeOver()
  {
    if(m_Data.TakeOverValue <= 0 && m_Data.CurrentTakeOverRate < 0)
    {
      m_Data.TakeOverValue = 0;
      m_Data.Team = null;
    }

    if(m_Data.TakeOverValue >=  m_Data.MaxTakeOverValue && m_Data.CurrentTakeOverRate > 0)
    {
      m_Data.TakeOverValue = m_Data.MaxTakeOverValue;
      ChangeOwner(m_TeamTakeOver);
      yield return null;
    }

    m_initiatedCount = true;
    ChangeZoneColor(m_TeamTakeOver.Team.GetColor());
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
    Color color = p_takeOverData.Team.GetColor();
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
      Debug.Log("Added a new Team to take over the Zone!");
    }
    // Note(Oskar): IF the team exist then add only take over data
    else if (!m_TakeOvers[p_takeOverData.Team].Contains(p_takeOverData))
    {
      m_TakeOvers[p_takeOverData.Team].Add(p_takeOverData);
      addedData = true;
      Debug.Log("Team Exist - add takeover data for the Team:" + p_takeOverData.Team);
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

  public Team GetTeamOwner()
  {
    return m_Data.Team;
  }
}
