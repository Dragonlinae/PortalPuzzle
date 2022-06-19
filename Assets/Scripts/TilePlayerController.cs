using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool inputFrame = false;
    public Transform movePoint;
    public Animator anim;

    public Transform mapParent;
    public GameObject[] tiles;

    public LayerMask barrier;
    public LayerMask map;

    public AudioSource music;

    public Array2DEditor.Array2DInt currentMapArray;


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (transform.position != movePoint.position)
        {
            anim.SetBool("moving", true);
        }
        else
        {
            anim.SetBool("moving", false);
        }
        // If we are not at the movePoint and any arrow key pressed, teleport to movePoint
        if (transform.position != movePoint.position && Input.anyKeyDown)
        {
            transform.position = movePoint.position;
        }

        // If direction key is pressed, move in that direction if it does not overlap with a barrier
        if (0 < music.time % 1 && music.time % 1 < 1)
        {
            if (true)
            {
                if (Input.anyKeyDown)
                {
                    inputFrame = false;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + Vector3.up, 0.1f, barrier))
                    {
                        movePoint.position += Vector3.up;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + Vector3.down, 0.1f, barrier))
                    {
                        movePoint.position += Vector3.down;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + Vector3.left, 0.1f, barrier))
                    {
                        movePoint.position += Vector3.left;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + Vector3.right, 0.1f, barrier))
                    {
                        movePoint.position += Vector3.right;
                    }
                }
            }
        }
        else
        {
            inputFrame = true;
        }

        foreach (Vector3 direction in new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right })
        {
            if (!Physics2D.OverlapCircle(movePoint.position + direction, 0.1f, map))
            {
                Instantiate(tiles[currentMapArray.GetCell(5 + (int)(movePoint.position.x + direction.x), 5 - (int)(movePoint.position.y + direction.y))], movePoint.position + direction, Quaternion.identity, mapParent);
            }
        }
    }
}