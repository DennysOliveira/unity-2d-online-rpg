using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    AudioSource audioSource;
    AudioListener audioListener;
    

    public override void Start()
    {
        base.Start();
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.Play();

        audioListener = GetComponentInChildren<AudioListener>();
        

    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        audioSource.Stop();
        audioListener.enabled = false;

        

    }
}
