using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;

    private int currentTileId = -1;
    private int clickedTileId = -1;

    private bool isMoving = false;
    private Coroutine coMove = null;

    private readonly GraphSearch graphSearch = new GraphSearch();

    public float moveSpeed = 20f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0f;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickedTileId = stage.ScreenPosToTileId(Input.mousePosition);
            if (clickedTileId >= 0 && clickedTileId < stage.Map.tiles.Length)
            {
                if (!isMoving)
                {
                    MoveTo(clickedTileId);
                }
            }
        }
    }

    public void Warp(int tileId)
    {
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }

        animator.speed = 1f;
        isMoving = false;
        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
        stage.OnTileVisited(currentTileId);
    }

    public void MoveTo(int tileId)
    {
        var startNode = stage.Graph.nodes[currentTileId];
        var endNode = stage.Graph.nodes[tileId];

        if (currentTileId == tileId || !endNode.CanVisit)
        {
            return;
        }

        graphSearch.Init(stage.Graph);
        if (graphSearch.AStar(startNode, endNode))
        {
            if (coMove != null)
            {
                StopCoroutine(coMove);
                coMove = null;
            }
            coMove = StartCoroutine(OnMove());
        }
    }

    private IEnumerator OnMove()
    {
        isMoving = true;
        animator.speed = 1f;

        for (int i = 1; i < graphSearch.path.Count; i++)
        {
            var nextTileId = graphSearch.path[i].id;
            var startPos = transform.position;
            var endPos = stage.GetTilePos(nextTileId);
            var duration = Vector3.Distance(startPos, endPos) / moveSpeed;

            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            currentTileId = nextTileId;
            transform.position = endPos;
            stage.OnTileVisited(currentTileId);
        }

        animator.speed = 0f;
        isMoving = false;
        coMove = null;
    }
}