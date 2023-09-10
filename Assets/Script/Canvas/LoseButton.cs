using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseButton : MonoBehaviour
{
    [SerializeField] private GameObject loseUI;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void hideLoseScreen(){
        loseUI.SetActive(false);
    }
    
}
