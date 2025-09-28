using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class UIEditor : MonoBehaviour
{
    public Grid grid;
    public GameObject editingUI;//하단OnOFF용
    public GameObject draggingInstance;
    public Vector2 draggingStartPos;
    public Vector3 draggingOffset  ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.mode != Mode.Editing)//에딧 아니면
        {
            editingUI.SetActive(false);//에딧UI 가림막 거짓으로
        }
        else
        {
            editingUI.SetActive(true);//에딧UI 가림막 참으로
        }

        if (GameManager.mode == Mode.Editing)//에디팅 모드 시 편집 기능
        {
            Vector2 screenPos = Input.mousePosition;//마우스 좌표 저장
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);//스크린에서 마우스 좌표

            if (Input.GetMouseButtonDown(0))//좌클 시
            {
                // 마우스 아래 오브젝트 감지
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);//범위 검사

                if (hit.collider != null && hit.collider.CompareTag("EditorbleUI"))
                {
                    if (hit.collider.gameObject.GetComponent<UIPlacer>().placed)//이미 게임모드 들어가서 설치 완료된거면
                    {
                        return;//걍 끝냄
                    }
                    if (hit.transform.parent.CompareTag("EditorbleUI"))//집는 대상 찾기
                    {
                        draggingInstance=hit.transform.parent.gameObject;//집음
                    }
                    else
                    {
                        draggingInstance=hit.transform.gameObject;
                    }
                    draggingStartPos= draggingInstance.transform.position;//초기 위치 설정
                    draggingOffset = draggingStartPos - mouseWorld;
                }
            }

            if (Input.GetMouseButtonDown(1))//우클 시 삭제
            {
                // 마우스 아래 오브젝트 감지
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider != null && hit.collider.CompareTag("EditorbleUI"))
                {
                    if (hit.collider.gameObject.GetComponent<UIPlacer>().placed)//이미 게임모드 들어가서 설치 완료된거면
                    {
                        return;//걍 끝냄
                    }
                    if (hit.transform.parent != null)//제거 대상 찾기
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
                
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                // 그리드에 스냅
                Vector3Int cell = grid.WorldToCell(worldPos + draggingOffset);//오프셋 포함위치 정렬
                Vector3 aligned = grid.GetCellCenterWorld(cell);
                draggingInstance.transform.position = aligned + Vector3.back;
            }

            if(Input.GetMouseButtonUp(0) && draggingInstance != null)//좌클 때면
            {   //놓은 곳에 아무것도 없으면
                var hits = Physics2D.OverlapBoxAll(draggingInstance.transform.position, new Vector2(1, 1), 0);
                foreach (var h in hits)
                {
                    if (h.CompareTag("EditorbleUI")&&h.gameObject!= draggingInstance && h.gameObject != h.transform.IsChildOf(draggingInstance.transform))//다른 UI있으면
                    {
                        draggingInstance.transform.position = draggingStartPos;
                        draggingInstance = null;
                        return;//밑에 코드 실행 ㄴㄴ
                    }
                }
                var xGadget = draggingInstance.GetComponent<XGadget>();
                
                if (xGadget != null) { xGadget.isHeld = true;
                }

                var toggleGadget = draggingInstance.GetComponent<ToggleGadget>();
                if (toggleGadget != null) toggleGadget.isHeld = true;

                draggingInstance = null;//현재위치에 있는 상태로 끝냄

            }
        }
    }
}
