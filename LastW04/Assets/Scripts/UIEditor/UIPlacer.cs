using UnityEngine;
using UnityEngine.UI;

public class UIPlacer : MonoBehaviour
{
    public SelectedUI typeUI;//내 UI 타입
    public bool placed;
    public bool spawned=true;//생성 초기상태
    [SerializeField]
    SpriteRenderer sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.mode == Mode.Editing)
        {
            if (!spawned)//막 생성된 상태가 아니면
            {
                placed = true;
                sprite.color = Color.gray;//비활성 색
            }
        }
        else
        {
            spawned = false;//기본 모드로 갔으면 생성초기 아니제~
            sprite.color = Color.white; 
        }
    }
}
