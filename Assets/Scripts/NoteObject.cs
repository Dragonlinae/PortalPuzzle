using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour {

    private float unitsPerSec;
    public bool canPress;
    public KeyCode keyToPress;
    public bool started = true;
    public char noteType;
    public bool isNext;
    public string activatorTag;

    // Start is called before the first frame update
    void Start() {
        unitsPerSec = GameManager.instance.unitsPerSec;
    }

    // Update is called once per frame
    void Update() {
        transform.localPosition -= new Vector3(0f, unitsPerSec * Time.deltaTime, 0f);

        if (Input.GetKeyDown(keyToPress)) {
            updateIsNext();
            if (isNext) {
                if (canPress) {
                    resetIsNext();
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                    if (Mathf.Abs(transform.localPosition.y) > 0.4) {
                        GameManager.instance.goodHit(transform.parent.gameObject);
                    } else if (Mathf.Abs(transform.localPosition.y) > 0.2) {
                        GameManager.instance.greatHit(transform.parent.gameObject);
                    } else {
                        GameManager.instance.perfectHit(transform.parent.gameObject);
                    }
                } else {
                    GameManager.instance.NoteMiss(transform.parent.gameObject);
                }
            }
        }
    }

    void updateIsNext() {
        if (noteType == 'L') {
            isNext = GameManager.instance.lcurrent;
        } else if (noteType == 'U') {
            isNext = GameManager.instance.ucurrent;
        } else if (noteType == 'D') {
            isNext = GameManager.instance.dcurrent;
        } else if (noteType == 'R') {
            isNext = GameManager.instance.rcurrent;
        }
    }
    void resetIsNext() {
        if (noteType == 'L') {
            GameManager.instance.lcurrent = false;
        } else if (noteType == 'U') {
            GameManager.instance.ucurrent = false;
        } else if (noteType == 'D') {
            GameManager.instance.dcurrent = false;
        } else if (noteType == 'R') {
            GameManager.instance.rcurrent = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == activatorTag) {
            canPress = true;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (gameObject.activeSelf) {
            if (other.tag == activatorTag) {
                canPress = false;

                GameManager.instance.NoteMiss(transform.parent.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
