using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourAnimal : MonoBehaviour
{
    public PanelManager PanelManager;
    public int id;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MainDeleteNotice()
    {
        PanelManager.DeleteButton(id);
        Destroy(gameObject);
    }
    public void ShowApplicantButton()
    {
        PanelManager.MainShowApplicant(id);
    }
}
