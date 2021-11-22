using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
  [SerializeField] private EnemyController enemyController;
  [SerializeField] private float multiplier = 1f;

  public void Damage(int dmg)
  {
    enemyController.Damage(Mathf.FloorToInt(dmg * multiplier));
  }
}
