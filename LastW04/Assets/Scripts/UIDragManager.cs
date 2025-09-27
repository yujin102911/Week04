using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragManager : MonoBehaviour, IPointerDownHandler,  IPointerUpHandler
{
    public GameObject prefabToSpawn; // 씬에 배치할 프리팹
    public Grid grid;                // 씬의 Grid
    public GameObject previewInstance;//미리보기할 프리팹
    private GameObject draggingInstance;//드래그중인 것
    private GameObject PlacedInstance;//드래그끝 배치한 것
    public int levelCurrent;//레벨
    public int levelBefore;//이전레벨
    public int limit;//레벨당 배치 제한

    public GameObject gameManager;
    public void Update()
    {
        if (draggingInstance != null)
        {
            Vector2 screenPos = Input.mousePosition;
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(draggingInstance.transform.position);
            //if (screenPos.z > 0 && screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height)
            if (screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height)
            {
                // 화면 안에 있을 때만 GUI 처리
            }
            
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            // 그리드에 스냅
            Vector3Int cell = grid.WorldToCell(worldPos);
            Vector3 aligned = grid.GetCellCenterWorld(cell);

            draggingInstance.transform.position = aligned + Vector3.back;
        }
        if (levelCurrent != levelBefore) //이전 레벨과 달라지면
        {
            
        }
    }
    // UI에서 클릭 시작
    public void OnPointerDown(PointerEventData eventData)
    {
        // 드래그 시작할 때 프리팹 인스턴스 생성
        //if (!PlacedInstance)
        {
            draggingInstance = Instantiate(previewInstance);
        }
    }
    public void OnPointerUp(PointerEventData eventData)//클릭 때면
    {
        if (draggingInstance == null) return;           //든거 없음 종료
        var hits = Physics2D.OverlapBoxAll(draggingInstance.transform.position, new Vector2(1, 1), 0);
        foreach (var h in hits)
        {
            if (h.CompareTag("EditorbleUI"))//다른 UI있으면
            {
                Destroy(draggingInstance);
                draggingInstance = null;
                return;//밑에 코드 실행 ㄴㄴ
            }
        }
        PlacedInstance = Instantiate(prefabToSpawn);//재대로 된거소환
        PlacedInstance.transform.position = draggingInstance.transform.position;//미리보기 위치로
        Destroy(draggingInstance);

    }
}
