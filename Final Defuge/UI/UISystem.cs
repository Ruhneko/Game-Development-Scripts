using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text kills;
    [SerializeField] private Text health;
    [SerializeField] private Text wave_announcer;
    [SerializeField] private Text ammo;
    [SerializeField] private Image gunType;




    private int kills_count, health_count, ammo_count, wave_count, zombies;
    private float wave_time;
    private Sprite pistol, shotgun, smg, currentSprite;
  

    void Awake()
    {
        EventBroadcaster.Instance.AddObserver(
            EventNames.ON_UPDATE_KILSS,
            UpdateKills
        );

        EventBroadcaster.Instance.AddObserver(
            EventNames.ON_UPDATE_HEALTH,
            UpdateHealth
        );

        EventBroadcaster.Instance.AddObserver(
            EventNames.ON_UPDATE_NEXT_WAVE,
            UpdateWave
        );

        EventBroadcaster.Instance.AddObserver(
            EventNames.ON_UPDATE_AMMO,
            UpdateAmmo
        );

        EventBroadcaster.Instance.AddObserver(
            EventNames.ON_SWITCH_GUNS,
            UpdateGun
        );

        EventBroadcaster.Instance.AddObserver(
        EventNames.ON_REMAINING_ZOMBIES,
            UpdateZombieCount
        );

        EventBroadcaster.Instance.AddObserver(
        EventNames.ON_UPDATE_WAVE_COUNT,
            UpdateWaveCount
        );

        pistol = Resources.Load<Sprite>("UI/pistol");
        shotgun = Resources.Load<Sprite>("UI/shotgun");
        smg = Resources.Load<Sprite>("UI/smg");
    }

    void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveObserver(
            EventNames.ON_UPDATE_KILSS
        );

        EventBroadcaster.Instance.RemoveObserver(
            EventNames.ON_UPDATE_HEALTH
        );

        EventBroadcaster.Instance.RemoveObserver(
             EventNames.ON_UPDATE_NEXT_WAVE
        );

        EventBroadcaster.Instance.RemoveObserver(
            EventNames.ON_UPDATE_AMMO
        );

        EventBroadcaster.Instance.RemoveObserver(
            EventNames.ON_SWITCH_GUNS
        );

        EventBroadcaster.Instance.RemoveObserver(
            EventNames.ON_UPDATE_WAVE_COUNT
        );

        EventBroadcaster.Instance.RemoveObserver(
            EventNames.ON_REMAINING_ZOMBIES
        );
    }

    void init()
    {
        kills_count = 0;
        kills.text = kills_count + "";
        wave_time = 0;
        health_count = 20;
        zombies = 0;
        health.text = health_count + "/20";
        gunType.sprite = pistol;
    }


    // Start is called before the first frame update
    void Start()
    {
        gameOverUI.SetActive(false);
        mainUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateKills()
    {
        kills_count++;
        kills.text = kills_count + "";
    }

    void UpdateHealth(Parameters param)
    {
        health_count = param.GetIntExtra("health", 0);
        health.text = health_count + "/20";

        if(health_count <= 0)
        {
            mainUI.SetActive(false);
            gameOverUI.SetActive(true);
        }
    }

    void UpdateWave(Parameters param)
    {

       wave_time = param.GetFloatExtra("wave_count", 1f);
       wave_announcer.text = wave_time + "s\nWave: " + (wave_count+1);
       StartCoroutine(updateTimer());

    }

    IEnumerator updateTimer()
    {
        yield return new WaitForSeconds(1);
        wave_time--;
       
        if (wave_time > 0)
        {
            wave_announcer.text = wave_time + "s\nWave: " + (wave_count+1);
            StartCoroutine(updateTimer());
        }             
    }

    void UpdateAmmo(Parameters param)
    {
        int remaining = param.GetIntExtra("ammo", 0);
        int max = param.GetIntExtra("max", 0);
        ammo.text = remaining + "/" + max;
        Debug.Log("SASAS");
    }

    void UpdateGun(Parameters param)
    {
        string gun = param.GetStringExtra("type", "");
        if(gun == "pistol")
        {
            gunType.sprite = pistol;
        }
        else if (gun == "shotgun")
        {
            gunType.sprite = shotgun;
        }
        else if (gun == "smg")
        {
            gunType.sprite = smg;
        }
    }


    void UpdateWaveCount(Parameters param)
    {
        wave_count = param.GetIntExtra("current_wave", 0);
    }

    void UpdateZombieCount(Parameters param)
    {
        zombies = param.GetIntExtra("zombies_left", 0);
        wave_announcer.text = zombies + " zombies\nWave: " + wave_count;
    }
}
