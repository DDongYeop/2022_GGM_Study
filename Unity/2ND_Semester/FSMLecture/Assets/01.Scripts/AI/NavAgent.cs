using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LineRenderer))]
public class NavAgent : MonoBehaviour
{
    private PriorityQueue<Node> _openList;
    private List<Node> _closeList;

    private List<Vector3Int> _routePath;

    public float speed = 5f;
    public bool cornerCheck = false;
    private bool _isMove = false;
    private int _moveIdx = 0; //라우트 패스의 몇번쨰를 진행하고 있는지
    private Vector3 _nextPos; //다음에 이동할 월드 포지션 

    [SerializeField] private bool _isDebug = false;

    private Vector3Int _currentPosition; //현재 타일 위치
    private Vector3Int _destination; //목표 타일 위치
    public Vector3Int Destination
    {
        get => _destination;
        set
        {
            SetCurrentPosition(); // 현재 위치에 따라 내 위치 배정
            _destination = value;
            CalcRoute(); // 이걸 하고나면 RoutePath가 리셋팅 됨
            _moveIdx = 0;
            if (_isDebug) PrintRoute(); //디버그 모드 켜져있다면 라우팅 경로 라인렌ㅌ더러로 그림 
        }
    }

    //더 갈 수 있는가?
    public bool CanMovePath => _routePath.Count > _moveIdx;

    private LineRenderer _lineRenderer;

    private void Awake() 
    {
        _openList = new PriorityQueue<Node>();
        _closeList = new List<Node>();
        _routePath = new List<Vector3Int>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start() 
    {
        SetCurrentPosition();
        transform.position = MapManager.Instance.GetWorldPos(_currentPosition);
    }

    private void SetCurrentPosition()
    {
        Vector3Int cellPos = MapManager.Instance.GetTilePos(transform.position);
        _currentPosition = cellPos;
    }

    // private void Update() 
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector3 mPos = Input.mousePosition;
    //         mPos.z = 0;
    //         Vector3 world = Camera.main.ScreenToWorldPoint(mPos);
    //         Vector3Int cellPos = MapManager.Instance.GetTilePos(world); //이걸로 월드를 타일 맵 포지션으로 변경
            
    //         _destination = cellPos;
            
    //         if (CalcRoute())
    //         {
    //             PrintRoute();
    //             _moveIdx = 0;
    //             _isMove = true;
    //             GetNextTarget();
    //         }
    //     }

    //     if (_isMove)
    //     {
    //         Vector3 dir = _nextPos - transform.position;
    //         transform.position += dir.normalized * speed * Time.deltaTime;
    //         if (dir.magnitude <= 0.1f)
    //             GetNextTarget();            
    //     }
    // }

    public Vector3Int GetNextTarget()
    {
        if (_moveIdx >= _routePath.Count)
        {
            //_isMove = false;
            return Vector3Int.zero;
        }
        // _currentPosition = _routePath[_moveIdx];
        // _nextPos = MapManager.Instance.GetWorldPos(_currentPosition);
        // _moveIdx++;
        return _routePath[_moveIdx];
    }

    private void PrintRoute() //계산한 경로를 디버그로 찍어본다. 
    {
        _lineRenderer.positionCount = _routePath.Count;

        _lineRenderer.SetPositions(_routePath.Select(p => MapManager.Instance.GetWorldPos(p)).ToArray()); //아래있는 for문하고 똑같은 역할
        // for (int i = 0; i < _routePath.Count; i++)
        // {
        //     Vector3 worldPos = MapManager.Instance.GetWorldPos(_routePath[i]);
        //     _lineRenderer.SetPosition(i, worldPos);
        // }
    }

    #region AStar 알고리즘

    private bool CalcRoute()
    {
        _openList.Clear();
        _closeList.Clear();

        _openList.Push(new Node 
        { 
            pos = _currentPosition, 
            _parnet = null, G = 0, 
            F = CalcH(_currentPosition)
        });

        bool result = false; //이건 갈 수 있는 곳인가?
        int cnt = 0; //안전코드
        while (_openList.Count > 0)
        {
            Node n = _openList.Pop(); //가장 가깝게 갈 수 있는 녀석을 뽑아온다
            FindOpenList(n);
            _closeList.Add(n); //n은 이미 썻으니 _closeList에 박아라
            if (n.pos == _destination) //마지막 방문했던 녀석이 목적지면 그럼 나가자
            {
                result = true;
                break;
            }

            //안전코드 
            cnt++;
            if (cnt >= 100000)
            {
                Debug.Log("while루프 너무 돌아서 빠갬");
                break;
            }
        }

        if (result) //길 찾음
        {
            _routePath.Clear();

            Node last = _closeList[_closeList.Count - 1];
            while (last._parnet != null)
            {
                _routePath.Add(last.pos);
                last = last._parnet;
            }
            _routePath.Reverse(); //역순리스트를 출발점부터 다시 들어오게 뒤집어준다 
        }

        return result;
    }

    //너는 노드 N과 연결된 오픈 리스트를 다 찾아서 _openList에 넣어줄거야 
    private void FindOpenList(Node n)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue; //이건 내 현재자리니깐 무시

                Vector3Int nextPos = n.pos + new Vector3Int(x, y, 0);

                Node temp = _closeList.Find(x => x.pos == nextPos); //이 녀식이 이미 방문했는지
                if (temp != null) continue;

                //타일에서 진짜 갈 수 있는 곳인지
                if (MapManager.Instance.CanMove(nextPos))
                {
                    float g = (n.pos - nextPos).magnitude + n.G;

                    Node nextOpenNode = new Node 
                    { 
                        pos = nextPos, 
                        _parnet = n,
                        G = g,
                        F = g + CalcH(nextPos)
                    };
                    //넣기전에 검사 해야함 
                    Node exist = _openList.Contains(nextOpenNode);

                    if (exist != null)
                    {
                        //이건 검증 해봐야함
                        if (nextOpenNode.G < exist.G)
                        {
                            exist.G = nextOpenNode.G;
                            exist.F = nextOpenNode.F;
                            exist._parnet = nextOpenNode._parnet;
                        }
                    }
                    else
                    {
                        _openList.Push(nextOpenNode);
                    }
                }
            }
        }
    }

    private float CalcH(Vector3Int pos)
    {
        //F = G + H
        Vector3Int distance = _destination - pos;
        return distance.magnitude;
    }

    #endregion
}
