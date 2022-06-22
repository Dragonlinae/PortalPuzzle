using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRay : MonoBehaviour
{

    public Texture[] textures;
    private LineRenderer lineRenderer;
    private int animationStep;
    public float fps;
    private float animationTime;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled)
        {
            animationTime += Time.deltaTime;
            if (animationTime > 1 / fps)
            {
                animationTime = 0;
                animationStep++;
                if (animationStep > textures.Length - 1)
                {
                    lineRenderer.enabled = false;
                    animationStep = 0;
                    animationTime = 0;
                }
                lineRenderer.material.mainTexture = textures[animationStep];
            }
        }

    }
}
