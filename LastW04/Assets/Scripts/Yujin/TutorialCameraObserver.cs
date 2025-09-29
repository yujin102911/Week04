using UnityEngine;

public class TutorialCameraObserver : MonoBehaviour
{
    // [System.Serializable]�� ����ϸ� �Ʒ� Ŭ������ �������� �ν����� â���� ���ϰ� ������ �� �ֽ��ϴ�.
    [System.Serializable]
    public class CameraRegionLink
    {
        public Camera camera;
        [Tooltip("�� ī�޶� Ȱ��ȭ�Ǿ��� �� LevelManager���� �˷��� Region ID")]
        public string regionId;
    }

    [Header("������ ī�޶� ���")]
    [SerializeField] private CameraRegionLink[] cameraLinks;

    // ���������� Ȱ��ȭ�Ǿ��� ī�޶� ����ϱ� ���� ����
    private Camera lastActiveCamera = null;

    void Update()
    {
        Camera currentActiveCamera = null;

        // ��Ͽ� �ִ� ��� ī�޶� Ȯ���Ͽ� ���� ���� �ִ� ī�޶� ã���ϴ�.
        foreach (var link in cameraLinks)
        {
            if (link.camera != null && link.camera.gameObject.activeInHierarchy)
            {
                currentActiveCamera = link.camera;
                break; // ���� ī�޶� �ϳ� ã������ �� �̻� ã�� �ʿ䰡 �����Ƿ� �ݺ��� �ߴ��մϴ�.
            }
        }

        // 1. ���� ���� ī�޶� �ְ�, 
        // 2. ������ ���� �ִ� ī�޶�� �ٸ��ٸ� (��, ī�޶� ��� �ٲ���ٸ�)
        if (currentActiveCamera != null && currentActiveCamera != lastActiveCamera)
        {
            // ��� ���� ī�޶� ���������� Ȱ��ȭ�� ī�޶�� ����մϴ�.
            lastActiveCamera = currentActiveCamera;

            // ��� ���� ī�޶�� ����� Region ID�� ã���ϴ�.
            string newRegionId = "";
            foreach (var link in cameraLinks)
            {
                if (link.camera == currentActiveCamera)
                {
                    newRegionId = link.regionId;
                    break;
                }
            }

            // LevelManager�� �����ϰ�, ã�� Region ID�� ������� �ʴٸ�
            if (LevelManager.Instance != null && !string.IsNullOrEmpty(newRegionId))
            {
                Debug.Log(newRegionId + " �������� ������Ʈ�� ��û�մϴ�.");
                // LevelManager���� Region ������ ��û�մϴ�.
                // affectCamera: false �ɼ��� ���� LevelManager�� ī�޶� ���� �������� �ʵ��� �մϴ�.
                LevelManager.Instance.SetCurrentRegion(newRegionId, affectCamera: false);
            }
        }
    }
}