using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

    public GameObject img1;
    public GameObject img2;
    public GameObject img3;
    public GameObject img4;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            changeImg();
        }

        if (Input.GetButtonDown("Jump")) {
            changeImg();
        }
    }
    void changeImg() {
        if (img1.activeInHierarchy) {
            img1.SetActive(false);
            img2.SetActive(true);
        } else if (img2.activeInHierarchy) {
            img2.SetActive(false);
            img3.SetActive(true);
        } else if (img3.activeInHierarchy) {
            img3.SetActive(false);
            img4.SetActive(true);
        } else if (img4.activeSelf) {
            //img4.SetActive(false);
            SceneManager.LoadScene("lvl0");
        }
    }

}
