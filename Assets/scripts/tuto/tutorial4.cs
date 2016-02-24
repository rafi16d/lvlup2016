using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tutorial4 : MonoBehaviour {

    public float maxdistance4 = 3;

    public GameObject player4;


    public string titletuto4;
    public string texttuto4;
    public Text tutovalue4;
    public Text tutovaluetitle4;
    public GameObject tutoscreen4;

    private GameObject thisobject;
    private float distance;
    private bool AlreadyView4 = false;

    // Use this for initialization
    void Start() {
        thisobject = this.gameObject;
        StartCoroutine(PauseCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!AlreadyView4) {

            distance = Vector3.Distance(player4.transform.position, thisobject.transform.position);

            if (distance < maxdistance4) {
                tutoscreen4.SetActive(true);
                Time.timeScale = 0f;
                tutovalue4.text = texttuto4;
                tutovaluetitle4.text = titletuto4;
            }
        }
    }

    IEnumerator PauseCoroutine() {
        while (true) {
            if (Input.GetButton("Submit")) {
                if (distance < maxdistance4) {
                    Time.timeScale = 1;
                    tutoscreen4.SetActive(false);
                    AlreadyView4 = true;
                }
            }
            yield return null;
        }
    }
}
