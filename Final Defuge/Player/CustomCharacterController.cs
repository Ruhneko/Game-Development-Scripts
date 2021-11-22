using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;

public class CustomCharacterController : MonoBehaviour
{
  private CharacterController characterController;
  private DefaultInput defaultInput;
  [HideInInspector] public Vector2 inputMovement;
  [HideInInspector] public Vector2 inputView;

  private Vector3 newCameraRotation;
  private Vector3 newCharacterRotation;

  [Header("References")]
  [SerializeField] private Transform cameraHolder;
  [SerializeField] private Transform feetTransform;
  [SerializeField] private Transform bulletTransform;

  [Header("Settings")]
  [SerializeField] private PlayerSettingsModel playerSettings;
  [SerializeField] private float viewClampYMin = -70;
  [SerializeField] private float viewClampYMax = 80;
  [SerializeField] private LayerMask playerMask;

  [Header("Gravity")]
  [SerializeField] private float gravityAmount;
  [SerializeField] private float gravityMin;
  [SerializeField] private float playerGravity = -10f;

  [SerializeField] private float jumpingVelocity;

  [Header("Stance")]
  [SerializeField] private PlayerStance playerStance;
  [SerializeField] private float playerStanceSmoothing;
  [SerializeField] private CharacterStance playerStandStance;
  [SerializeField] private CharacterStance playerCrouchStance;
  [SerializeField] private CharacterStance playerProneStance;
  [SerializeField] private float stanceCheckErrorMargin = 0.05f;

  private float cameraHeight;
  private float cameraHeightVelocity;

  private Vector3 stanceCapsuleCenter;
  private Vector3 stanceCapsuleCenterVelocity;

  private float stanceCapsuleHeight;
  private float stanceCapsuleHeightVelocity;

  private bool sprinting;

  private Vector3 newMovementSpeed;
  private Vector3 newMovementSpeedVelocity;

  [Header("Weapon")]
  [SerializeField] private WeaponController weaponController;
  [SerializeField] private WeaponScriptableObject pistol;
  [SerializeField] private WeaponScriptableObject shotgun;
  [SerializeField] private WeaponScriptableObject smg;
  private GameObject weaponObj;
  private Dictionary<string, int> clips = new Dictionary<string, int>();

  private void Awake()
  {
    defaultInput = new DefaultInput();

    defaultInput.Character.Movement.performed += e
      => inputMovement = e.ReadValue<Vector2>();
    defaultInput.Character.View.performed += e
      => inputView = e.ReadValue<Vector2>();
    defaultInput.Character.Jump.performed += e
      => Jump();
    defaultInput.Character.Crouch.performed += e
      => Crouch();
    defaultInput.Character.Prone.performed += e
      => Prone();
    defaultInput.Character.Sprint.performed += e
      => ToggleSprint();
    defaultInput.Character.SprintReleased.performed += e
      => StopSprint();
    defaultInput.Character.ChangeWeaponPistol.performed += e
      => ChangeWeapon(pistol);
    defaultInput.Character.ChangeWeaponShotgun.performed += e
      => ChangeWeapon(shotgun);
    defaultInput.Character.ChangeWeaponSMG.performed += e
      => ChangeWeapon(smg);

    defaultInput.Enable();

    newCameraRotation = cameraHolder.localRotation.eulerAngles;
    newCharacterRotation = transform.localRotation.eulerAngles;

    characterController = GetComponent<CharacterController>();

    cameraHeight = cameraHolder.localPosition.y;

    weaponController.Initialize(this);
    clips["pistol"] = pistol.clipSize;
    clips["shotgun"] = shotgun.clipSize;
    clips["smg"] = smg.clipSize;
    ChangeWeapon(pistol);
  }

  private void Update()
  {
    CalculateView();
    CalculateMovement();
    CalculateJump();
    CalculateStance();
  }

  private void CalculateView()
  {
    newCharacterRotation.y += playerSettings.ViewXSensitivity *
      (playerSettings.ViewXInverted ? -1f : 1f) *
      inputView.x *
      Time.deltaTime;
    transform.localRotation = Quaternion.Euler(newCharacterRotation);

    newCameraRotation.x += playerSettings.ViewYSensitivity *
      (playerSettings.ViewYInverted ? 1f : -1f) *
      inputView.y *
      Time.deltaTime;
    newCameraRotation.x = Mathf.Clamp(
      newCameraRotation.x,
      viewClampYMin,
      viewClampYMax
    );

    cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
  }

