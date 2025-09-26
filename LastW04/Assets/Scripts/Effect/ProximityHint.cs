using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ProximityHint : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer hint;
    [SerializeField] private float radius = 1.5f;

    private void Awake()
    {
        if (hint) hint.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!player || !hint) return;
        bool near = Vector2.Distance(player.position, transform.position) <= radius;
        if (hint.gameObject.activeSelf != near) hint.gameObject.SetActive(near);
    }
}
