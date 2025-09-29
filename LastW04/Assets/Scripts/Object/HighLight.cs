using System.Linq;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    [SerializeField] SelectedUI[] typeUI;//내 UI 타입
    [SerializeField] SelectedUI typeUIBefore;//기존 UI 타입
    [SerializeField] float thicknessValue=0.1f;
    private SpriteRenderer sr;
    Material myMaterial;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        myMaterial = GetComponent<SpriteRenderer>().material;
        typeUIBefore=SelectedUI.None;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.selectedUI!= typeUIBefore)//상태가 바꼇을때
        {
            if (typeUI.Contains(GameManager.selectedUI))
            {
                myMaterial.SetFloat("_Thickness", thicknessValue);
                myMaterial.SetColor("_Color", new Color (1f,1f,1f));
            }
            else
            {
                myMaterial.SetFloat("_Thickness", 0f);
                myMaterial.SetColor("_Color", new Color(0f, 0f, 0f));
            }
            typeUIBefore = GameManager.selectedUI;//저장된 UI
        }
    }
}
