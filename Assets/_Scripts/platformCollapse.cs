using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformCollapse : MonoBehaviour
{

    // Speed of vertical bob
    public float speed;

    // Amplitude of vertical bob
    public float width;


    // Starting position
    Vector3 home;
    // Extent of bob position
    Vector3 target;

    // Start is called before the first frame update

    GameObject player;


    // How long the player can stand on the platform before it starts to shrink
    public float fallDelay;

    public AudioClip rising;
    public AudioClip falling;

    AudioSource audio;

    float targetScaleX;

    void Start()
    {
        home = transform.position;
        target = new Vector3(home.x, home.y + width, home.z);

        player = null;



        targetScaleX = transform.localScale.x;

        audio = GetComponent<AudioSource>();



        StartCoroutine("moveDown");

    }

    // Shrinking coroutine, scales down the platform until it is extremely small (or is interrupted by the player getting off)
    IEnumerator Shrink()
    {
        yield return new WaitForSeconds(fallDelay);
        audio.PlayOneShot(falling);
        while(transform.localScale.x > 0.01f)
        {
            transform.localScale *= 0.99f;
            yield return new WaitForSeconds(0.01f);
        }
       
    }


    // Regrows the platform to its original size, unless interrupted by the player stepping on it again.
    IEnumerator ReGrow()
    {
        yield return new WaitForSeconds(1.5f);
        audio.PlayOneShot(rising);
        while (transform.transform.localScale.x < targetScaleX)
        {
            transform.localScale *= 1.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Upward bob
    IEnumerator moveUp()
    {
        while (transform.position.y < target.y)
        {
            if (player != null)
            {
                player.transform.Translate(new Vector3(0, speed, 0));
            }
            transform.Translate(new Vector3(0, speed, 0));
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("moveDown");
    }

    // Downward bob
    IEnumerator moveDown()
    {
        while (transform.position.y > home.y)
        {
            if (player != null)
            {
                player.transform.Translate(new Vector3(0, -speed, 0));
            }
            transform.Translate(new Vector3(0, -speed, 0));
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("moveUp");
    }

    // Detecting collision with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopCoroutine("ReGrow");
            player = collision.gameObject;

            StartCoroutine("Shrink");
        }
    }


    // Detecting when player has stepped off.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopCoroutine("Shrink");
            player = null;

            StartCoroutine("ReGrow");
        }
    }
}
