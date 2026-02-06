using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingImage;

    // 스크롤 뷰 관련 오브젝트
    [SerializeField] private Scrollbar _scrollBar;
    [SerializeField] private GameObject _scrollView;
    [SerializeField] private GameObject _contentObject;

    // 스크롤 뷰 View 영역 상하 margin
    [SerializeField] private float _scrollviewMarginTop;
    [SerializeField] private float _scrollviewMarginBottom;

    // 하위 항목 박스 사이즈 관련
    [SerializeField] private float _rectSize = 80;
    [SerializeField] private float _spacing = 10;

    // 시작 박스 항목 박스 갯수 관련
    [SerializeField] private int _rectCount = 12;

    // 한번에 보여지는 박스 갯수
    [SerializeField] private int _showRectCount = 3;

    private List<GameObject> _objects = new List<GameObject>();
    private List<float> _rectPositions = new List<float>();
    private float _pastPos = 0;
    private int _totalObjectCount;
    private int _prevIndex = 0;

    float _scrollbarSize = 0;

    private void Start()
    {
        _pastPos = _scrollBar.value;

        float deltay = _rectSize * _showRectCount + _spacing * (_showRectCount - 1);
        deltay += _scrollviewMarginTop;
        deltay += _scrollviewMarginBottom;
        _scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(_scrollView.GetComponent<RectTransform>().sizeDelta.x, deltay);

        float contenty = (_rectSize + _spacing) * _rectCount - _spacing;
        _contentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_contentObject.GetComponent<RectTransform>().sizeDelta.x, contenty);

        _totalObjectCount = _showRectCount + 2;
        for(int i = 0; i < _rectCount; i++)
        {
            float yPos = -1 * ((_rectSize * i) + _spacing * (i));
            _rectPositions.Add(yPos);

            if (i < _totalObjectCount)
            {
                GameObject obj = _contentObject.transform.GetChild(i).gameObject;
                obj.GetComponent<RectTransform>().localPosition = new Vector3(obj.GetComponent<RectTransform>().localPosition.x, yPos);

                SetContents(obj, i.ToString());
                _objects.Add(obj);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _scrollbarSize = (float)Math.Truncate(_scrollBar.size * 100) / 100;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(_scrollBar.value <= 0)
            {
                float deltaSize = (float)Math.Truncate(_scrollBar.size * 100) / 100;
                if(_scrollbarSize != deltaSize)
                {
                    Debug.Log("Loading");
                }
            }
        }

        int pageCount = _rectCount - _showRectCount;
        int pageOffset = pageCount - 2;

        float delta = _pastPos - _scrollBar.value;

        if (delta == 0)
        {

        }
        else if (delta > 0)
        {
            _pastPos = _scrollBar.value;
            if((int)Math.Round(_scrollBar.value * pageCount) <= pageOffset)
            {
                int temp = pageOffset - (int)Math.Round(_scrollBar.value * pageCount);

                for(int i=_prevIndex; i < temp + 1; i++)
                {
                    int on = _rectCount - pageOffset + i;

                    int objIndex = i % _totalObjectCount;
                    _objects[objIndex].GetComponent<RectTransform>().localPosition
                        = new Vector3(_objects[objIndex].GetComponent<RectTransform>().localPosition.x, _rectPositions[on]);

                    SetContents(_objects[objIndex], on.ToString());
                }
                _prevIndex = temp;
            }
        }
        else
        {
            _pastPos = _scrollBar.value;

            if((int)Math.Round(_scrollBar.value * pageCount) <= pageOffset + 1)
            {
                int temp = pageOffset + 1 - (int)Math.Round(_scrollBar.value * pageCount);
                for(int i = _prevIndex; i >= temp; i--)
                {
                    int on = _rectCount - pageOffset + i;

                    int objIndex = i % _totalObjectCount;
                    _objects[objIndex].GetComponent<RectTransform>().localPosition
                        = new Vector3(_objects[objIndex].GetComponent<RectTransform>().localPosition.x, _rectPositions[on - (_showRectCount + 2)]);

                    SetContents(_objects[objIndex], (on - (_showRectCount + 2)).ToString());
                }
                _prevIndex = temp;
            }
        }
    }

    private void AddContetns(int count)
    {
        float contenty = (_rectSize + _spacing) * count;
        _contentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_contentObject.GetComponent<RectTransform>().sizeDelta.x,
            _contentObject.GetComponent<RectTransform>().sizeDelta.y + contenty);

        _rectCount += count;

        int pastCount = _rectPositions.Count;
        for(int i = pastCount; i < pastCount + count; i++)
        {
            float yPos = -1 * ((_rectSize * i) + _spacing * (i));
            _rectPositions.Add(yPos);
        }
    }

    private void SetContents(GameObject obj, string str)
    {
        obj.GetComponentInChildren<Text>().text = str;
    }

    IEnumerator IELoadData(int count)
    {
        _loadingImage.SetActive(true);
        this.GetComponent<GraphicRaycaster>().enabled = false;

        yield return new WaitForSeconds(3f);

        AddContetns(count);
        _loadingImage.SetActive(false);
        this.GetComponent<GraphicRaycaster>().enabled = true;
    }
}
