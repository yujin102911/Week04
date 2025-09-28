// SceneBindings.cs
using UnityEngine;

[DisallowMultipleComponent]
public class SceneBindings : MonoBehaviour
{
    [Header("Scene-scoped refs")]
    public Transform player;

    [Header("UI Drag Managers")]
    public UIDragManager sliderUI;
    public UIDragManager toggleUI;
    public UIDragManager deleteUI;

    [Header("Level UI Limits (index = level number)")]
    public int[] levelUISlider;
    public int[] levelUIToggle;
    public int[] levelUIDelete;
}
