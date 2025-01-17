using Unity.Netcode;
using UnityEngine;

public class Emote : NetworkBehaviour
{
    float Timer = 0;

    private void Update()
    {
        Timer += Time.deltaTime;

        if (Timer >= 1.5f)
        {
            if(IsServer) GetComponent<NetworkObject>().Despawn();
        }
    }
}
