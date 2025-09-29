using UnityEngine;
using System.Collections;


public class TutoPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;
    [SerializeField] private GameObject panel3;


    private void Update()
    {
        string currentId = LevelManager.Instance.CurrentRegionId;

        if(currentId == "Region_07")
        {
            if (!panel1.activeInHierarchy)
            {
                panel1.SetActive(true);
                panel2.SetActive(false);
                panel3.SetActive(false);
            }
        }
        if(currentId == "Region_08")
        {
            if (!panel2.activeInHierarchy)
            {
                panel1.SetActive(false); panel2.SetActive(true); panel3.SetActive(false);
            }
        }
        if (currentId == "Region_09")
        {
            if (!panel3.activeInHierarchy)
            {
                panel1.SetActive(false) ; panel2.SetActive(false); panel3.SetActive(true);
            }
        }


    }
    
}
