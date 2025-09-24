using UnityEngine;

public enum Mode
{
    None,Editing
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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
        if (Mode.Editing == Mode.Editing)
        {

        }
    }
}
