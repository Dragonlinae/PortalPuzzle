using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public Animator anim;
    public Transform dirIndicator;

    public Transform mapParent;
    public GameObject[] tiles;
    public GameObject startTile;
    public GameObject exitTile;

    public LayerMask barrier;
    public LayerMask canPortalSurface;
    public LayerMask map;
    public LayerMask portal;
    public LayerMask exitLayerMask;

    public AudioSource music;

    public Array2DEditor.Array2DInt[] allMapArrays;
    public Array2DEditor.Array2DInt currentMapArray;
    public DialogueTrigger[] allDialogues;
    public DialogueManager dialogueManager;

    public Transform bluePortal;
    public Transform purplePortal;

    public Transform bluePortalMask;
    public Transform purplePortalMask;

    public Transform shadowMC;
    public Transform shadowMovePoint;
    public int levelNum = 0;


    private Vector3 moveDirection;
    private Transform currPortal;
    private Transform currPortalMask;
    private bool moved = false;
    private bool portalActive = false;


    // Start is called before the first frame update
    void Start()
    {
        ResetStage();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueManager.isTalking)
        {
            if (Input.anyKeyDown)
            {
                dialogueManager.DisplayNextSentence();
            }
            return;
        }

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
            else if (Input.GetKeyDown(KeyCode.Space)) // Fire Portal
            {
                // Detect nearest wall above us and put blue portal at that position of hit object and face player
                RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 20f, canPortalSurface);
                if (hit)
                {
                    LineRenderer lr = GetComponentInChildren<LineRenderer>();
                    lr.SetPosition(0, transform.position + Vector3.back);
                    lr.SetPosition(1, (Vector3)hit.point + Vector3.back);
                    // change lr sprite color
                    lr.startColor = Color.white;
                    lr.endColor = currPortal.GetComponent<SpriteRenderer>().color;
                    lr.GetComponent<LineRenderer>().enabled = true;
                    currPortal.position = hit.transform.position + Vector3.back;
                    currPortalMask.position = hit.transform.position;
                    currPortal.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, hit.normal));
                    currPortalMask.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, hit.normal));
                    currPortal.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    currPortal.gameObject.GetComponent<ParticleSystem>().Play();

                    // Reveal block in front of portal
                    if (!Physics2D.OverlapCircle(currPortal.position + (Vector3)hit.normal, 0.1f, map))
                        Instantiate(tiles[currentMapArray.GetCell(currentMapArray.GridSize.x / 2 + (int)(currPortal.position.x + hit.normal.x), currentMapArray.GridSize.y / 2 - (int)(currPortal.position.y + hit.normal.y))], currPortal.position + (Vector3)hit.normal, Quaternion.identity, mapParent);

                    // Update currPortal
                    if (currPortal == bluePortal)
                    {
                        currPortal = purplePortal;
                        currPortalMask = purplePortalMask;
                        // Change dirIndicator child object color to purple
                        dirIndicator.GetChild(0).GetComponent<SpriteRenderer>().color = purplePortal.GetComponent<SpriteRenderer>().color;
                    }
                    else
                    {
                        currPortal = bluePortal;
                        currPortalMask = bluePortalMask;
                        // Change dirIndicator child object color to blue
                        dirIndicator.GetChild(0).GetComponent<SpriteRenderer>().color = bluePortal.GetComponent<SpriteRenderer>().color;
                    }

                    // Check if blue portal and purple portal overlap
                    if (Vector2.Distance(bluePortal.position, purplePortal.position) < 0.5f && Quaternion.Angle(bluePortal.rotation, purplePortal.rotation) < 10)
                    {
                        // Disable current portal
                        currPortal.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        currPortal.gameObject.GetComponent<ParticleSystem>().Stop();
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
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
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
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Start();
            }
        }

        if (moved)
        {
            // If player moves into a portal, check if move is valid and teleport to other portal, else move player as long as move is valid
            Collider2D[] allHitPortals = Physics2D.OverlapCircleAll(movePoint.position + moveDirection, 0.1f, portal);
            foreach (Collider2D hitPortal in allHitPortals)
            {
                if (hitPortal && portalActive && hitPortal.gameObject.transform.rotation == Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, moveDirection)) && !Physics2D.OverlapCircle(purplePortal.position + purplePortal.rotation * Vector3.up, 0.1f, barrier) && !Physics2D.OverlapCircle(bluePortal.position + bluePortal.rotation * Vector3.up, 0.1f, barrier))
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
            }
            if (allHitPortals.Length == 0 && !Physics2D.OverlapCircle(movePoint.position + moveDirection, 0.1f, barrier))
            {
                movePoint.position += moveDirection;
            }

            // Rotate dirIndicator based on moveDirection
            dirIndicator.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, moveDirection));

            // Check if player is at the end of the level
            Collider2D finishLevel = Physics2D.OverlapCircle(movePoint.position, 0.1f, exitLayerMask);
            if (finishLevel)
            {
                NextLevel();
            }

            moved = false;

            DrawTiles();
        }
    }

    void ResetStage()
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

        // Set map
        currentMapArray = allMapArrays[levelNum];

        // Find position of start and exit tile in currentMapArray
        Vector3 startPos = Vector3.zero;
        for (int i = 0; i < currentMapArray.GridSize.x; i++)
        {
            for (int j = 0; j < currentMapArray.GridSize.y; j++)
            {
                if (currentMapArray.GetCell(i, j) == -1)
                {
                    startPos = new Vector3(i - currentMapArray.GridSize.x / 2, currentMapArray.GridSize.y / 2 - j, 0);
                    Instantiate(startTile, startPos, Quaternion.identity, mapParent);
                }
                else if (currentMapArray.GetCell(i, j) == -2)
                {
                    Vector3 exitPos = new Vector3(i - currentMapArray.GridSize.x / 2, currentMapArray.GridSize.y / 2 - j, 0);
                    Instantiate(exitTile, exitPos, Quaternion.identity, mapParent);
                }
            }
        }

        // Reset player position
        movePoint.position = startPos;
        transform.position = startPos;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        moveDirection = Vector3.up;
        dirIndicator.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, moveDirection));

        DrawTiles();

        // Run Dialogue
        if (allDialogues[levelNum] != null)
        {
            allDialogues[levelNum].TriggerDialogue();
        }
    }

    void DrawTiles()
    {
        foreach (Vector3 direction in new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.zero })
        {
            if (!Physics2D.OverlapCircle(movePoint.position + direction, 0.1f, map))
            {
                Instantiate(tiles[currentMapArray.GetCell(currentMapArray.GridSize.x / 2 + (int)(movePoint.position.x + direction.x), currentMapArray.GridSize.y / 2 - (int)(movePoint.position.y + direction.y))], movePoint.position + direction, Quaternion.identity, mapParent);
            }
        }
    }

    void NextLevel()
    {
        if (levelNum < allMapArrays.Length - 1)
        {
            levelNum++;
            Start();
        }
        else
        {
            // TODO: End game
        }
    }
}