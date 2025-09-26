using UnityEngine;
using UnityEngine.InputSystem;

public class UIEditor : MonoBehaviour
{
    public Grid grid;
    public GameObject editingUI;//하단OnOFF용
    public Transform draggingInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.mode != Mode.Editing)//에딧 아니면
        {
            editingUI.SetActive(true);//에딧UI 가림막 거짓으로
        }
        else
        {
            editingUI.SetActive(false);//에딧UI 가림막 참으로
        }

        if (GameManager.mode == Mode.Editing)//에디팅 모드 시 편집 기능
        {
            if (Input.GetMouseButtonDown(0))//좌클 시
            {
                // 마우스 아래 오브젝트 감지
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null && hit.collider.CompareTag("EditorbleUI"))
                {
                    if (hit.transform.parent != null)
                    {
                        draggingInstance=hit.transform.parent;
                    }
                    else
                    {
                        draggingInstance=hit.transform;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))//우클 시 삭제
            {
                // 마우스 아래 오브젝트 감지
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null && hit.collider.CompareTag("EditorbleUI"))
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

            if (Input.GetMouseButton(0) && draggingInstance != null)//좌클 드래그중일때
            {
                Vector2 screenPos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                // 그리드에 스냅
                Vector3Int cell = grid.WorldToCell(worldPos);
                Vector3 aligned = grid.GetCellCenterWorld(cell);
                draggingInstance.transform.position = aligned + Vector3.back;
            }
            if(Input.GetMouseButtonUp(0) && draggingInstance != null)
            {   //놓은 곳에 아무것도 없으면
                var hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0);
                foreach (var h in hits)
                {
                    if (h.CompareTag("EditorbleUI"))//다른 UI있으면
                    {
                        draggingInstance = null;
                        return;//밑에 코드 실행 ㄴㄴ
                    }
                }
                draggingInstance = null;


            }
        }
    }
}
