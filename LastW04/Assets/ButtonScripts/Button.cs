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
    [SerializeField] private bool isOn = false;          // Toggle ����� �� ���� ����
    [SerializeField, Min(0f)] private float pulseSeconds = 0.15f; // Pulse ����

    [Header("Cooldown")]
    [SerializeField, Min(0f)] private float pressCooldown = 0.15f;

    [Header("Player Filter")]
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private LayerMask includeLayers = ~0;
    [SerializeField] private bool requireOverlapForInput = true; // Ʈ���� �ȿ����� ��ȣ�ۿ� ���

    [Header("Input (New Input System)")]
    [Tooltip("�÷��̾� ��ȣ�ۿ�(��: EŰ, Gamepad South) �׼��� ����")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Events")]
    public UnityEvent OnPressed;              // ��ư�� ������ ��(�� ��� ����)
    public UnityEvent OnReleased;             // Pulse ���� ��
    public UnityEvent<bool> OnStateChanged;   // Toggle ��忡�� ���� ����(true=On)
    public UnityEvent OnTurnedOn;             // ���°� On�� ���� ��
    public UnityEvent OnTurnedOff;            // ���°� Off�� ���� ��

    // ����
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

    // ������������������ Trigger ���� ������������������
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

    // ������������������ Input �ݹ�(����) ������������������
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

    // ������������������ ���� ������ ���� ������������������
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
        // Pulse ���� Onó�� �����ϰ� ������ �ӽ÷� isOn=true�� �� ���� ����(�ʿ�� �ּ� ����)
        // bool prev = isOn; isOn = true; OnStateChanged?.Invoke(isOn);

        yield return new WaitForSeconds(pulseSeconds);

        OnReleased?.Invoke();

        // isOn = prev; OnStateChanged?.Invoke(isOn);
    }

    // ������������������ ���� ���� API (UI���� ���� ȣ��) ������������������
    public void UI_Press() => TryPress();                 // UI Button.onClick �� �� �Լ� ����
    public void UI_ToggleChanged(bool value)              // UI Toggle.onValueChanged(bool) �� �� �Լ� ����
    {
        if (mode == Mode.Toggle)
        {
            SetState(value);
        }
        else
        {
            // Pulse ��忡���� true�� �ٲ�� ������ ���� ������ ó��
            if (value) TryPress();
        }
    }

    public void SetState(bool on) // �ܺ� ��ũ��Ʈ/Timeline ��� ���� ���� ����
    {
        if (isOn == on) return;
        isOn = on;
        OnStateChanged?.Invoke(isOn);
        if (isOn) OnTurnedOn?.Invoke();
        else OnTurnedOff?.Invoke();
    }

    // ����: ���� ���� �б��
    public bool IsOn => isOn;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!enabled) return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        var c = GetComponent<Collider2D>();
        if (!c) return;
        // �ܼ� �ð�ȭ(�ݶ��̴� Ÿ�Ժ��� �ٸ��� �׸����� Ȯ��)
        Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
    }
#endif
}
