using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartSettings : NetworkBehaviour
{
    [SerializeField] Slider widthSlider;
    [SerializeField] Slider heightSlider;
    [SerializeField] Slider dificultySlider;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] TextMeshProUGUI textWidth;
    [SerializeField] TextMeshProUGUI textHeight;
    [SerializeField] Spawner spawner;
    [SerializeField] GameObject Hud;
    [SerializeField] GameObject Hosting;
    [SerializeField] GameObject Clienting;
    void Start()
    {
        UpdateWidthText();
        UpdateHeightText();
        settingsMenu.SetActive(false);
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        settingsMenu.SetActive(true);
        Hosting.SetActive(true);
    }
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
        Clienting.SetActive(true);
    }
    public void StartButton()
    {

        Maze.Instance.width = (int)widthSlider.value;
        Maze.Instance.height = (int)heightSlider.value;
        Maze.Instance.dificulty = 5 + dificultySlider.value;
        Maze.Instance.CreateGridRpc(Maze.Instance.width, Maze.Instance.height);
        Maze.Instance.Backtrack();
        SpawnPowerUps();

        SpawnPlayerRpc();

        RemoveMenuRpc();
    }
    public void UpdateWidthText()
    {
        textWidth.text = widthSlider.value.ToString();
    }
    public void UpdateHeightText()
    {
        textHeight.text = heightSlider.value.ToString();
    }
    void SpawnPowerUps()
    {
        int times = (Maze.Instance.width * Maze.Instance.height) / 20;
        while (times > 0)
        {
            spawner.SpawnAtNode(spawner.PowerUpObj, Random.Range(0, Maze.Instance.height), Random.Range(0, Maze.Instance.width));
            times--;
        }
    }
    [Rpc(SendTo.Everyone)]
    void RemoveMenuRpc()
    {
        this.gameObject.SetActive(false);
        Hud.SetActive(true);
    }
    [Rpc(SendTo.Everyone)]
    void SpawnPlayerRpc()
    {
        NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.position = Maze.Instance.maze[Random.Range(0, Maze.Instance.height), Random.Range(0, Maze.Instance.width)].body.transform.position; ;
    }
}
