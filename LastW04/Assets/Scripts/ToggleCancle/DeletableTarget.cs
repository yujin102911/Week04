using UnityEngine;

public class DeletableTarget : MonoBehaviour
{
    [Tooltip("필수 오브젝트면 삭제 불가")]
    [SerializeField] private bool essential = false;

    [Header("삭제 대상 (비워두면 자기 자신)")]
    [SerializeField] private GameObject deleteTarget;

    [Header("연출(선택)")]
    [SerializeField] private GameObject vfxOnDelete;
    [SerializeField] private AudioClip sfxOnDelete;

    public bool CanDelete => !essential;

    public void DeleteSelf()
    {
        if (!CanDelete) return;

        // 실제 지울 대상을 결정 (없으면 자기 자신)
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
