using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float moveSpeed = 5f;

    void Start()
    {
        
    }

    void Update()
    {

        PlayerMoveInput();
    }


    void PlayerMoveInput()
    {
        Vector3 moveToVertical = new Vector3(moveSpeed * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= moveToVertical;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += moveToVertical;
        }

        Vector3 moveToHorizontal = new Vector3(0, moveSpeed * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += moveToHorizontal;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= moveToHorizontal;
        }
    }
}
