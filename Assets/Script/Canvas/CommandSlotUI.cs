using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlotUI : MonoBehaviour
{
    [SerializeField] public GameObject commandPannel;
    [SerializeField] private Button myButton;
    [SerializeField, Tooltip("GameObject that will execute the command corresponding to the ability")] public GameObject commandLinkerObject;
    public UnityEngine.UI.Image imageComponent;

    private CommandLinkerUI commandLinker;



    void Start()
    {
        commandLinkerObject = Instantiate(commandLinkerObject,transform);
        commandLinker = commandLinkerObject.GetComponent<CommandLinkerUI>();

        imageComponent.sprite = commandLinker.sprite;
        myButton.onClick.AddListener(commandLinker.useCommand);
    }
}
