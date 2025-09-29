using System.Linq;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    [SerializeField] Material defaultMaterial;  // 인스펙터에서 할당
    [SerializeField] Material highlightMaterial;  // 인스펙터에서 할당Grid
    [SerializeField]
    SelectedUI[] typeUI;//내 UI 타입

    private SpriteRenderer sr;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (typeUI.Contains(GameManager.selectedUI))
        {
            sr.material = highlightMaterial;
        }else
        {
            sr.material = defaultMaterial;
        }
    }
}
