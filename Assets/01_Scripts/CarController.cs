using System;
using System.Collections;
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
    
    private Vector2 targetPos;
    private Vector2 velocity;
    
    private NetworkManager networkManager;
    private GameDirector gameDirector;
    private SwipeGame_PlayerData swipeGamePlayerData = null;
    private InGameHandler inGameHandler;
    private void Awake()
    {
        /* Initialize */
        _cam = Camera.main;
        Debug.Assert(_cam != null);
        networkManager = FindObjectOfType<NetworkManager>();
        Debug.Assert(networkManager != null);
        gameDirector = FindObjectOfType<GameDirector>();
        Debug.Assert(gameDirector != null);
        lineRenderer = FindObjectOfType<LineRenderer>();
        Debug.Assert(lineRenderer != null);
        inGameHandler = FindObjectOfType<InGameHandler>();
        Debug.Assert(inGameHandler != null);
        if(!TryGetComponent(out audioSource))
        {
            Debug.Assert(false);    
        }
        


        flag = GameObject.Find("flag");
        Debug.Assert(flag != null);
    }

    private void Start()
    {
        /* set params */
        cameraOffset = _cam.transform.position;
    }

    /* For Debugging */
    [ContextMenu("PrintCarInfo")]
    private void PrintCarInfo()
    {
        Debug.Log($"GameType: {gameDirector.gameType}");
        Debug.Log($"HostID: {networkManager.hostId}");
        Debug.Log($"PlayerID: {swipeGamePlayerData.id}");
        Debug.Log($"NickName: {swipeGamePlayerData.nickName}");
        Debug.Log($"IsHost = {swipeGamePlayerData.id == networkManager.hostId}");
    }
    
    private void Update()
    {
        if (swipeGamePlayerData == null || swipeGamePlayerData.id != networkManager.hostId || gameDirector.gameType != GameDirector.EGameType.InGame)
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
            
            /* Network */
            inGameHandler.SendCarMove(startPos, lastPos);
        }
    }
    
    public void Move(float magnitude)
    {
        targetPos = (Vector2)transform.position + new Vector2(magnitude, 0);
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        float smoothTime = 1f; // 부드러운 이동을 위한 시간
        while (Vector2.Distance(transform.position, targetPos) > STOP_SPEED)
        {
            // SmoothDamp를 사용하여 부드럽게 이동합니다.
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            yield return null;
        }
    }

    public void SetPlayerData(SwipeGame_PlayerData pd)
    {
        this.swipeGamePlayerData = pd;
    }
    
}
