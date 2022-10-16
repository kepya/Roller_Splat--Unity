using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody ballRb;
    public float speed = 150;
    public bool isTraveling = false;
    private Vector3 travelDirection;
    private Vector3 nextCollisionDirection;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;
    public AudioClip hitClip;
    private AudioSource audioSource;

    private Color solveColor;

    public int minSwipeRecognition = 500; //en milimiter

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
        ballRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        if (isTraveling)
        {
            ballRb.velocity = speed * travelDirection * Time.deltaTime;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while(i< hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if (nextCollisionDirection != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionDirection) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionDirection = Vector3.zero;
            }
        }

        if (isTraveling)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();
                //if go up / down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5)
                {
                    //go up / down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                //if go up / down
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5)
                {
                    //go left / rigth
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }
            swipePosLastFrame = swipePosCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    void SetDestination(Vector3 direction)
    {
        travelDirection = direction;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionDirection = hit.point;
        }
        isTraveling = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name.Contains("WallPiece"))
        {
            if (hitClip)
            {
                //audioSource.PlayOneShot(hitClip);
            }
        }
    }
}
