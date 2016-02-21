using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Btn : MonoBehaviour {

    public void NextLevelButton(string levelName) {
        SceneManager.LoadScene(levelName);
    }

}
