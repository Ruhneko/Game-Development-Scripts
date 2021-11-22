using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static Mode mode;

  [SerializeField] private EnemyController enemyPrefab;
  [SerializeField] private EnemyController enemyDurablePrefab;
  [SerializeField] private Home homebase;
  [SerializeField] private float timeBetweenWaves = 10f;
  [SerializeField] private List<Transform> spawnpoints;
  [SerializeField] private AudioSource audioPlayer;

  public Transform pillowParent;
  public GameObject bodyPillow;
  public Vector3 bodyPillowOrginalPosition;
    

  private int waveNumber = 0;
  private float breaktime = 10f;
  private int zombieCount = 0;
  private float tick = 0f;
  private int spawnCount = 2;
  private int spawnIncrement = 2;
  private float spawnSecond = 0.3f;
  private float spawnThird = 0.1f;
  private float spawnEnemyDurable = 0.2f;
  private int health = 20;
  private int score = 0;

  void Start()
  {
    string modeStr = PlayerPrefs.GetString("MODE", "Infinite");

    switch (modeStr)
    {
      case "Infinite":
        mode = Mode.INFINITE;
        bodyPillow.SetActive(false);
        break;
      case "BodyPillow":
        mode = Mode.BODY_PILLOW;
        bodyPillowOrginalPosition = bodyPillow.transform.position;
        break;
    }
    homebase.setHomeGameManager(this);
    UpdateHealth();
    StartCoroutine(SpawnWave());
  }


  IEnumerator SpawnWave()
  {
    zombieCount = spawnCount;
    waveNumber++;
    InitWaveUI();

    // TODO: Third
    // Parameters p = new Parameters();
    // p.PutExtra("wave_count", waveNumber);
    // EventBroadcaster.Instance.PostEvent(EventNames.ON_UPDATE_NEXT_WAVE, p);

    int random = Random.Range(0, spawnpoints.Count);
    int i = 0;
    while (i < spawnCount)
    {
      SpawnEnemy(spawnpoints[random]);
      i++;

      if (i < spawnCount && Random.Range(0f, 1f) < spawnSecond)
      {
        random = (random + 1) % spawnpoints.Count;
        SpawnEnemy(spawnpoints[random]);
        i++;
        if (i < spawnCount && Random.Range(0f, 1f) < spawnThird)
        {
          random = (random + 1) % spawnpoints.Count;
          SpawnEnemy(spawnpoints[random]);
          i++;
        }
      }

      yield return new WaitForSeconds(Random.Range(0.1f, 1f));
    }
  }

  private void SpawnEnemy(Transform spawnpoint)
  {
    EnemyController ec = Random.Range(0f, 1f) < spawnEnemyDurable ? enemyDurablePrefab : enemyPrefab;
    EnemyController enemy = Instantiate<EnemyController>(ec, spawnpoint.position, Quaternion.identity);
    enemy.SetGameManager(this);
  }

  IEnumerator Breaktime()
  {
    Parameters p = new Parameters();
    p.PutExtra("wave_count", breaktime);
    EventBroadcaster.Instance.PostEvent(EventNames.ON_UPDATE_NEXT_WAVE, p);

    yield return new WaitForSeconds(breaktime);

    if (waveNumber % 5 == 0)
    {
      spawnIncrement += 2;
    }
    spawnCount = spawnCount + spawnIncrement;
    StartCoroutine(SpawnWave());
  }

  public void RemoveEnemy()
  {
    zombieCount--;
    score++;
    UpdateRemainingUI();
    EventBroadcaster.Instance.PostEvent(EventNames.ON_UPDATE_KILSS);
    if (zombieCount == 0)
    {
      StartCoroutine(Breaktime());
    }
  }

  public void DecreaseEnemy()
  {
    zombieCount--;
    UpdateRemainingUI();
    if (zombieCount == 0)
    {
      StartCoroutine(Breaktime());
    }
  }

  public void loseHealth()
  {
    health--;
    UpdateHealth();
    if (health == 0) //temp
    {
     StopAllCoroutines();
     StartCoroutine(GameOver());
    }
  }

  public IEnumerator GameOver()
  {
    audioPlayer.clip = Resources.Load<AudioClip>("GameOver");
    audioPlayer.Play();
    yield return new WaitForSeconds(7);

        if (mode == Mode.INFINITE)
        {
            int lastscore = PlayerPrefs.GetInt("SCORE");
            if (lastscore < score)
            {
                PlayerPrefs.SetInt("SCORE", score);
            }
        }
        else if (mode == Mode.BODY_PILLOW)
        {
            int lastscore = PlayerPrefs.GetInt("PILLOW_SCORE");
            if (lastscore < score)
            {
                PlayerPrefs.SetInt("PILLOW_SCORE", score);
            }
        }
    
    SceneManager.LoadScene("main_menu", LoadSceneMode.Single);
  }


  private void InitWaveUI()
  {
    UpdateWaveCountUI();
    UpdateRemainingUI();
  }

  private void UpdateWaveCountUI()
  {
      Parameters p = new Parameters();
      p.PutExtra("current_wave", waveNumber);
      EventBroadcaster.Instance.PostEvent(EventNames.ON_UPDATE_WAVE_COUNT, p);
  }

    private void UpdateRemainingUI()
  {
      Parameters p = new Parameters();
      p.PutExtra("zombies_left", zombieCount);
      EventBroadcaster.Instance.PostEvent(EventNames.ON_REMAINING_ZOMBIES, p);
  }

    private void UpdateHealth()
    {
        Parameters p = new Parameters();
        p.PutExtra("health", health);
        EventBroadcaster.Instance.PostEvent(EventNames.ON_UPDATE_HEALTH, p);
    }
}
