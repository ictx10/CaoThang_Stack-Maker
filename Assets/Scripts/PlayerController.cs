using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public enum ePlayerDicrection
{
    None = 0,
    Forward = 1,
    Right = 2,
    Left = 3,
    Backward = 4,
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float _timeToMove = 0.05f;
    [SerializeField] private MapBrick _rayCurrentBrick;
    [SerializeField] private BrickPlayer _brickPlayer;
    [SerializeField] private FinishLevel _finishLevel;

    [SerializeField] private int score = 0;

    private Vector3 startPosTouch;
    private Vector3 endPosTouch;
    private Vector3 dirPlayerPos;
    private ePlayerDicrection currentDirection = ePlayerDicrection.None;

    private readonly Vector3[] _rotationVectors =
    {
        Vector3.zero,
        new Vector3 (0, 180, 0),
        new Vector3 (0, 270, 0),
        new Vector3 (0, 90, 0),
        new Vector3 (0,0, 0),
    };

    private readonly Dictionary<string, System.Action> _tagActions = new Dictionary<string, System.Action>();
    public int Score
    {
        get => score;
        set
        {
            score = value;
        }
    }

    private void Awake()
    {
        _tagActions[TagConst.TAG_WIN_AREA] = EndLevel;

    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputTouch();
    }
    private void FixedUpdate()
    {
        
    }

    private void Init()
    {
        if (startPoint == null || _brickPlayer == null)
        {
            enabled = false;
            return;
        }
        transform.position = startPoint.position;
        currentDirection = ePlayerDicrection.None;
        dirPlayerPos = Vector3.zero;
    }
    bool CastRay()
    {
        if (currentDirection == ePlayerDicrection.None)
        {
            return false;
        }
        Vector3 directionOfRay = currentDirection switch
        {
            ePlayerDicrection.Forward => Vector3.forward,
            ePlayerDicrection.Backward => Vector3.back,
            ePlayerDicrection.Right => Vector3.right,
            ePlayerDicrection.Left => Vector3.left,
            _ => Vector3.zero
        };

        Vector3 rayOrigin = new Vector3(transform.position.x, transform.root.position.y + 0.1f, transform.position.z);
        bool raycastHit = Physics.Raycast(rayOrigin, directionOfRay, out RaycastHit hit, 1f);

        if (raycastHit)
        {
            _rayCurrentBrick = hit.collider.GetComponent<MapBrick>();
            if (_rayCurrentBrick != null)
            {
                if (hit.collider.CompareTag(TagConst.TAG_GROUND))
                {
                    return false;
                }
                else if (_rayCurrentBrick.CompareTag(TagConst.TAG_BRICK) || _rayCurrentBrick.CompareTag(TagConst.TAG_BRIDGE))
                {
                    if (_brickPlayer != null && !_rayCurrentBrick.isPush)
                    {
                        _brickPlayer.Count += _rayCurrentBrick.CompareTag(TagConst.TAG_BRIDGE) ? -2 : 1;
                    }
                    if (_rayCurrentBrick.CompareTag(TagConst.TAG_BRICK) && !_rayCurrentBrick.isPush)
                    {
                        Score++;
                    }
                    _rayCurrentBrick.DisableComponent();
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        _rayCurrentBrick = null;
        return true;
    }
    private void GetInputTouch()
    {
        if (Camera.main == null) return;
        var mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            startPosTouch = mousePos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPosTouch = mousePos;
            dirPlayerPos = (endPosTouch - startPosTouch).normalized;
            currentDirection = GetDirectionFromVector(dirPlayerPos);
            PlayerRotate();
            StartCoroutine(MoveStepByStep(_timeToMove));
        }
    }
    private ePlayerDicrection GetDirectionFromVector(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.1f) return ePlayerDicrection.None;

        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            return direction.x > 0 ? ePlayerDicrection.Right : ePlayerDicrection.Left;
        }
        return direction.y > 0 ? ePlayerDicrection.Forward : ePlayerDicrection.Backward;
    }

    private IEnumerator MoveStepByStep(float timeToMove)
    {
        while (currentDirection != ePlayerDicrection.None)
        {
            bool canMove = CastRay();

            if (!canMove || _brickPlayer == null)
            {
                StopMoving();
                break;
            }

            PlayerMove();

            // Kiểm tra sau khi di chuyển
            if (_rayCurrentBrick != null && _rayCurrentBrick.isStop)
            {
                Debug.Log("Stopping");
                StopMoving();
                break;
            }

            // Cập nhật vị trí của player
            if (_rayCurrentBrick != null && _rayCurrentBrick.ePlayerPush != ePlayerDicrection.None && !_rayCurrentBrick.isStop)
            {
                currentDirection = _rayCurrentBrick.ePlayerPush;
                PlayerRotate();
                _rayCurrentBrick = null; // Reset current brick ray after moving
            }

            if (_rayCurrentBrick != null && _tagActions.TryGetValue(_rayCurrentBrick.tag, out var action))
            {
                action.Invoke();
                _rayCurrentBrick = null;
            }

            yield return new WaitForSeconds(timeToMove);
        }
    }
    private void PlayerRotate()
    {
        if (currentDirection == ePlayerDicrection.None)
        {
            return;
        }
        transform.rotation = Quaternion.Euler(_rotationVectors[(int)currentDirection]);
    }
    private void PlayerMove()
    {
        Vector3 moveDirection = currentDirection switch
        {
            ePlayerDicrection.Forward => Vector3.forward,
            ePlayerDicrection.Backward => Vector3.back,
            ePlayerDicrection.Right => Vector3.right,
            ePlayerDicrection.Left => Vector3.left,
            _ => Vector3.zero
        };
        transform.position += moveDirection * moveSpeed;
    }
    private void StopMoving()
    {
        currentDirection = ePlayerDicrection.None;
        _rayCurrentBrick = null ;
    }
    private void EndLevel()
    {
        StopMoving();
        Debug.Log("Level Ended");
        StartCoroutine(EffectEndLevel());
    }
    private IEnumerator EffectEndLevel()
    {
        while (_brickPlayer != null && _brickPlayer.Count > 0)
        {
            _brickPlayer.Count--;
            Score++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitUntil(() => _brickPlayer == null || _brickPlayer.Count == 0);
        if (_brickPlayer != null) _finishLevel.EffectEndLevel();
    }

}
