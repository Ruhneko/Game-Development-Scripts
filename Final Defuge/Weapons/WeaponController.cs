using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;
using static Automatic;

public class WeaponController : MonoBehaviour
{
  private DefaultInput defaultInput;
  private CustomCharacterController characterController;

  [Header("Weapon")]
  public WeaponScriptableObject weapon;

  [Header("Settings")]
  [SerializeField] private WeaponSettingsModel settings;

  private Transform bulletTransform;
  private Vector3 startingPosition;
  [HideInInspector] public int currentClip;
  private bool firing = false;
  private bool reloading = false;
  private int enemyLayer;
  public float firingTick = 0f;

  [SerializeField] private AudioSource audioData, audioReload;

  bool initialized;

  Vector3 reloadingPosition;
  Vector3 recoilPosition;
  Vector3 targetWeaponPosition;
  Vector3 targetWeaponPositionVelocity;

  Vector3 newWeaponRotation;
  Vector3 newWeaponRotationVelocity;
  Vector3 targetWeaponRotation;
  Vector3 targetWeaponRotationVelocity;

  Vector3 newWeaponMovementRotation;
  Vector3 newWeaponMovementRotationVelocity;
  Vector3 targetWeaponMovementRotation;
  Vector3 targetWeaponMovementRotationVelocity;

  private void Awake()
  {
    defaultInput = new DefaultInput();

    defaultInput.Character.Fire.performed += e
      => Fire();
    defaultInput.Character.FireReleased.performed += e
      => FireRelease();
    defaultInput.Character.Reload.performed += e
      => Reload();

    defaultInput.Enable();

    startingPosition = transform.localPosition;
    targetWeaponPosition = startingPosition;
    reloadingPosition = new Vector3(
      startingPosition.x,
      startingPosition.y - 0.75f,
      startingPosition.z
    );
    recoilPosition = new Vector3(
      startingPosition.x,
      startingPosition.y,
      startingPosition.z - 0.5f
    );

    enemyLayer = LayerMask.NameToLayer("Enemy");

  }

  private void Start()
  {
    newWeaponRotation = transform.localRotation.eulerAngles;
    currentClip = weapon.clipSize;
    updateAmmoUI();
    }

  public void Initialize(CustomCharacterController cc)
  {
    characterController = cc;
    initialized = true;
  }

  private void Update()
  {
    if (!initialized || weapon == null) return;

    CalculatePosition();
    CalculateRotation();
    Firing();
  }

  private void CalculatePosition()
  {
    Vector3 position = transform.localPosition;

    position = Vector3.SmoothDamp(
      position,
      targetWeaponPosition,
      ref targetWeaponPositionVelocity,
      0.25f
    );

    transform.localPosition = position;
  }

  private void CalculateRotation()
  {
    targetWeaponRotation.y += settings.SwayAmount *
      (settings.SwayXInverted ? -1f : 1f) *
      characterController.inputView.x *
      Time.deltaTime;
    targetWeaponRotation.x += settings.SwayAmount *
      (settings.SwayYInverted ? 1f : -1f) *
      characterController.inputView.y *
      Time.deltaTime;

    targetWeaponRotation.x = Mathf.Clamp(
      targetWeaponRotation.x,
      -settings.SwayClampX,
      settings.SwayClampX
    );
    targetWeaponRotation.y = Mathf.Clamp(
      targetWeaponRotation.y,
      -settings.SwayClampY,
      settings.SwayClampY
    );
    targetWeaponRotation.z = targetWeaponRotation.y * 2f;

    targetWeaponRotation = Vector3.SmoothDamp(
      targetWeaponRotation,
      Vector3.zero,
      ref targetWeaponRotationVelocity,
      settings.SwayResetSmoothing
    );
    newWeaponRotation = Vector3.SmoothDamp(
      newWeaponRotation,
      targetWeaponRotation,
      ref newWeaponRotationVelocity,
      settings.SwaySmoothing
    );

    targetWeaponMovementRotation.z = settings.MovementSwayX *
      (settings.MovementSwayXInverted ? -1f : 1f) *
      characterController.inputMovement.x;
    targetWeaponMovementRotation.x = settings.MovementSwayY *
      (settings.MovementSwayYInverted ? -1f : 1f) *
      characterController.inputMovement.y;

    targetWeaponMovementRotation = Vector3.SmoothDamp(
      targetWeaponRotation,
      Vector3.zero,
      ref targetWeaponMovementRotationVelocity,
      settings.SwayResetSmoothing
    );
    newWeaponMovementRotation = Vector3.SmoothDamp(
      newWeaponMovementRotation,
      targetWeaponMovementRotation,
      ref newWeaponMovementRotationVelocity,
      settings.MovementSwaySmoothing
    );

    transform.localRotation = Quaternion.Euler(
      newWeaponRotation + newWeaponMovementRotation
    );
  }

