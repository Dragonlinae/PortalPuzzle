using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRay : MonoBehaviour
{

    public Texture[] textures;
    private LineRenderer lr;
    private ParticleSystem ps;
    private int animationStep;
    public float fps;
    private float animationTime;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lr.enabled)
        {
            animationTime += Time.deltaTime;
            if (animationTime > 1 / fps)
            {
                animationTime = 0;
                animationStep++;
                if (animationStep > textures.Length - 1)
                {
                    lr.enabled = false;
                    ps.Stop();
                    animationStep = 0;
                    animationTime = 0;
                }
                lr.material.mainTexture = textures[animationStep];
            }
        }
    }

    public void fire(Vector3 start, Vector3 end, Vector3 dir, Color color)
    {
        // Draw the ray
        lr.SetPosition(0, start + Vector3.back);
        lr.SetPosition(1, end + Vector3.back);
        // change lr sprite color
        lr.startColor = color;
        lr.endColor = color;
        var main = ps.main;
        var shape = ps.shape;
        var emission = ps.emission;
        main.startColor = color;
        shape.radius = (Vector3.Distance(start, end) / 2f);
        shape.rotation = new Vector3(0, 0, Vector3.Angle(Vector3.right, dir));
        shape.position = (end + start) / 2f;
        emission.rateOverTime = shape.radius * 300f;
        lr.enabled = true;
        ps.Play();
        animationStep = 0;
        animationTime = 0;

    }
}
