using UnityEngine;
using System.Collections;
using UnityEngine.Events; // ★ 추가

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

    [Header("Events")]
    [Tooltip("토글 상태가 바뀔 때 bool(isOn)을 함께 전달")]
    public UnityEvent<bool> onStateChanged; // ★ 추가

    public bool IsOn => isOn;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void Toggle() => SetState(!isOn);

    public void SetState(bool on)
    {
        if (isOn == on)
        {
            // 그래도 외부에서 강제로 갱신을 기대할 수 있으니 최소한 비주얼/콜라이더/이벤트는 한번 쏴준다
            ApplyLocalVisuals(on);
            onStateChanged?.Invoke(isOn);
            return;
        }

        isOn = on;

        // 1) 대상 오브젝트 자체를 껐다 켜는 모드
        if (toggleGameObject)
        {
            if (target != null && target != gameObject)
            {
                if (target.activeSelf != isOn)
                    target.SetActive(isOn);

                ApplyLocalVisuals(isOn);    // ★ 스프라이트/콜라이더 반영
                onStateChanged?.Invoke(isOn); // ★ 이벤트 발행
                Physics2D.SyncTransforms();
                return;
            }

            ApplyLocalVisuals(isOn);
            onStateChanged?.Invoke(isOn);
            Physics2D.SyncTransforms();
            return;
        }

        // 2) 게임오브젝트 on/off 대신 로컬 비주얼/콜라이더만 바꾸는 모드
        ApplyLocalVisuals(isOn);
        onStateChanged?.Invoke(isOn); // ★ 이벤트 발행
        Physics2D.SyncTransforms();
    }

    private void ApplyLocalVisuals(bool on)
    {
        if (solid && disableSolidOnOpen)
            solid.enabled = !on; // 열리면 충돌 off (클릭용 콜라이더를 여기 연결하지 말 것)

        if (sr)
            sr.sprite = on ? openSprite : closedSprite;
    }

    private void Start()
    {
        // 시작 시 현재 상태 반영 + 이벤트 한 번 발행(초기 상태와 연동되는 외부 오브젝트가 있을 수 있음)
        ApplyLocalVisuals(isOn);
        onStateChanged?.Invoke(isOn);
        Physics2D.SyncTransforms();
    }
}
