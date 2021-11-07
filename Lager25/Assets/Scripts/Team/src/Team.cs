using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Team : MonoBehaviour
{
  [SerializeField] private int m_Number;
  [SerializeField] private Color m_Color;

  public int GetNumber()
  {
    return m_Number;
  }

  public void SetNumber(int p_Number)
  {
    m_Number = p_Number;
  }

  public Color GetColor()
  {
    return m_Color;
  }

  public void SetColor(Color p_Color)
  {
    m_Color = p_Color;
  }
}
