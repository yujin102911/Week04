using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Mode//모드설정
{
    None,Editing
}
public enum SelectedUI//모드설정
{
    None, Slide, Toggle, Delete
}
public class GameManager : MonoBehaviour
{
    [SerializeField]
    Mode defalutMode;
    public static SelectedUI selectedUI;
    [SerializeField] SelectedUI selectedUICheck;
    public static Mode mode;
    public static GameManager instance;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()//싱글톤
    {
        mode = defalutMode;
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
        selectedUICheck = selectedUI;
        if (Input.GetKeyUp(KeyCode.Tab)) //에딧모드 전환
        {
            if (GameManager.mode != Mode.Editing)
            {
                GameManager.mode = Mode.Editing;
            }
            else
            {
                GameManager.mode = Mode.None;
            }
        }
    }
}
