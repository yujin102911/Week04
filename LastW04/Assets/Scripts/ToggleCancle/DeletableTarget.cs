using UnityEngine;

public class DeletableTarget : MonoBehaviour
{
    [Tooltip("필수 오브젝트면 삭제 불가")]
    [SerializeField] private bool essential = false;

    [Header("연출(선택)")]
    [SerializeField] private GameObject vfxOnDelete;
    [SerializeField] private AudioClip sfxOnDelete;

    public bool CanDelete => !essential;

    public void DeleteSelf()
    {
        if (!CanDelete) return;

        if (vfxOnDelete) Instantiate(vfxOnDelete, transform.position, Quaternion.identity);

        if (sfxOnDelete)
        {
            // 간단 재생용. 프로젝트 오디오 구조에 맞게 바꿔도 됨.
            var tmp = new GameObject("SFX_Delete");
            var src = tmp.AddComponent<AudioSource>();
            src.PlayOneShot(sfxOnDelete);
            Object.Destroy(tmp, sfxOnDelete.length);
        }

        Destroy(gameObject);
    }
}
