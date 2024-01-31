using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageBox : MonoBehaviour
{
    public GameObject box;
    // Start is called before the first frame update
    void Awake()
    {
        box = gameObject;
    }

    public void CloseErrorBox()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
