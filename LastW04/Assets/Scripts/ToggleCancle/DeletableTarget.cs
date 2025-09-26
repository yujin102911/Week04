using UnityEngine;

public class DeletableTarget : MonoBehaviour
{
    [Tooltip("�ʼ� ������Ʈ�� ���� �Ұ�")]
    [SerializeField] private bool essential = false;

    [Header("����(����)")]
    [SerializeField] private GameObject vfxOnDelete;
    [SerializeField] private AudioClip sfxOnDelete;

    public bool CanDelete => !essential;

    public void DeleteSelf()
    {
        if (!CanDelete) return;

        if (vfxOnDelete) Instantiate(vfxOnDelete, transform.position, Quaternion.identity);

        if (sfxOnDelete)
        {
            // ���� �����. ������Ʈ ����� ������ �°� �ٲ㵵 ��.
            var tmp = new GameObject("SFX_Delete");
            var src = tmp.AddComponent<AudioSource>();
            src.PlayOneShot(sfxOnDelete);
            Object.Destroy(tmp, sfxOnDelete.length);
        }

        Destroy(gameObject);
    }
}