  private void Firing()
  {
    if (!firing || weapon.automatic != Automatic.FULL) return;

    firingTick += Time.deltaTime;
    if (firingTick < weapon.fireSpeed) return;
    firingTick -= weapon.fireSpeed;

    FireWeapon();
  }

  private void Fire()
  {
    if (reloading) return;

    firing = true;
    firingTick = 0f;
    FireWeapon();
  }

  private void FireWeapon()
  {
    if (reloading) return;

    if (currentClip < 1)
    {
      if (weapon.automatic == Automatic.FULL)
        audioData.Pause();
      firing = false;
      Reload();
      return;
    }



    audioData.Play();


    targetWeaponPosition = recoilPosition;
    StartCoroutine(Recoiling());

    currentClip--;
    updateAmmoUI();


        // TODO: Call a raycaster here

       RaycastHit[] hits = Physics.SphereCastAll(
      bulletTransform.position,
      weapon.weaponSpray,
      transform.TransformDirection(Vector3.forward),
      weapon.shootingDistance,
      weapon.layerMask
    );

    if (hits.Length > 0)
    {
      for (int i = 0; i < hits.Length && i < weapon.bulletPenetrationCount; i++)
      {
        GameObject go = hits[i].collider.gameObject;

        if ((go.layer & enemyLayer) > 0)
        {
          DamageScript ds = go.GetComponent<DamageScript>();
          float hitPercent = Random.Range(0f, 0.999f);

          if (hitPercent < weapon.weaponAccuracy)
            ds.Damage(Random.Range(weapon.minDamage, weapon.maxDamage));
        }
      }
    }
  }

  private void Reload()
  {
    // TODO: Coroutine // play sound
    StartCoroutine(Reloading());
  }

  private IEnumerator Reloading()
  {

    audioReload.Play();
    reloading = true;

    targetWeaponPosition = reloadingPosition;
    yield return new WaitForSeconds(weapon.reloadTime);
    targetWeaponPosition = startingPosition;

    currentClip = weapon.clipSize;
    updateAmmoUI();
    reloading = false;
  }

  private IEnumerator Recoiling()
  {
    yield return new WaitForSeconds(0.15f);

    if (!reloading)
      targetWeaponPosition = startingPosition;
  }

  private void FireRelease()
  {
    firing = false;
    if (weapon.automatic == Automatic.FULL)
      audioData.Pause();
  }

  public bool IsFiring()
  {
    return firing;
  }

  public bool isReloading()
  {
    return reloading;
  }

  public void SetWeapon(WeaponScriptableObject weapon, Transform bulletTransform)
  {
    this.weapon = weapon;
    currentClip = weapon.clipSize;
    this.bulletTransform = bulletTransform;
    audioData.clip = weapon.audioClip;
    audioReload.clip = weapon.audioReload;
    }

    public void updateAmmoUI()
    {
        Parameters p = new Parameters();
        p.PutExtra("ammo", currentClip);
        p.PutExtra("max", weapon.clipSize);
        EventBroadcaster.Instance.PostEvent(EventNames.ON_UPDATE_AMMO, p);
    }

    public void updateWeaponUI()
    {
        Parameters p = new Parameters();
        p.PutExtra("type", weapon.name);
        EventBroadcaster.Instance.PostEvent(EventNames.ON_SWITCH_GUNS, p);
        updateAmmoUI();
    }
}

