using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tutorial2 : MonoBehaviour {

    public float maxdistance2 = 3;

    public GameObject player2;


    public string titletuto2;
    public string texttuto2;
    public Text tutovalue2;
    public Text tutovaluetitle2;
    public GameObject tutoscreen2;

    private GameObject thisobject;
    private float distance;
    private bool AlreadyView2 = false;

    // Use this for initialization
    void Start() {
        thisobject = this.gameObject;
        StartCoroutine(PauseCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!AlreadyView2) {

            distance = Vector3.Distance(player2.transform.position, thisobject.transform.position);

            if (distance < maxdistance2) {
                tutoscreen2.SetActive(true);
                Time.timeScale = 0f;
                tutovalue2.text = texttuto2;
                tutovaluetitle2.text = titletuto2;
            }
        }
    }

    IEnumerator PauseCoroutine() {
        while (true) {
            if (Input.GetButton("Submit")) {
                if (distance < maxdistance2) {
                    Time.timeScale = 1;
                    tutoscreen2.SetActive(false);
                    AlreadyView2 = true;
                }
            }
            yield return null;
        }
    }
}
