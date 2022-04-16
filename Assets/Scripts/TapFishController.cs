using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TapFishController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera mainCamera;
    public GameObject ocean;

    [Tooltip("An object that will not affect the scene but act as a danger to make the flock escape")]
    public GameObject dangerSimulationPrefab;

    // Update is called once per frame
    public LayerMask fishLayer;
    public LayerMask sharkLayer;

    public float rumbleTime;
    public float rumbleDist;

    public AudioSource normalSource;
    public AudioSource irritatedSource;

    private GameObject currentDangerGameObject;
    private Coroutine currentDangerCoroutine;

    private Coroutine currentRumbleCoroutine;
    private bool inRumble;

    public Shark curShark;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("trigger");
            if (currentRumbleCoroutine != null)
            {
                StopCoroutine(currentRumbleCoroutine);
            }

            curShark.Irritate();
            currentRumbleCoroutine = StartCoroutine(RunRumble());
        }

        if (curShark.irritated)
        {
            if (!inRumble)
            {
                currentRumbleCoroutine ??= StartCoroutine(RunRumble());
            }
        }
        else
        {
            if (currentRumbleCoroutine != null)
            {
                StopCoroutine(currentRumbleCoroutine);
            }

            if (!normalSource.isPlaying)
            {
                irritatedSource.Stop();
                normalSource.Play();
            }
        }

        if (Input.touchCount > 0
            && Input.GetTouch(0).phase == TouchPhase.Began
        )
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.GetTouch(0).position),
                out var raycastHit,
                Mathf.Infinity,
                fishLayer
            ))
            {
                if (currentDangerCoroutine != null)
                {
                    StopCoroutine(currentDangerCoroutine);
                }

                currentDangerCoroutine = StartCoroutine(Danger(raycastHit.point));
            }
            else if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.GetTouch(0).position),
                out raycastHit,
                Mathf.Infinity,
                sharkLayer
            ))
            {
                print("shark bites you");
                if (currentRumbleCoroutine != null)
                {
                    StopCoroutine(currentRumbleCoroutine);
                }

                curShark = raycastHit.collider.GetComponent<Shark>();
                curShark.Irritate();
                currentRumbleCoroutine = StartCoroutine(RunRumble());
            }
        }
    }

    IEnumerator Danger(Vector3 position)
    {
        if (currentDangerGameObject)
        {
            Destroy(currentDangerGameObject);
        }

        currentDangerGameObject = Instantiate(dangerSimulationPrefab, position, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);

        if (currentDangerGameObject)
        {
            Destroy(currentDangerGameObject);
        }
    }

    IEnumerator RunRumble()
    {
        inRumble = true;
        if (!irritatedSource.isPlaying)
        {
            normalSource.Stop();
            irritatedSource.Play();
        }

        Vector3 start = ocean.transform.position;
        for (float t = 0; t < rumbleTime; t += Time.deltaTime)
        {
            float p = 1 - t / rumbleTime;
            ocean.transform.position = start + Random.onUnitSphere * rumbleDist * p;
            yield return null;
        }

        inRumble = false;
    }
}