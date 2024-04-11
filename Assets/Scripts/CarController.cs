using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float oneClickSpeed = 0.2f;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector2 startPos;
    private Vector2 lastPos;
    private static readonly float SPEED_LOSE = 0.96f;
    private Camera _cam;
    private GameObject car;
    private GameObject flag;
    private AudioSource audioSource;
    private Vector3 cameraOffset;

    private NetworkManager NetworkManager;
    private void Awake()
    {
        _cam = Camera.main;
        this.car = GameObject.Find("car");
        this.flag = GameObject.Find("flag");
        cameraOffset = _cam.transform.position;
        audioSource = GetComponent<AudioSource>();
        NetworkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(NetworkManager != null);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.startPos = Input.mousePosition;
            Vector3 temp = _cam.ScreenToWorldPoint(startPos) - cameraOffset;
            lineRenderer.SetPosition(0, temp);
            lineRenderer.SetPosition(1, temp);
            lineRenderer.enabled = true;
        }
        else if (Input.GetMouseButton(0))
        {
            lastPos = Input.mousePosition;
            Vector3 temp = _cam.ScreenToWorldPoint(lastPos) - cameraOffset;

            lineRenderer.SetPosition(1, temp);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastPos = Input.mousePosition;
            float swipeLength = lastPos.x - this.startPos.x;
            this.speed = swipeLength / 500.0f;
            
            lineRenderer.enabled = false;
            audioSource.Play();
            
            this.NetworkManager.networkFlag = 1;
        }
        transform.Translate(this.speed, 0,0);
        this.speed *= SPEED_LOSE;
        if (this.speed < 0.002f && this.speed > -0.002f)
        {
            CarStoped();
        }
    }

    private void CarStoped()
    {
        this.speed = 0;
        float length = this.flag.transform.position.x - this.car.transform.position.x;
        if (length >= 0 && NetworkManager.networkFlag == 1)
        {
            NetworkManager.SendLengthToServer(length);
        }
    }
    
}
