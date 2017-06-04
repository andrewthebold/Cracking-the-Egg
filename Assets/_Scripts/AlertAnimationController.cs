using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertAnimationController : MonoBehaviour
{
    // In this example we show how to invoke a coroutine and
    // continue executing the function in parallel.

    public float delay = 2.0f;

    private Animator anim;
    private AudioSource audioSrc;
    private IEnumerator coroutine;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();

        coroutine = Pulse();
        StartCoroutine(coroutine);
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
            anim.SetBool("Expand", true);
            audioSrc.Play();
            yield return new WaitForSeconds((float) (anim.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2.0));

            anim.SetBool("Idle", true);

            yield return new WaitForSeconds(delay);
        }
    }
}

/*{

    private Animator anim;

    private bool running = false;

    private IEnumerator coroutine;

    AnimationState expander;

    // Use this for initialization
    void Start () {
        Debug.Log("Test");

        anim = GetComponent<Animator>();

        expander = GetComponent<Animation>()["Alert_Expand"];

        expander.wrapMode = WrapMode.Once;

        coroutine = Pulse();

        StartCoroutine(coroutine);
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
           // running = true;
            anim.Play("Alert_Expand");
            Debug.Log("Testing.");

            yield return new WaitForSeconds(5f);

            running = false;
        }
    }
}*/
