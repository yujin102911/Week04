using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public Transform snap;          // 가젯이 붙을 위치/회전 기준
    [HideInInspector] public bool occupied; // 이미 가젯이 붙었는지 여부
}
