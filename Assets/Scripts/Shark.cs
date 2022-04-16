// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB Simple Boids. MIT license - license_nvjob.txt
// #NVJOB Nicholas Veselov - https://nvjob.github.io
// #NVJOB Simple Boids v1.1.1 - https://nvjob.github.io/unity/nvjob-boids


using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[HelpURL("https://nvjob.github.io/unity/nvjob-boids")]
[AddComponentMenu("#NVJOB/Boids/Shark")]

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class Shark : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [Header("General Settings")] public float speed = 5;
    public float walkZone = 100;
    public Transform camRig;
    public bool debug;

    [Header("Hunting Settings")] public float huntingZone = 50;
    public LayerMask layerFlock;
    public Material sharkMaterial;

    //--------------

    Transform thisTransform;
    Vector3 vel, velCam, target, targetCurent, targetRandom, targetFlock;
    [SerializeField] float startYpos, huntTime, huntSpeed, speedSh, acselSh;
    bool hunting;
    static WaitForSeconds delay0 = new WaitForSeconds(8.0f);

    public bool irritated;

    public float curSpeed;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public AudioClip normalAudio;
    public AudioClip irritatedAudio;
    public AudioSource bgm;
    public GameObject angerMark;

    void Awake()
    {
        //--------------

        thisTransform = transform;
        curSpeed = speed;
        startYpos = transform.position.y;
        huntSpeed = 1.0f;

        StartCoroutine(RandomVector());

        //--------------
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Update()
    {
        angerMark.SetActive(irritated);
    }

    void LateUpdate()
    {
        //--------------

        Hunting();
        Move();
        // CameraRig();
        DebugPath();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Hunting()
    {
        //-------------- 
        if (irritated)
        {
            print("shark bites you");
            if ((camRig.position - transform.position).magnitude > huntingZone)
            {
                irritated = false;
            }
        }

        curSpeed = irritated ? speed * 2 : speed;

        if (hunting || irritated)
        {
            if (huntTime < 4.0f)
            {
                huntTime += Time.deltaTime * 0.5f;
            }
            else
            {
                hunting = false;
                irritated = false;
            }

            if (irritated)
            {
                targetCurent = camRig.position;
            }
            else
            {
                Collider[] flockColliders = Physics.OverlapSphere(thisTransform.position, huntingZone, layerFlock);
                if (flockColliders.Length > 0) targetFlock = flockColliders[0].transform.position;
                targetCurent = targetFlock;
            }

            if (huntSpeed < 2.1f) huntSpeed += Time.deltaTime * 0.2f;
            if (acselSh < 0.6f) acselSh += Time.deltaTime * 0.1f;
        }
        else
        {
            targetCurent = targetRandom;
            if (huntTime > 0) huntTime -= Time.deltaTime;
            else if (Physics.CheckSphere(thisTransform.position, huntingZone, layerFlock)) hunting = true;
            if (huntSpeed > 1.0f) huntSpeed -= Time.deltaTime * 0.2f;
            if (acselSh > 0) acselSh -= Time.deltaTime * 0.1f;
        }

        if (acselSh >= 0)
        {
            speedSh += Time.deltaTime * acselSh;
            sharkMaterial.SetFloat("_ScriptControl", speedSh);
        }

        //--------------
    }

    public void Irritate()
    {
        huntTime = 0f;
        irritated = true;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Move()
    {
        //--------------

        target = Vector3.SmoothDamp(target, targetCurent, ref vel, 3.0f);
        target = new Vector3(target.x, startYpos, target.y);
        Vector3 newDir = Vector3.RotateTowards(thisTransform.forward, target - thisTransform.position,
            Time.deltaTime * 0.35f, 0);
        thisTransform.rotation = Quaternion.LookRotation(newDir);
        thisTransform.Translate(Vector3.forward * Time.deltaTime * huntSpeed * curSpeed);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void CameraRig()
    {
        //--------------

        camRig.position = thisTransform.position;

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    IEnumerator RandomVector()
    {
        //--------------

        while (true)
        {
            targetRandom = Random.insideUnitSphere * walkZone;
            yield return delay0;
        }

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void DebugPath()
    {
        //--------------

        if (debug == true) Debug.DrawLine(thisTransform.position, targetCurent);

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}