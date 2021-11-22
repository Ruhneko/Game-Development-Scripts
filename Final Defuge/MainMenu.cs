using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
  public int score;
  [SerializeField] private TextMeshProUGUI scoreText;
  [SerializeField] private Button unlockable;
  [SerializeField] private TextMeshProUGUI bodyPillow;
  public void Start()
  {
    unlockable.enabled = false;
    bodyPillow.text = "Pillow Mode (Unlock with 100 kills)";
    score = PlayerPrefs.GetInt("SCORE", -1);
    int pillowScore = PlayerPrefs.GetInt("PILLOW_SCORE", 0);
    int pillow = PlayerPrefs.GetInt("PILLOW", 0);

    if (score > -1)
    {
      scoreText.text = "Score: " + score;
      if (score >= 100 && pillow == 0)
      {
        scoreText.text += " Unlocked Body Pillow Mode";
        PlayerPrefs.SetInt("PILLOW", 1);
        bodyPillow.text = "Pillow Mode";
        unlockable.enabled = true;
        pillow = 1;
      }
      else if (pillow == 1)
      {
        bodyPillow.text = "Pillow Mode";
        unlockable.enabled = true;
        scoreText.text += "\nPillow: " + pillowScore;
      }
    }
  }

  public void GoInfinity()
  {
    PlayerPrefs.SetString("MODE", "Infinite");
    SceneManager.LoadScene("main_env", LoadSceneMode.Single);
  }

  public void GoBodyPillow()
  {
    PlayerPrefs.SetString("MODE", "BodyPillow");
    SceneManager.LoadScene("main_env", LoadSceneMode.Single);
  }

  public void Quit()
  {
    Application.Quit();
  }

  public void ResetPlayerPrefs()
  {
    PlayerPrefs.DeleteKey("SCORE");
    PlayerPrefs.DeleteKey("PILLOW_SCORE");
    PlayerPrefs.DeleteKey("PILLOW");
    SceneManager.LoadScene("main_menu", LoadSceneMode.Single);
  }
}