using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public int playerNum;
    [SerializeField] GameObject Heart;
    [SerializeField] GameObject Broken;
    public override void OnNetworkSpawn()
    {
        playerNum = Stats.Instance.playerNum.Value + 1;
        if (IsOwner) IncreaseplayerNumServerRpc();

    }

    [ServerRpc]
    void CollectServerRpc(ulong networkId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkId].Despawn();
        Stats.Instance.SetScoreServerRpc(playerNum);

    }
    [ServerRpc]
    void IncreaseplayerNumServerRpc()
    {
        Stats.Instance.playerNum.Value++;
    }
    void PlayerColide()
    {
        if (IsServer)
        {
            if (Stats.Instance.playerOnePower.Value > Stats.Instance.playerTwoPower.Value)
            {
                Stats.Instance.WinnerRpc(1);
            }
            else if (Stats.Instance.playerTwoPower.Value > Stats.Instance.playerOnePower.Value)
            {
                Stats.Instance.WinnerRpc(2);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "PowerUp(Clone)")
        {
            NetworkObject powerUpNetwork = collision.gameObject.GetComponent<NetworkObject>();
            CollectServerRpc(powerUpNetwork.NetworkObjectId);
        }

        if (collision.gameObject.name == "Player(Clone)")
        {
            PlayerColide();
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            EmoteServerRpc(true, transform.position);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            EmoteServerRpc(false, transform.position);
        }

    }
    [ServerRpc(RequireOwnership =false)]
    void EmoteServerRpc(bool goodOrBad, Vector2 playerPos, ServerRpcParams serverRpcParams = default)
    {
        ulong Id = serverRpcParams.Receive.SenderClientId;
        if (goodOrBad)
        {
            GameObject spawnedEmote = Instantiate(Heart, NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(Id).transform.position, transform.rotation);
            NetworkObject spawnedNet = spawnedEmote.GetComponent<NetworkObject>();
            spawnedNet.SpawnWithOwnership(Id);
        }
        if (!goodOrBad)
        {
            GameObject spawnedEmote = Instantiate(Broken, NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(Id).transform.position, transform.rotation);
            NetworkObject spawnedNet = spawnedEmote.GetComponent<NetworkObject>();
            spawnedNet.SpawnWithOwnership(Id);
        }
    }
}
