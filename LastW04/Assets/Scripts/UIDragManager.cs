using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject prefabToSpawn; // 씬에 배치할 프리팹
    public Grid grid;                // 씬의 Grid
    private GameObject draggingInstance;//드래그중인 것
    private GameObject PlacedInstance;//드래그끝 배치한 것
    public GameObject gameManager;
    public void Update()
    {
        if (draggingInstance != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(draggingInstance.transform.position);
            if (screenPos.z > 0 && screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height)
            {
                // 화면 안에 있을 때만 GUI 처리
            }
        }

    }
    // UI에서 클릭 시작
    public void OnPointerDown(PointerEventData eventData)
    {
        // 드래그 시작할 때 프리팹 인스턴스 생성
        if (!PlacedInstance)
        {
            draggingInstance = Instantiate(prefabToSpawn);
        }
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (draggingInstance == null) return;

        // 화면 좌표 → 월드 좌표
        Vector3 screenPos = eventData.position;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = -1f; // 2D용 Z값

        // 그리드에 스냅
        Vector3Int cell = grid.WorldToCell(worldPos);
        Vector3 aligned = grid.CellToWorld(cell);

        draggingInstance.transform.position = aligned+Vector3.back;
    }

    // 드래그 끝
    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggingInstance == null) return;
        ObjectPlacer placerMode = gameManager.GetComponent<ObjectPlacer>();
        if (placerMode!=null)
        {
            placerMode.isInstallMode = true;
            Debug.Log("dem");
        }  
            
        PlacedInstance = draggingInstance;
        draggingInstance = null; // 드래그 완료 후 매니저에서 제어 끝

    }
}
