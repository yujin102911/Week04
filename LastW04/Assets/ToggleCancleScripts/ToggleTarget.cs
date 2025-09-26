using UnityEngine;
using System.Collections;

public class ToggleTarget : MonoBehaviour
{
    [Header("Target (끄고/켜고/상태 바꿀 대상)")]
    [SerializeField] private GameObject target;

    [Header("Optional Components (스프라이트/콜라이더 전환용)")]
    [SerializeField] private Collider2D solid;      // 충돌용(클릭용 부모 콜라이더를 여기에 넣지 말 것)
    [SerializeField] private SpriteRenderer sr;

    [Header("Sprites (문 등 상태 전환용)")]
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;

    [Header("State")]
    [SerializeField] private bool isOn = true;

    [Header("On/Off Mode")]
    [Tooltip("오브젝트 자체를 껐다 켜고 싶으면 true (이때 target.SetActive 또는 대체 로직 사용)")]
    [SerializeField] private bool toggleGameObject = true; // 스파이크 용도 기본 true 추천

    [Header("Options")]
    [Tooltip("열릴 때 solid 콜라이더를 꺼도 되는지 (클릭용은 절대 넣지 말 것)")]
    [SerializeField] private bool disableSolidOnOpen = false; // 클릭 토글 안정성 위해 기본 false


    public bool IsOn => isOn;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void Toggle() => SetState(!isOn);

    public void SetState(bool on)
    {

        isOn = on;

        // 1) 대상 오브젝트 자체를 껐다 켜는 모드
        if (toggleGameObject)
        {
            if (target != gameObject)
            {
                if (target.activeSelf != isOn)
                    target.SetActive(isOn);

                Physics2D.SyncTransforms();
                return;
            }

            Physics2D.SyncTransforms();
            return;
        }


        if (solid && disableSolidOnOpen)
            solid.enabled = !isOn; // 열리면 충돌 off (클릭용 콜라이더를 여기 연결하지 말 것)

        if (sr)
            sr.sprite = isOn ? openSprite : closedSprite;

        Physics2D.SyncTransforms();
    }

    private void Start()
    {
        SetState(isOn); // 초기 상태 반영(armed=false라면 실제 토글은 arm 후부터 적용)
    }
}
