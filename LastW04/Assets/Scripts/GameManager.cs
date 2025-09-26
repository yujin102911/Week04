using UnityEngine;
using UnityEngine.InputSystem;

public enum Mode//모드설정
{
    None,Editing
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()//싱글톤
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 살아남음
        }
        else
        {
            Destroy(gameObject); // 이미 있으면 중복 방지
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
