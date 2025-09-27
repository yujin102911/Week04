using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelClearChecker : MonoBehaviour
{
    public bool autoLogClear = true; // ���� ���� �� Debug.Log ���

    void OnEnable()
    {
        Statue.OnAnyStatueStateChanged += CheckClear;
    }
    void OnDisable()
    {
        Statue.OnAnyStatueStateChanged -= CheckClear;
    }

    void Start()
    {
        // �ʱ� ���� �� 1ȸ ����
        Statue.ReevaluateAll();
        CheckClear();
    }

    void CheckClear()
    {
        // �� �� ��� Statue�� �����̸� Ŭ����
        var all = Object.FindObjectsByType<Statue>(FindObjectsSortMode.None); // �� ����
        bool allOk = all.Length > 0 && all.All(s => s.IsSatisfied);

        if (allOk && autoLogClear)
        {
            Debug.Log("[MiniPuzzle] CLEAR: ��� ������ ���� �����Դϴ�.");
            // ���⼭ UI ���/�� ���� �� ���ϴ� �׼� ����
        }
    }
}
