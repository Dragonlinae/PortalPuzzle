using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public Animator anim;
    public Transform dirIndicator;

    public Transform mapParent;
    public GameObject[] tiles;

    public LayerMask barrier;
    public LayerMask map;
    public LayerMask portal;

    public AudioSource music;

    public Array2DEditor.Array2DInt currentMapArray;

    public Transform bluePortal;
    public Transform purplePortal;

    public Transform bluePortalMask;
    public Transform purplePortalMask;

    public Transform shadowMC;
    public Transform shadowMovePoint;


    private Vector3 moveDirection = Vector3.up;
    private Transform currPortal;
    private Transform currPortalMask;
    private bool moved = false;
    private bool portalActive = false;


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        currPortal = bluePortal;
        currPortalMask = bluePortalMask;

        // Clear map
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        // Reset portal visibility
        bluePortal.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        purplePortal.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        portalActive = false;

        // Reset player position
        movePoint.position = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        drawTiles();
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

        if (Input.anyKeyDown)
        {
            // If we are not at the movePoint and any arrow key pressed, teleport to movePoint
            if (transform.position != movePoint.position)
            {
                transform.position = movePoint.position;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveDirection = Vector3.up;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveDirection = Vector3.down;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDirection = Vector3.left;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDirection = Vector3.right;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                // Detect nearest wall above us and put blue portal at that position of hit object and face player
                RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 10f, barrier);
                if (hit.collider != null)
                {
                    currPortal.position = hit.collider.transform.position;
                    currPortalMask.position = hit.collider.transform.position;
                    currPortal.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, hit.normal));
                    currPortalMask.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, hit.normal));
                    currPortal.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    // Update currPortal
                    if (currPortal == bluePortal)
                    {
                        currPortal = purplePortal;
                        currPortalMask = purplePortalMask;
                        // Change dirIndicator child object color to purple
                        dirIndicator.GetChild(0).GetComponent<SpriteRenderer>().color = Color.magenta;
                    }
                    else
                    {
                        currPortal = bluePortal;
                        currPortalMask = bluePortalMask;
                        // Change dirIndicator child object color to blue
                        dirIndicator.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
                    }

                    // Check if blue portal and purple portal overlap
                    if (Vector2.Distance(bluePortal.position, purplePortal.position) < 0.5f)
                    {
                        // Disable current portal
                        currPortal.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
                // Check if both portals are active
                if (bluePortal.gameObject.GetComponent<SpriteRenderer>().enabled && purplePortal.gameObject.GetComponent<SpriteRenderer>().enabled)
                {
                    portalActive = true;
                }
                else
                {
                    portalActive = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Start();
            }
        }

        if (moved)
        {
            // If player moves into a portal, teleport to other portal, else move player as long as move is valid
            Collider2D hitPortal = Physics2D.OverlapCircle(movePoint.position + moveDirection, 0.1f, portal);
            if (hitPortal && portalActive)
            {
                if (hitPortal.gameObject == bluePortal.gameObject)
                {
                    // Set up shadow movement
                    shadowMC.SetPositionAndRotation(transform.position, transform.rotation);
                    shadowMovePoint.position = bluePortal.position;

                    transform.position = purplePortal.position;
                    movePoint.position = purplePortal.position + purplePortal.rotation * Vector3.up;
                    // get difference in rotation of blue and purple portal and rotate player accordingly
                    transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + purplePortal.eulerAngles.z - bluePortal.eulerAngles.z + 180);

                }
                else
                {
                    // Set up shadow movement
                    shadowMC.SetPositionAndRotation(transform.position, transform.rotation);
                    shadowMovePoint.position = purplePortal.position;

                    transform.position = bluePortal.position;
                    movePoint.position = bluePortal.position + bluePortal.rotation * Vector3.up;
                    // get difference in rotation of blue and purple portal and rotate player accordingly
                    transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + bluePortal.eulerAngles.z - purplePortal.eulerAngles.z + 180);
                }
            }
            else if (!Physics2D.OverlapCircle(movePoint.position + moveDirection, 0.1f, barrier))
            {
                movePoint.position += moveDirection;
            }

            // Rotate dirIndicator based on moveDirection
            dirIndicator.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, moveDirection));


            moved = false;

            drawTiles();
        }
    }

    void drawTiles()
    {
        foreach (Vector3 direction in new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.zero })
        {
            if (!Physics2D.OverlapCircle(movePoint.position + direction, 0.1f, map))
            {
                Instantiate(tiles[currentMapArray.GetCell(5 + (int)(movePoint.position.x + direction.x), 5 - (int)(movePoint.position.y + direction.y))], movePoint.position + direction, Quaternion.identity, mapParent);
            }
        }
    }
}