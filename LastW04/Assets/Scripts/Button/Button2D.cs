using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class Button2D : MonoBehaviour
{
    [Header("Detect")]
    [Tooltip("이 레이어에 속한 오브젝트만 버튼을 누를 수 있음 (예: Player, Box)")]
    [SerializeField] private LayerMask detectionLayers;

    [Tooltip("이 값이 0이면 '누구든 올라오면' 활성화. 0보다 크면, 누르는 오브젝트들의 총 질량이 이 값 이상일 때만 활성화.")]
    [SerializeField, Min(0f)] private float minTotalMassToPress = 0f;

    [Header("Visual (optional: Sprite swap)")]
    [Tooltip("버튼의 스프라이트를 바꿔줄 대상(없으면 기능 비활성)")]
    [SerializeField] private SpriteRenderer sr;
    [Tooltip("눌렸을 때 표시할 스프라이트")]
    [SerializeField] private Sprite pressedSprite;
    [Tooltip("안 눌렸을 때 표시할 스프라이트")]
    [SerializeField] private Sprite unpressedSprite;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;
    [Tooltip("상태가 바뀔 때 true/false를 함께 보냄")]
    public UnityEvent<bool> onStateChanged;

    // 내부 상태
    private readonly HashSet<Rigidbody2D> occupants = new HashSet<Rigidbody2D>();
    private bool isPressed = false;

    private void Reset()
    {
        // 기본값: Player와 Box를 감지하도록 가정 (프로젝트 레이어에 맞춰 수정)
        detectionLayers = LayerMask.GetMask("Default", "Player", "Box");
        var rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        // 편의상 자동 할당 시도
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        // 혹시 누락되었으면 한 번 더 찾아줌(옵션)
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 안전: 비활성→활성시 초기화
        occupants.Clear();
        SetPressed(false, forceRefresh: true); // 초기 상태로 시각 동기화
    }

    private void OnDisable()
    {
        // 안전: 비활성화될 때 항상 해제 상태로
        occupants.Clear();
        SetPressed(false, forceRefresh: true);
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        if (occupants.Count == 0) return;

        // 살아있는 Rigidbody만 남기기
        bool changed = false;
        var toRemove = new List<Rigidbody2D>();
        foreach (var rb in occupants)
        {
            if (rb == null || !rb.gameObject.activeInHierarchy)
                toRemove.Add(rb);
        }
        if (toRemove.Count > 0)
        {
            foreach (var dead in toRemove) occupants.Remove(dead);
            changed = true;
        }
        if (changed) RecomputePressed();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!MatchesLayer(other.gameObject.layer)) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        // 같은 오브젝트의 여러 콜라이더로 인한 중복 방지: Rigidbody2D 단위로 관리
        if (occupants.Add(rb))
            RecomputePressed();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb == null) return;

        if (occupants.Remove(rb))
            RecomputePressed();
    }

    private bool MatchesLayer(int layer)
    {
        return (detectionLayers.value & (1 << layer)) != 0;
    }

    private void RecomputePressed()
    {
        bool nextPressed;
        if (minTotalMassToPress <= 0f)
        {
            nextPressed = occupants.Count > 0;
        }
        else
        {
            float totalMass = 0f;
            foreach (var rb in occupants)
            {
                if (rb != null) totalMass += rb.mass;
            }
            nextPressed = totalMass >= minTotalMassToPress;
        }
        SetPressed(nextPressed);
    }

    private void SetPressed(bool pressed, bool forceRefresh = false)
    {
        if (!forceRefresh && isPressed == pressed) return;
        isPressed = pressed;

        // 로그(원하면 끄셔도 됨)
        if (isPressed) Debug.Log($"{name} 버튼이 눌렸습니다!");
        else Debug.Log($"{name} 버튼에서 내려왔습니다!");

        // 스프라이트 갱신
        RefreshSprite();

        // 이벤트 발행
        onStateChanged?.Invoke(isPressed);
        if (isPressed) onPressed?.Invoke();
        else onReleased?.Invoke();
    }

    private void RefreshSprite()
    {
        if (isPressed)
        {
            if (pressedSprite) sr.sprite = pressedSprite;
        }
        else
        {
            if (unpressedSprite) sr.sprite = unpressedSprite;
        }
    }

    // 디버그 표시(선택)
    private void OnDrawGizmosSelected()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = isPressed ? Color.red : Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (col is BoxCollider2D b)
        {
            Gizmos.DrawWireCube(b.offset, b.size);
        }
    }
}
