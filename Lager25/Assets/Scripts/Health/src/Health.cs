using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
  public int Value = 100;

  /// <summary>
  /// Changes the value of the Health with given parameter
  /// </summary>
  /// <param name="p_Value">Value to change Health</param>
  /// <returns>bool to check if value is below or equal to 0</returns>
  public bool ChangeHealth(int p_Value)
  {
    Value += p_Value;

    return Value <= 0;
  }
}
