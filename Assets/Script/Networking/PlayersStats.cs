using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayersStats : NetworkBehaviour
{
    [SyncVar] public int team;
    void Start()
    {
        if (!isLocalPlayer)
        {
            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                image.enabled = false;
            }

            Button[] buttons = GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                button.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