  private void CalculateMovement()
  {
    if (inputMovement.y <= 0.1f)
    {
      sprinting = false;
    }

    float verticalSpeed = playerSettings.WalkingForwardSpeed;
    float horizontalSpeed = playerSettings.WalkingStrafeSpeed;

    if (sprinting)
    {
      verticalSpeed = playerSettings.RunningForwardSpeed;
      horizontalSpeed = playerSettings.RunningStrafeSpeed;
    }

    if (!characterController.isGrounded)
    {
      playerSettings.SpeedEffector = playerSettings.FallingSpeedEffector;
    }
    else if (playerStance == PlayerStance.Crouch)
    {
      playerSettings.SpeedEffector = playerSettings.CrouchSpeedEffector;
    }
    else if (playerStance == PlayerStance.Prone)
    {
      playerSettings.SpeedEffector = playerSettings.ProneSpeedEffector;
    }
    else
    {
      playerSettings.SpeedEffector = 1f;
    }

    // Effectors
    verticalSpeed *= playerSettings.SpeedEffector;
    horizontalSpeed *= playerSettings.SpeedEffector;

    newMovementSpeed = Vector3.SmoothDamp(
      newMovementSpeed,
      new Vector3(
        horizontalSpeed * inputMovement.x * Time.deltaTime,
        0,
        verticalSpeed * inputMovement.y * Time.deltaTime
      ),
      ref newMovementSpeedVelocity,
      characterController.isGrounded
        ? playerSettings.MovementSmoothing
        : playerSettings.FallingSmoothing
    );
    Vector3 movementSpeed = transform.TransformDirection(newMovementSpeed);

    movementSpeed.y = jumpingVelocity;

    characterController.Move(movementSpeed);
  }

  private void CalculateJump()
  {
    if (characterController.isGrounded)
    {
      jumpingVelocity = 0f;
    }
    else
    {
      jumpingVelocity += playerGravity * Time.deltaTime;
    }
  }

  private void CalculateStance()
  {
    CharacterStance currentStance = null;

    switch (playerStance)
    {
      case PlayerStance.Stand:
        currentStance = playerStandStance;
        break;
      case PlayerStance.Crouch:
        currentStance = playerCrouchStance;
        break;
      case PlayerStance.Prone:
        currentStance = playerProneStance;
        break;
    }

    cameraHeight = Mathf.SmoothDamp(
      cameraHolder.localPosition.y,
      currentStance.CameraHeight,
      ref cameraHeightVelocity,
      playerStanceSmoothing
    );
    cameraHolder.localPosition = new Vector3(
      cameraHolder.localPosition.x,
      cameraHeight,
      cameraHolder.localPosition.z
    );

    characterController.height = Mathf.SmoothDamp(
      characterController.height,
      currentStance.StanceCollider.height,
      ref stanceCapsuleHeightVelocity,
      playerStanceSmoothing
    );
    characterController.center = Vector3.SmoothDamp(
      characterController.center,
      currentStance.StanceCollider.center,
      ref stanceCapsuleCenterVelocity,
      playerStanceSmoothing
    );
  }

  private void Jump()
  {
    if (
      !characterController.isGrounded ||
      playerStance == PlayerStance.Prone
    ) return;

    if (playerStance == PlayerStance.Crouch)
    {
      if (StanceCheck(playerStandStance.StanceCollider.height))
      {
        return;
      }

      playerStance = PlayerStance.Stand;
      return;
    }

    jumpingVelocity = playerSettings.JumpingHeight;
  }

  private void Crouch()
  {
    if (playerStance == PlayerStance.Crouch)
    {
      if (StanceCheck(playerStandStance.StanceCollider.height))
      {
        return;
      }

      playerStance = PlayerStance.Stand;
      return;
    }

    if (StanceCheck(playerCrouchStance.StanceCollider.height))
    {
      return;
    }

    playerStance = PlayerStance.Crouch;
  }

  private void Prone()
  {
    playerStance = PlayerStance.Prone;
  }

  private bool StanceCheck(float stanceCheckHeight)
  {
    Vector3 start = new Vector3(
      feetTransform.position.x,
      feetTransform.position.y
        + characterController.radius
        + stanceCheckErrorMargin,
      feetTransform.position.z
    );
    Vector3 end = new Vector3(
      feetTransform.position.x,
      feetTransform.position.y
        - characterController.radius
        - stanceCheckErrorMargin
        + stanceCheckHeight,
      feetTransform.position.z
    );

    return Physics.CheckCapsule(
      start, end,
      characterController.radius,
      playerMask
    );
  }

  private void ToggleSprint()
  {
    if (inputMovement.y <= 0.2f)
    {
      sprinting = false;
      return;
    }

    sprinting = !sprinting;
  }

  private void StopSprint()
  {
    if (playerSettings.SprintingHold)
    {
      sprinting = false;
    }
  }

  private void ChangeWeapon(WeaponScriptableObject weapon)
  {
    if (weaponObj != null)
    {
      if (
        weaponController.isReloading() ||
        weaponController.IsFiring() ||
        weaponController.weapon.name.Equals(weapon.name)
      ) return;

      clips[weaponController.weapon.name] = weaponController.currentClip;
      Destroy(weaponObj);
    }

    weaponObj = Instantiate(weapon.weaponObj, weaponController.transform);
    weaponController.SetWeapon(
      weapon,
      bulletTransform
    );

    weaponController.currentClip = clips[weapon.name];
    weaponController.updateAmmoUI();
    weaponController.updateWeaponUI();
   }
}
