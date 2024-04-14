using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [ReadOnly(false)][SerializeField] private float speed = 0;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject flag;
    private static readonly float STOP_SPEED = 0.002f;
    private static readonly float SPEED_LOSE = 0.96f;

    private Vector2 startPos;
    private Vector2 lastPos;
    private Camera _cam;
    private AudioSource audioSource;
    private Vector3 cameraOffset;

    private NetworkManager NetworkManager;
    private void Awake()
    {
        /* Initialize */
        _cam = Camera.main;
        Debug.Assert(_cam != null);
        NetworkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(NetworkManager != null);
        TryGetComponent(out audioSource);

        /* set params */
        cameraOffset = _cam.transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            
            /* UI or Other */
            Vector3 temp = _cam.ScreenToWorldPoint(startPos) - cameraOffset;
            lineRenderer.SetPosition(0, temp);
            lineRenderer.SetPosition(1, temp);
            lineRenderer.enabled = true;
        }
        else if (Input.GetMouseButton(0))
        {
            lastPos = Input.mousePosition;

            /* UI or Other */
            Vector3 temp = _cam.ScreenToWorldPoint(lastPos) - cameraOffset;
            lineRenderer.SetPosition(1, temp);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastPos = Input.mousePosition;
            
            float swipeLength = this.lastPos.x - startPos.x;
            speed = swipeLength / 500.0f;
            
            /* UI or Other */
            lineRenderer.enabled = false;
            
            /* Audio */
            audioSource.Play();
            
            /* NetWork */
            this.NetworkManager.networkFlag = 1;
        }
        
        transform.Translate(this.speed, 0,0);
        speed *= SPEED_LOSE;
        if (this.speed < STOP_SPEED && this.speed > -STOP_SPEED)
        {
            CarStoped();
        }
    }

    private void CarStoped()
    {
        speed = 0;
        float length = flag.transform.position.x - transform.position.x;
        if (length >= 0 && NetworkManager.networkFlag == 1)
        {
            NetworkManager.SendLengthToServer(length);
        }
    }
    
}