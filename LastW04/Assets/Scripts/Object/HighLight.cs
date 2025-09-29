using System.Linq;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    [SerializeField] Material defaultMaterial;  // 인스펙터에서 할당
    [SerializeField] Material highlightMaterial;  // 인스펙터에서 할당Grid
    [SerializeField] SelectedUI[] typeUI;//내 UI 타입
    Material defaultMaterialNew;  // 인스펙터에서 할당
    Material highlightMaterialNew;  // 인스펙터에서 할당Grid
    SelectedUI typeUIBefore;//기존 UI 타입
    private SpriteRenderer sr;
    [SerializeField] Material myMaterial;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        myMaterial = GetComponent<SpriteRenderer>().material;
        defaultMaterialNew = defaultMaterial;
        highlightMaterialNew = highlightMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.selectedUI!= typeUIBefore)//상태가 바꼇을때
        {
            if (typeUI.Contains(GameManager.selectedUI))
            {
                myMaterial.SetFloat("Thickness", 0.1f);
            }
            else
            {
                myMaterial.SetFloat("Thickness", 0f);
            }
            typeUIBefore = GameManager.selectedUI;//저장된 UI
        }
    }
}
