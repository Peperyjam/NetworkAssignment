using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] Maze maze;

    public GameObject PowerUpObj;

    public GameObject SpawnAtNode(GameObject template,int y, int x)
    {
        GameObject spawnedObj = Instantiate(template, maze.maze[y,x].body.transform);
        NetworkObject powrUpNetworked = spawnedObj.GetComponent<NetworkObject>();
        powrUpNetworked.Spawn();
        
        

        return spawnedObj;
    }

}
