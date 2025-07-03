using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _brickPrefabs;
    [SerializeField] private Transform _playerBody;
    [SerializeField] private Transform _brickParent;
    [SerializeField] private Vector3 _startBrickPos = Vector3.zero;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.3f, 0);

    private int _brickCount;
    private readonly List<GameObject> _listBrick = new List<GameObject>();
    public int Count
    {
        get => _brickCount;
        set 
        {
            _brickCount = Mathf.Max(0, value);
            UpdateBrickPos();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Init()
    {
        if (_brickPrefabs == null || _playerBody == null || _brickParent == null)
        {
            Debug.LogError("BrickPrefab, PlayerBody, or BrickParent is not assigned");
            enabled = false;
            return;
        }

        _brickCount = 0;
        _playerBody.localPosition = new Vector3(0, 1.3f, 0);
        ClearBrick();
    }

    void UpdateBrickPos()
    {
        while (_listBrick.Count > _brickCount)
        {
            GameObject newBrick = Instantiate(_brickPrefabs, _brickParent);
            newBrick.SetActive(false);
            _listBrick.Add(newBrick);
        }
        for (int i = 0; i < _listBrick.Count; i++)
        {
            bool isActive = i < _brickCount;
            _listBrick[i].SetActive(isActive);
            if (isActive)
            {
                _listBrick[i].transform.localPosition = _startBrickPos + i * _offset;
            }
            _playerBody.localPosition = _startBrickPos + (_brickCount > 0 ? (_brickCount * _offset) : Vector3.zero);
        }
    }

    void ClearBrick()
    {
        foreach (GameObject brick in _listBrick)
        {
            if (brick != null)
            {
                Destroy(brick);
            }
            _listBrick.Clear();
        }
    }
    private void OnDestroy()
    {
        ClearBrick();
    }
}
