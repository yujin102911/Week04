using UnityEngine;

public class HighLight : MonoBehaviour
{
    public Material defaultMaterial;  // 인스펙터에서 할당
    public Material highlightMaterial;  // 인스펙터에서 할당

    private SpriteRenderer sr;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ChangeMaterial()
    {
        sr.material = highlightMaterial; // Material 교체
    }    

    // Update is called once per frame
    void Update()
    {
        
    }
}
