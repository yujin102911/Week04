using UnityEngine;
using UnityEngine.InputSystem;

public enum Mode//모드설정
{
    None,Editing
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;    
    public Grid grid;
    public Mode mode;
    public GameObject editingUI;
    Transform draggingInstance;
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
        if (Input.GetKeyUp(KeyCode.E)) //에딧모드 전환
        {
            if (mode != Mode.Editing)
            {
                mode = Mode.Editing;
            }
            else 
            {
                mode = Mode.None; 
            }
        }
        if (mode == Mode.Editing)//에디팅 모드 시 편집 기능
        {
            editingUI.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                // 마우스 아래 오브젝트 감지
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null)
                {
                    draggingInstance = hit.collider.transform;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                // 마우스 아래 오브젝트 감지
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null)
                {
                    if (hit.collider.tag == "EditorbleUI")
                    {
                        if (hit.transform.parent != null)
                        {
                            Destroy(hit.transform.parent.gameObject);
                        }
                        else
                        {
                            Destroy(hit.collider.gameObject);
                        }
                            
                    }
                }
            }

            if (Input.GetMouseButton(0) && draggingInstance != null)
            {
                if (draggingInstance == null) return;

                // 화면 좌표 → 월드 좌표
                Vector3 screenPos = Mouse.current.position.ReadValue();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                worldPos.z = -1f; // 2D용 Z값

                // 그리드에 스냅
                Vector3Int cell = grid.WorldToCell(worldPos);
                Vector3 aligned = grid.CellToWorld(cell);

                draggingInstance.transform.position = aligned + Vector3.back;
            }

            if (Input.GetMouseButtonUp(0) && draggingInstance != null)
            {
                // 스냅 처리/*
                /*Vector2Int cell = Vector2.grid.WorldToCell(draggingObject.position);
                draggingObject.position = grid.CellToWorld(cell);*/
                draggingInstance = null;
            }
        } else 
        {
            editingUI.SetActive(false);
        }
    }
}
