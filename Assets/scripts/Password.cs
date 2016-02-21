using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Password : MonoBehaviour {

    public int id;
    public GameObject popup;
    public GameObject player;
    public Button btnClose;
    public Text password;
    public Text idValue;
    private Rigidbody2D collRd2d;

    private Vector3 _startPosition;

    [Header("User Interface")]
    public Text core1_pwd;
    public Text core2_pwd;
    public Text core3_pwd;

    private bool isdestroy = false;

    // Use this for initialization
    void Start() {
        _startPosition = transform.position;

    }
    // Update is called once per frame
    void Update() {
    }

    // fixedUpdate is called regular
    void FixedUpdate() {
        if (!isdestroy) {
            transform.position = _startPosition + new Vector3(0.0f, Mathf.Sin(Time.time) * (Time.deltaTime + 0.03f), 0.0f);
        }

        btnClose.onClick.AddListener(cancel);//adds a listener for when you click the button

        if (Input.GetButton("Submit")) {
            cancel();
        }
    }

    void cancel() {
        popup.SetActive(false);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        PlayerMovement playmovementscript = player.GetComponent<Rigidbody2D>().gameObject.GetComponent<PlayerMovement>();

        playmovementscript.setGameObjectHashCodeAsDestroy(this.gameObject.GetHashCode());


    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player") {

            // Show PopUp
            popup.SetActive(true);
            password.text = Core.getPassword(id);
            idValue.text = "#" + id;
            btnClose.onClick.AddListener(cancel);//adds a listener for when you click the button

            if (Input.GetButton("Submit")) {
                cancel();
            }
            if (id == 1) {
                core1_pwd.text = Core.getPassword(id);
            } else if (id == 2) {
                core2_pwd.text = Core.getPassword(id);
            } else if (id == 3) {
                core3_pwd.text = Core.getPassword(id);
            }

            collRd2d = coll.gameObject.GetComponent<Rigidbody2D>();
            collRd2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            Destroy(this.gameObject);
            isdestroy = true;
        }

    }
}