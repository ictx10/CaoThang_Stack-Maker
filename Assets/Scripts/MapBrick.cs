using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBrick : MonoBehaviour
{
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private MeshRenderer _meshCollider;
    public ePlayerDicrection ePlayerPush = ePlayerDicrection.None;
    public bool isPush {  get; private set; } = false;
    public bool isStop = false; 

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    private void Reset()
    {
        Init();
    }
    void Init()
    {
        if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
        if (_meshCollider == null) _meshCollider = GetComponent<MeshRenderer>();
        if (_meshCollider != null && transform.CompareTag(TagConst.TAG_BRIDGE))
        {
            _meshCollider.enabled = false;
        }
    }
    public void DisableComponent()
    {
        if (transform.CompareTag(TagConst.TAG_START_AREA) || transform.CompareTag(TagConst.TAG_WIN_AREA))
        {
            return;
        }
        else if (transform.CompareTag(TagConst.TAG_BRIDGE))
        {
            if (_boxCollider != null)
            {
                _meshCollider.enabled = true;
            }
            isPush = true;
            return;
        }
        if (_meshCollider != null)
        {
            _meshCollider.enabled = false;
        }
        isPush = true;
    }
}
