using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // PlayerInput (있으면 자동 감지)

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class PathMiniController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform player;
    [SerializeField] Transform respawnPoint;
    [SerializeField] GameObject overlayMap;

    [Header("Light State")]
    [SerializeField] bool isLightOn = true;

    [Header("Blink/Respawn")]
    [SerializeField] float blinkInterval = 0.12f;
    [SerializeField] int blinkCount = 8;
    [SerializeField] float retriggerCooldown = 0.5f;

    [Header("Optional: 움직임 관련 컴포넌트(블링크 동안 비활성)")]
    [SerializeField] MonoBehaviour[] movementComponentsToDisable;

    float _lastTriggerTime = -999f;
    bool _respawning;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }


    void OnEnable() => ApplyLightState();
    void OnValidate()
    {
        // 인스펙터에서 값만 갱신하고, 실제 SetActive는 런타임에서만 수행
        // 아무 것도 하지 않음, 또는 에디터용 표시만 갱신
    }

    // --- Light ---
    public bool IsLightOn => isLightOn;
    public void ToggleLight() => SetLight(!isLightOn);
    public void SetLight(bool on)
    {
        isLightOn = on;
        ApplyLightState();
    }
    void ApplyLightState()
    {
        if (overlayMap) overlayMap.SetActive(!isLightOn); // 밝음=비활성, 어두움=활성
    }

    // --- Wrong path trigger ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_respawning || !player || !respawnPoint) return;
        if (other.transform == player || other.transform.IsChildOf(player))
        {
            if (Time.time >= _lastTriggerTime + retriggerCooldown)
            {
                _lastTriggerTime = Time.time;
                StartCoroutine(BlinkThenRespawn());
            }
        }
    }

    IEnumerator BlinkThenRespawn()
    {
        _respawning = true;

        // 1) 이동 정지(플레이어 코드 건드리지 않음)
        var rb = player.GetComponent<Rigidbody2D>();
        Vector2 prevVel = default;
        RigidbodyConstraints2D prevConstraints = default;
        if (rb)
        {
            prevVel = rb.linearVelocity;
            prevConstraints = rb.constraints;
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX
                           | RigidbodyConstraints2D.FreezePositionY
                           | RigidbodyConstraints2D.FreezeRotation;
        }

        // PlayerInput 자동 감지(있으면 끔)
        var playerInput = player.GetComponent<PlayerInput>();
        bool inputWasEnabled = false;
        if (playerInput)
        {
            inputWasEnabled = playerInput.enabled;
            playerInput.enabled = false;
        }

        // 임의로 지정한 컴포넌트들도 비활성
        var disabledFlags = new bool[movementComponentsToDisable.Length];
        for (int i = 0; i < movementComponentsToDisable.Length; i++)
        {
            var comp = movementComponentsToDisable[i];
            if (!comp) continue;
            disabledFlags[i] = comp.enabled;
            comp.enabled = false;
        }

        // 2) 깜빡임
        var rends = new List<Renderer>();
        player.GetComponentsInChildren(true, rends);

        for (int i = 0; i < blinkCount; i++)
        {
            SetRenderersEnabled(rends, false);
            yield return new WaitForSeconds(blinkInterval);
            SetRenderersEnabled(rends, true);
            yield return new WaitForSeconds(blinkInterval);
        }

        // 3) 리스폰
        player.position = respawnPoint.position;

        // 4) 이동 정지 해제/복원
        for (int i = 0; i < movementComponentsToDisable.Length; i++)
        {
            var comp = movementComponentsToDisable[i];
            if (!comp) continue;
            comp.enabled = disabledFlags[i];
        }

        if (playerInput) playerInput.enabled = inputWasEnabled;

        if (rb)
        {
            rb.constraints = prevConstraints; // 원래 제약 복원
            rb.linearVelocity = prevVel;           // 원래 속도 복원(원치 않으면 0 유지)
        }

        _respawning = false;
    }

    void SetRenderersEnabled(List<Renderer> list, bool enabled)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i]) list[i].enabled = enabled;
        }
    }
}
