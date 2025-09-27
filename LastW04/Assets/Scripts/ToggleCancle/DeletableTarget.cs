using UnityEngine;

public class DeletableTarget : MonoBehaviour
{
    [Tooltip("�ʼ� ������Ʈ�� ���� �Ұ�")]
    [SerializeField] private bool essential = false;

    [Header("���� ��� (����θ� �ڱ� �ڽ�)")]
    [SerializeField] private GameObject deleteTarget;

    [Header("����(����)")]
    [SerializeField] private GameObject vfxOnDelete;
    [SerializeField] private AudioClip sfxOnDelete;

    public bool CanDelete => !essential;

    public void DeleteSelf()
    {
        if (!CanDelete) return;

        // ���� ���� ����� ���� (������ �ڱ� �ڽ�)
        GameObject target = deleteTarget ? deleteTarget : gameObject;

        if (vfxOnDelete)
            Instantiate(vfxOnDelete, target.transform.position, Quaternion.identity);

        if (sfxOnDelete)
        {
            var tmp = new GameObject("SFX_Delete");
            var src = tmp.AddComponent<AudioSource>();
            src.PlayOneShot(sfxOnDelete);
            Destroy(tmp, sfxOnDelete.length);
        }

        Destroy(target);
    }
}
