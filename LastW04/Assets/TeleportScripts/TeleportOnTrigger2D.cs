using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class TeleportOnTrigger2D : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private Transform target;

    [Header("Filter")]
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private LayerMask includeLayers = ~0;

    [Header("Behavior")]
    [SerializeField] private bool alignRotation = false;
    [SerializeField] private bool preserveVelocity = true;
    [SerializeField, Min(0f)] private float perObjectCooldown = 0.15f;

    private int _includeMask;
    private readonly Dictionary<int, float> _cooldownUntil = new();

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }
    void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col && !col.isTrigger) col.isTrigger = true;
        _includeMask = includeLayers.value;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsEligible(other) || !target) return;

        var go = CooldownKey(other);
        if (IsOnCooldown(go)) return;

        Teleport(other);
        StampCooldown(go);
    }

    private bool IsEligible(Collider2D other)
    {
        var go = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (((1 << go.layer) & _includeMask) == 0) return false;
        if (!string.IsNullOrEmpty(requiredTag) && !go.CompareTag(requiredTag)) return false;
        return true;
    }

    private static GameObject CooldownKey(Collider2D c)
    {
        var rb = c.attachedRigidbody;
        return rb ? rb.gameObject : c.gameObject;
    }

    private bool IsOnCooldown(GameObject go)
    {
        if (perObjectCooldown <= 0f) return false;
        return _cooldownUntil.TryGetValue(go.GetInstanceID(), out var until) && until > Time.unscaledTime;
    }
    private void StampCooldown(GameObject go)
    {
        if (perObjectCooldown <= 0f) return;
        _cooldownUntil[go.GetInstanceID()] = Time.unscaledTime + perObjectCooldown;
    }

    private void Teleport(Collider2D other)
    {
        Transform root = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform;

        var rb = other.attachedRigidbody;
        if (rb && !preserveVelocity)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
            rb.angularVelocity = 0f;
        }

        if (alignRotation)
            root.SetPositionAndRotation(target.position, target.rotation);
        else
            root.position = new Vector3(target.position.x, target.position.y, root.position.z);

        if (rb) rb.WakeUp();
        Physics2D.SyncTransforms();
    }
}
