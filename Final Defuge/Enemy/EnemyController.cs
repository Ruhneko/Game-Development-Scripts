using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

  [SerializeField] private int health;
  private Vector3 startPosition;
  private bool alive = true;
  private Animator animator;
  private UnityEngine.AI.NavMeshAgent agent;
  private GameManager gameManager;
  private int pillowTime = 0;
  public Transform goal;

  [SerializeField] private List<Collider> colliders;

  private bool hasBodyPillow = false;
  [SerializeField] private Transform bodyPosition;

  private void Start()
  {
    animator = GetComponent<Animator>();
    agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    startPosition = transform.position;

    if (GameManager.mode == Mode.INFINITE)
    {
      if (agent != null)
      {
        agent.destination = new Vector3(
          goal.position.x + Random.Range(-0.5f, 0.5f),
          goal.position.y + Random.Range(-0.5f, 0.5f),
          goal.position.z + Random.Range(-0.5f, 0.5f)
        );
      }
    }
    else if (GameManager.mode == Mode.BODY_PILLOW)
    {
      if (agent != null)
      {
        agent.destination = gameManager.bodyPillow.transform.position;
      }
    }
  }

  private void Update()
  {
    if (
      GameManager.mode == Mode.BODY_PILLOW && hasBodyPillow
      )
    {
      pillowTime++;
      if (pillowTime == 60){
        Parameters p = new Parameters();
        p.PutExtra("health", 0);
        EventBroadcaster.Instance.PostEvent(EventNames.ON_UPDATE_HEALTH, p);
        gameManager.StartCoroutine(gameManager.GameOver());
      }
    }
  }

  public bool isAlive()
  {
    return alive;
  }

  public void Damage(int dmg)
  {
    health -= dmg;

    if (health < 0)
    {
      Die();
    }
  }

  private void Die()
  {
    foreach (Collider c in colliders)
    {
      Destroy(c);
    }

    if (animator)
    {
      animator.SetBool("dead", true);
    }

    if (hasBodyPillow)
    {
      // TODO: Drop the body pillow to this location
      gameManager.bodyPillow.transform.SetParent(gameManager.pillowParent);
      gameManager.bodyPillow.transform.position = gameManager.bodyPillowOrginalPosition;
    }

    if (isAlive())
    {
      gameManager.RemoveEnemy();
      alive = false;
      agent.isStopped = true;
      StartCoroutine(Disappear());
    }
  }

  IEnumerator Disappear()
  {
    yield return new WaitForSeconds(4.0f);
    Destroy(gameObject);
  }

  public void SetGameManager(GameManager gameManager)
  {
    this.gameManager = gameManager;
  }

  public void SetStartPosition(Vector3 startPosition)
  {
    this.startPosition = startPosition;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (GameManager.mode == Mode.INFINITE)
    {
      if (other.gameObject.layer == LayerMask.NameToLayer("CollideEnemy"))
      {
        if (alive)
        {
          Destroy(gameObject);
          gameManager.DecreaseEnemy();
          alive = false;
          agent.isStopped = true;
        }
      }
    }
    else if (GameManager.mode == Mode.BODY_PILLOW)
    {
      if (other.gameObject.layer == LayerMask.NameToLayer("Kaguya"))
      {
        hasBodyPillow = true;
        agent.destination = startPosition;
        gameManager.bodyPillow.transform.SetParent(transform);
        gameManager.bodyPillow.transform.position = bodyPosition.position;
      }
    }
  }
}
