using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Stats : NetworkBehaviour
{
    public static Stats Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public NetworkVariable<int> playerNum = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOnePower = new NetworkVariable<int>();
    public NetworkVariable<int> playerTwoPower = new NetworkVariable<int>();

    [SerializeField] TextMeshProUGUI p1Text;
    [SerializeField] TextMeshProUGUI p2Text;
    [SerializeField] GameObject endPanel;
    [SerializeField] TextMeshProUGUI winnerText;
    private void Start()
    {
        playerOnePower.OnValueChanged += PlayerOnePowerChanged;
        playerTwoPower.OnValueChanged += PlayerTwoPowerChanged;

        UpdateScoreRpc();
    }
    private void PlayerOnePowerChanged(int oldValue, int newValue)
    {
        UpdateScoreRpc();
    }

    private void PlayerTwoPowerChanged(int oldValue, int newValue)
    {
        UpdateScoreRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateScoreRpc()
    {
        p1Text.text = "Player 1 Score: " + playerOnePower.Value;
        p2Text.text = "Player 2 Score: " + playerTwoPower.Value;
        Debug.Log("player1: " + playerOnePower.Value);
        Debug.Log("player2: " + playerTwoPower.Value);
    }
    [ServerRpc]
    public void SetScoreServerRpc (int playerNum)
    {
        if (playerNum == 1) playerOnePower.Value++;
        if (playerNum == 2) playerTwoPower.Value++;
        UpdateScoreRpc();

    }
    [Rpc(SendTo.Everyone)]
    public void WinnerRpc(int winnerNum)
    {
        winnerText.text = "Winner: player " + winnerNum;
        endPanel.SetActive(true);
    }
}
