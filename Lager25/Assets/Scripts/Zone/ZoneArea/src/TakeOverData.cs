using UnityEngine;

public class TakeOverData : MonoBehaviour
{
  public Team Team;
  public int TakeOverRate;

  public void Start()
  {
    if(Team == null)
      Team = GetComponent<Team>();
  }
}
