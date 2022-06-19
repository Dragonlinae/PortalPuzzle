using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kirbPlayer : MonoBehaviour {
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;
    public Animator animator;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(leftKey)) {
            animator.SetTrigger("Left");
        } else if (Input.GetKey(upKey)) {
            animator.SetTrigger("Up");
        } else if (Input.GetKey(downKey)) {
            animator.SetTrigger("Down");
        } else if (Input.GetKey(rightKey)) {
            animator.SetTrigger("Right");
        }
    }
}
