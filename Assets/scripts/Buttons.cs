using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour {
    private bool ISTUTO = false;

    public Toggle btntoggle;

    public void NextLevelButton(string levelName) {
        SceneManager.LoadScene(levelName);

        Time.timeScale = 1.0f; // Le temps reprend

    }

    public void ResumeButton() {
        GameObject.Find("PauseScreenGame").SetActive(false);
        Time.timeScale = 1.0f; // Le temps reprend
    }

    public void ExitButton() {
        Application.Quit();
        Time.timeScale = 1.0f; // Le temps reprend

    }

    public void FirstLevelButton(string level) {
        if (btntoggle.isOn) {
            SceneManager.LoadScene("intro");
        } else {
            SceneManager.LoadScene(level);

        }

        Time.timeScale = 1.0f; // Le temps reprend
    }


}
