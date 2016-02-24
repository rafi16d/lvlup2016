using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class System32 : MonoBehaviour {
    [Header("Screen")]
    public GameObject rebootScreen;
    public GameObject rebootText;
    public GameObject userinterface;

    public bool isBlueScreen = false;
    public GameObject blueScreen;

    public string nextlvl;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        if (Input.anyKeyDown) {
            if (isBlueScreen) {
                if (Input.GetKey("escape") || Input.GetButton("Fire3")) {
                    SceneManager.LoadScene("MenuPrincipal");
                }
                //Load next lvl
                if (isBlueScreen)
                    SceneManager.LoadScene(nextlvl);
            }
        }

    }


    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player") {
            userinterface.SetActive(false);
            Core.resetState();
            GameObject.Find("_Manager").SendMessage("stopMusic");
            GameObject.Find("Player").SendMessage("stopSound");
            GameObject.Find("Player").SendMessage("sleepPlayer", Mathf.Infinity);
            //GameObject.Find("_Manager").SendMessage("upgradeMusic");
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();

            StartCoroutine(WaitForReboot());

        }

    }

    IEnumerator WaitForReboot() {
        rebootScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        rebootText.SetActive(true);
        StartCoroutine(WaitForBlue());
    }

    IEnumerator WaitForBlue() {
        yield return new WaitForSeconds(1.5f);
        rebootScreen.SetActive(false);
        rebootText.SetActive(false);

        blueScreen.SetActive(true);
        isBlueScreen = true;
    }
}
