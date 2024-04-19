using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [ReadOnly(false)][SerializeField] private float speed = 0;
    private LineRenderer lineRenderer;
    private GameObject flag;
    private static readonly float STOP_SPEED = 0.002f;
    private static readonly float SPEED_LOSE = 0.96f;
    
    private Vector2 startPos;
    private Vector2 lastPos;
    private Camera _cam;
    private AudioSource audioSource;
    private Vector3 cameraOffset;

    private NetworkManager networkManager;
    private GameDirector gameDirector;
    private PlayerData playerData = null;
    private void Awake()
    {
        /* Initialize */
        _cam = Camera.main;
        Debug.Assert(_cam != null);
        networkManager = FindObjectOfType<NetworkManager>();
            Debug.Assert(networkManager != null);
        gameDirector = FindObjectOfType<GameDirector>();
        Debug.Assert(gameDirector != null);
        TryGetComponent(out audioSource);
        lineRenderer = FindObjectOfType<LineRenderer>();
        flag = GameObject.Find("flag");

        /* set params */
        cameraOffset = _cam.transform.position;
    }

    /* For Debugging */
    [ContextMenu("PrintCarInfo")]
    private void PrintCarInfo()
    {
        Debug.Log($"GameType: {gameDirector.gameType}");
        Debug.Log($"HostID: {networkManager.hostId}");
        Debug.Log($"PlayerID: {playerData.id}");
        Debug.Log($"IsSame = {playerData.id == networkManager.hostId}");
    }
    
    private void Update()
    {
        if (playerData == null || playerData.id != networkManager.hostId || gameDirector.gameType != GameDirector.EGameType.InGame)
        {
            return;
        }
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
            this.networkManager.networkFlag = 1;
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
        if (length >= 0 && networkManager.networkFlag == 1)
        {
            gameDirector.gameType = GameDirector.EGameType.GameEnd;
        }
    }

    public void SetPlayerData(PlayerData pd)
    {
        this.playerData = pd;
    }
    
}
