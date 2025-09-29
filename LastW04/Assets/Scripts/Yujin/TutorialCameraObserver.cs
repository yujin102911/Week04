using UnityEngine;

public class TutorialCameraObserver : MonoBehaviour
{
    // [System.Serializable]을 사용하면 아래 클래스의 변수들을 인스펙터 창에서 편하게 설정할 수 있습니다.
    [System.Serializable]
    public class CameraRegionLink
    {
        public Camera camera;
        [Tooltip("이 카메라가 활성화되었을 때 LevelManager에게 알려줄 Region ID")]
        public string regionId;
    }

    [Header("감시할 카메라 목록")]
    [SerializeField] private CameraRegionLink[] cameraLinks;

    // 마지막으로 활성화되었던 카메라를 기억하기 위한 변수
    private Camera lastActiveCamera = null;

    void Update()
    {
        Camera currentActiveCamera = null;

        // 목록에 있는 모든 카메라를 확인하여 현재 켜져 있는 카메라를 찾습니다.
        foreach (var link in cameraLinks)
        {
            if (link.camera != null && link.camera.gameObject.activeInHierarchy)
            {
                currentActiveCamera = link.camera;
                break; // 켜진 카메라를 하나 찾았으면 더 이상 찾을 필요가 없으므로 반복을 중단합니다.
            }
        }

        // 1. 현재 켜진 카메라가 있고, 
        // 2. 이전에 켜져 있던 카메라와 다르다면 (즉, 카메라가 방금 바뀌었다면)
        if (currentActiveCamera != null && currentActiveCamera != lastActiveCamera)
        {
            // 방금 켜진 카메라를 마지막으로 활성화된 카메라로 기억합니다.
            lastActiveCamera = currentActiveCamera;

            // 방금 켜진 카메라와 연결된 Region ID를 찾습니다.
            string newRegionId = "";
            foreach (var link in cameraLinks)
            {
                if (link.camera == currentActiveCamera)
                {
                    newRegionId = link.regionId;
                    break;
                }
            }

            // LevelManager가 존재하고, 찾은 Region ID가 비어있지 않다면
            if (LevelManager.Instance != null && !string.IsNullOrEmpty(newRegionId))
            {
                Debug.Log(newRegionId + " 지역으로 업데이트를 요청합니다.");
                // LevelManager에게 Region 변경을 요청합니다.
                // affectCamera: false 옵션을 통해 LevelManager가 카메라를 직접 제어하지 않도록 합니다.
                LevelManager.Instance.SetCurrentRegion(newRegionId, affectCamera: false);
            }
        }
    }
}