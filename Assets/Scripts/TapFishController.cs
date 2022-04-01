using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TapFishController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera mainCamera;

    [Tooltip("An object that will not affect the scene but act as a danger to make the flock escape")]
    public GameObject dangerSimulationPrefab;

    // Update is called once per frame
    public LayerMask fishLayer;

    private GameObject currentDangerGameObject;
    private Coroutine currentDangerCoroutine;

    void Update()
    {
        if (Input.touchCount > 0
            && Input.GetTouch(0).phase == TouchPhase.Began
        )
        {
            print(Input.GetTouch(0).position);
            print(mainCamera.ScreenPointToRay(Input.GetTouch(0).position));
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
}