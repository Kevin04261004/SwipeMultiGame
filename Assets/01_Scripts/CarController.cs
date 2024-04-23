using System;
using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [ReadOnly(false)][SerializeField] private float speed = 0;
    private LineRenderer lineRenderer;
    private GameObject flag;
    private Transform SpawnTransform;
    private static readonly float STOP_SPEED = 0.002f;
    private static readonly float SPEED_LOSE = 0.96f;
    private static readonly float LESS_TOUCH = 10f;
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
    private bool hasSwipe = false;
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
        SpawnTransform = GameObject.Find("SpawnPos").transform;
        Debug.Assert(SpawnTransform != null);

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

    public bool IsHost()
    {
        return swipeGamePlayerData.id == networkManager.hostId;
    }
    
    private void Update()
    {
        if (swipeGamePlayerData == null || swipeGamePlayerData.id != networkManager.hostId || gameDirector.gameType != GameDirector.EGameType.InGame || hasSwipe)
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
            /* UI or Other */
            lineRenderer.enabled = false;
            if (swipeLength < LESS_TOUCH)
            {
                return;
            }

            /* Network */
            inGameHandler.SendCarMove(_cam.ScreenToWorldPoint(startPos), _cam.ScreenToWorldPoint(lastPos));
            hasSwipe = true;
        }
    }
    
    public void Move(float magnitude)
    {
        /* Audio */
        audioSource.Play();
        targetPos = (Vector2)transform.position + new Vector2(magnitude, 0);
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        float smoothTime = 1f;
        while (Vector2.Distance(transform.position, targetPos) > STOP_SPEED)
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            yield return null;
        }

        if (IsHost())
        {
            gameDirector.gameType = GameDirector.EGameType.GameEnd;
            inGameHandler.retryButton.SetActive(true);
        }
    }

    public void RetryGame()
    {
        hasSwipe = false;
        transform.position = SpawnTransform.position;
    }
    public void SetPlayerData(SwipeGame_PlayerData pd)
    {
        this.swipeGamePlayerData = pd;
    }
}
