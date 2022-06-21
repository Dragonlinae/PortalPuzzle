using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowTilePlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public Animator anim;
    public SpriteRenderer sprite;

    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Input.anyKeyDown)
        {
            // If we are not at the movePoint and any arrow key pressed, teleport to movePoint
            if (transform.position != movePoint.position)
            {
                transform.position = movePoint.position;
            }
        }

        if (transform.position != movePoint.position)
        {
            anim.SetBool("moving", true);
            // make sprite visible
            sprite.enabled = true;
        }
        else
        {
            anim.SetBool("moving", false);
            // make sprite not visible if at target
            sprite.enabled = false;
        }
    }
}