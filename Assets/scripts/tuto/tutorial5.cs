using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tutorial5 : MonoBehaviour {

    public float maxdistance5 = 3;

    public GameObject player5;


    public string titletuto5;
    public string texttuto5;
    public Text tutovalue5;
    public Text tutovaluetitle5;
    public GameObject tutoscreen5;

    private GameObject thisobject;
    private float distance;
    private bool AlreadyView5 = false;

    // Use this for initialization
    void Start() {
        thisobject = this.gameObject;
        StartCoroutine(PauseCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!AlreadyView5) {

            distance = Vector3.Distance(player5.transform.position, thisobject.transform.position);

            if (distance < maxdistance5) {
                tutoscreen5.SetActive(true);
                Time.timeScale = 0f;
                tutovalue5.text = texttuto5;
                tutovaluetitle5.text = titletuto5;
            }
        }
    }

    IEnumerator PauseCoroutine() {
        while (true) {
            if (Input.GetButton("Submit")) {
                if (distance < maxdistance5) {
                    Time.timeScale = 1;
                    tutoscreen5.SetActive(false);
                    AlreadyView5 = true;
                }
            }
            yield return null;
        }
    }
}
