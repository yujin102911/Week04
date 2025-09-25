using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class Button : MonoBehaviour
{
    public enum Mode { Toggle, Pulse }

    [Header("Mode & State")]
    [SerializeField] private Mode mode = Mode.Toggle;
    [SerializeField] private bool isOn = false;          // Toggle 모드일 때 유지 상태
    [SerializeField, Min(0f)] private float pulseSeconds = 0.15f; // Pulse 지속

    [Header("Cooldown")]
    [SerializeField, Min(0f)] private float pressCooldown = 0.15f;

    [Header("Player Filter")]
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private LayerMask includeLayers = ~0;
    [SerializeField] private bool requireOverlapForInput = true; // 트리거 안에서만 상호작용 허용

    [Header("Input (New Input System)")]
    [Tooltip("플레이어 상호작용(예: E키, Gamepad South) 액션을 연결")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Events")]
    public UnityEvent OnPressed;              // 버튼이 눌렸을 때(두 모드 공통)
    public UnityEvent OnReleased;             // Pulse 끝날 때
    public UnityEvent<bool> OnStateChanged;   // Toggle 모드에서 상태 변경(true=On)
    public UnityEvent OnTurnedOn;             // 상태가 On이 됐을 때
    public UnityEvent OnTurnedOff;            // 상태가 Off가 됐을 때

    // 내부
    private Collider2D _col;
    private readonly HashSet<Collider2D> _inside = new();
    private int _includeMask;
    private float _nextPressTime = 0f;
    private Coroutine _pulseCo;

    void Reset()
    {
        _col = GetComponent<Collider2D>();
        if (_col) _col.isTrigger = true;
    }

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col && !_col.isTrigger) _col.isTrigger = true;
        _includeMask = includeLayers.value;
    }

    void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteractPerformed;
            if (!interactAction.action.enabled) interactAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.performed -= OnInteractPerformed;
        _inside.Clear();
    }

    // ───────── Trigger 판정 ─────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsEligible(other)) return;
        _inside.Add(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!IsEligible(other)) return;
        _inside.Add(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _inside.Remove(other);
    }

    private bool IsEligible(Collider2D other)
    {
        var go = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (((1 << go.layer) & _includeMask) == 0) return false;
        if (!string.IsNullOrEmpty(requiredTag) && !go.CompareTag(requiredTag)) return false;
        return true;
    }

    private bool HasAnyEligibleInside() => _inside.Count > 0;

    // ───────── Input 콜백(신형) ─────────
    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        TryPressByPlayer();
    }

    private void TryPressByPlayer()
    {
        if (requireOverlapForInput && !HasAnyEligibleInside()) return;
        TryPress();
    }

    // ───────── 공통 프레스 로직 ─────────
    private void TryPress()
    {
        if (Time.unscaledTime < _nextPressTime) return;
        _nextPressTime = Time.unscaledTime + pressCooldown;

        OnPressed?.Invoke();

        if (mode == Mode.Toggle)
        {
            SetState(!isOn);
        }
        else // Pulse
        {
            if (_pulseCo != null) StopCoroutine(_pulseCo);
            _pulseCo = StartCoroutine(CoPulse());
        }
    }

    private IEnumerator CoPulse()
    {
        // Pulse 동안 On처럼 동작하고 싶으면 임시로 isOn=true로 둘 수도 있음(필요시 주석 해제)
        // bool prev = isOn; isOn = true; OnStateChanged?.Invoke(isOn);

        yield return new WaitForSeconds(pulseSeconds);

        OnReleased?.Invoke();

        // isOn = prev; OnStateChanged?.Invoke(isOn);
    }

    // ───────── 상태 제어 API (UI에서 직접 호출) ─────────
    public void UI_Press() => TryPress();                 // UI Button.onClick → 이 함수 연결
    public void UI_ToggleChanged(bool value)              // UI Toggle.onValueChanged(bool) → 이 함수 연결
    {
        if (mode == Mode.Toggle)
        {
            SetState(value);
        }
        else
        {
            // Pulse 모드에서는 true로 바뀌는 순간만 누른 것으로 처리
            if (value) TryPress();
        }
    }

    public void SetState(bool on) // 외부 스크립트/Timeline 등에서 직접 제어 가능
    {
        if (isOn == on) return;
        isOn = on;
        OnStateChanged?.Invoke(isOn);
        if (isOn) OnTurnedOn?.Invoke();
        else OnTurnedOff?.Invoke();
    }

    // 선택: 현재 상태 읽기용
    public bool IsOn => isOn;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!enabled) return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        var c = GetComponent<Collider2D>();
        if (!c) return;
        // 단순 시각화(콜라이더 타입별로 다르게 그리려면 확장)
        Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
    }
#endif
}
