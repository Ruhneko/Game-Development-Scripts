using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Automatic;

[CreateAssetMenu]
public class WeaponScriptableObject : ScriptableObject
{
  public new string name;
  public GameObject weaponObj;
  public Automatic automatic;
  public int clipSize;
  public float weaponSpray;
  public float weaponAccuracy;
  public float shootingDistance;
  public float fireSpeed;
  public float reloadTime;
  public int bulletPenetrationCount;
  public int maxDamage;
  public int minDamage;
  public LayerMask layerMask;
  public AudioClip audioClip;
  public AudioClip audioReload;
}
