using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class Button2D : MonoBehaviour
{
    [Header("Detect")]
    [Tooltip("�� ���̾ ���� ������Ʈ�� ��ư�� ���� �� ���� (��: Player, Box)")]
    [SerializeField] private LayerMask detectionLayers;

    [Tooltip("�� ���� 0�̸� '������ �ö����' Ȱ��ȭ. 0���� ũ��, ������ ������Ʈ���� �� ������ �� �� �̻��� ���� Ȱ��ȭ.")]
    [SerializeField, Min(0f)] private float minTotalMassToPress = 0f;

    [Header("Visual (optional: Sprite swap)")]
    [Tooltip("��ư�� ��������Ʈ�� �ٲ��� ���(������ ��� ��Ȱ��)")]
    [SerializeField] private SpriteRenderer sr;
    [Tooltip("������ �� ǥ���� ��������Ʈ")]
    [SerializeField] private Sprite pressedSprite;
    [Tooltip("�� ������ �� ǥ���� ��������Ʈ")]
    [SerializeField] private Sprite unpressedSprite;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;
    [Tooltip("���°� �ٲ� �� true/false�� �Բ� ����")]
    public UnityEvent<bool> onStateChanged;

    // ���� ����
    private readonly HashSet<Rigidbody2D> occupants = new HashSet<Rigidbody2D>();
    private bool isPressed = false;

    private void Reset()
    {
        // �⺻��: Player�� Box�� �����ϵ��� ���� (������Ʈ ���̾ ���� ����)
        detectionLayers = LayerMask.GetMask("Default", "Player", "Box");
        var rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        // ���ǻ� �ڵ� �Ҵ� �õ�
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        // Ȥ�� �����Ǿ����� �� �� �� ã����(�ɼ�)
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // ����: ��Ȱ����Ȱ���� �ʱ�ȭ
        occupants.Clear();
        SetPressed(false, forceRefresh: true); // �ʱ� ���·� �ð� ����ȭ
    }

    private void OnDisable()
    {
        // ����: ��Ȱ��ȭ�� �� �׻� ���� ���·�
        occupants.Clear();
        SetPressed(false, forceRefresh: true);
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        if (occupants.Count == 0) return;

        // ����ִ� Rigidbody�� �����
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

        // ���� ������Ʈ�� ���� �ݶ��̴��� ���� �ߺ� ����: Rigidbody2D ������ ����
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

        // �α�(���ϸ� ���ŵ� ��)
        if (isPressed) Debug.Log($"{name} ��ư�� ���Ƚ��ϴ�!");
        else Debug.Log($"{name} ��ư���� �����Խ��ϴ�!");

        // ��������Ʈ ����
        RefreshSprite();

        // �̺�Ʈ ����
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

    // ����� ǥ��(����)
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
