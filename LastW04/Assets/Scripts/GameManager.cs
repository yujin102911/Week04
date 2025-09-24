using UnityEngine;

public enum Mode//모드설정
{
    None,Editing
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;    
    public Grid grid;
    public Mode mode;
    Transform draggingObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()//싱글톤
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 살아남음
        }
        else
        {
            Destroy(gameObject); // 이미 있으면 중복 방지
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == Mode.Editing)
        {

            if (Input.GetMouseButtonDown(0))
            {
                // 마우스 아래 오브젝트 감지
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null)
                {
                    draggingObject = hit.collider.transform;
                }
            }

            if (Input.GetMouseButton(0) && draggingObject != null)
            {
                // 드래그 중
                Vector2 mousePos = Input.mousePosition;
                draggingObject.position = Camera.main.ScreenToWorldPoint(mousePos);
            }

            if (Input.GetMouseButtonUp(0) && draggingObject != null)
            {
                // 스냅 처리/*
                /*Vector2Int cell = Vector2.grid.WorldToCell(draggingObject.position);
                draggingObject.position = grid.CellToWorld(cell);*/
                draggingObject = null;
            }
        }
    }
}
