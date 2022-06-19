using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelector : MonoBehaviour
{

    public int direction = 0;
    public bool moving = false;
    public GameObject[] worldList = new GameObject[3];
    public int currentWorld = 0;
    public int nextWorld = 0;
    public float totalRotate = 83.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && moving == false) {
            moving = true;
            direction = 1;
            nextWorld = (currentWorld >= worldList.Length - 1) ? 0 : currentWorld + 1;
            worldList[nextWorld].SetActive(true);
            worldList[nextWorld].transform.rotation = Quaternion.Euler(0, 0, -83.3f);
            totalRotate = 83.3f;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && moving == false) {
            moving = true;
            direction = -1;
            nextWorld = (currentWorld <= 0) ? worldList.Length - 1 : currentWorld - 1;
            worldList[nextWorld].SetActive(true);
            worldList[nextWorld].transform.rotation = Quaternion.Euler(0, 0, 83.3f);
            totalRotate = 83.3f;
        }
        if (moving == true) {
            float rotateAmount = Time.deltaTime * 200;
            if (totalRotate - rotateAmount > 0) {
                totalRotate -= rotateAmount;
                worldList[currentWorld].transform.Rotate(0, 0, rotateAmount * direction);
                worldList[nextWorld].transform.Rotate(0, 0, rotateAmount * direction);
            } else {
                worldList[currentWorld].transform.Rotate(0, 0, totalRotate * direction);
                worldList[nextWorld].transform.Rotate(0, 0, totalRotate * direction);
                worldList[currentWorld].SetActive(false);
                currentWorld = nextWorld;
                moving = false;
            }

        }
    }

    public void backToMenu() {
        SceneManager.LoadScene("Menu");
    }
}
