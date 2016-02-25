using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainmenu : MonoBehaviour {

    public Toggle btntoggle;
    public string level;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            if (btntoggle.isOn) {
                SceneManager.LoadScene("introduction");
            } else {
                SceneManager.LoadScene(level);

            }

        }

        if (Input.GetButtonDown("Jump")) {
            if (btntoggle.isOn) {
                SceneManager.LoadScene("introduction");
            } else {
                SceneManager.LoadScene(level);

            }

        }
    }
}
