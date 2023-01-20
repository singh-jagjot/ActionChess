using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
public class GameInfo : MonoBehaviour
{
    public TextMeshProUGUI winMsg;
    // public GameObject chessBoard;

    public GameObject relayOb;
    public GameObject relayScreen;
    public TextMeshProUGUI codedisp;
    public TMP_InputField inpcode;
    private string msg;

    void LateUpdate()
    {
        winMsg.text = this.msg;
        codedisp.text = this.joincode;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMsg(string msg){
        this.msg = msg;
    }

    public void gameOver()
    {
        Transform gameOverPanelTransform = transform.Find("InfoPanel");
        gameOverPanelTransform.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Time.timeScale = 0f;
    }

    public void startGame(){
        GameObject.Find("NetworkManager").SetActive(false);
        var cb = GameObject.Find("ChessBoardMainNetwork");
        foreach (var item in cb.GetComponent<NetworkBoard>().GetPieceDict())
        {
            if(item.Value!=null)
                item.Value.SetActive(false);
        }
        cb.SetActive(false);
        GameObject.Destroy(cb);
        transform.Find("InfoPanel").gameObject.SetActive(false);
        Transform sp = transform.Find("StartPanel");
        // Cursor.lockState = CursorLockMode.Locked;
        // Time.timeScale = 1f;
        sp.gameObject.SetActive(false);
    }

    public void startMpGame(){
         var cb = GameObject.Find("ChessBoardMain");
        foreach (var item in cb.GetComponent<Board>().GetPieceDict())
        {
            if(item.Value!=null)
                item.Value.SetActive(false);
        }
        cb.SetActive(false);
        GameObject.Destroy(cb);
        transform.Find("InfoPanel").gameObject.SetActive(false);
        Transform sp = transform.Find("StartPanel");
        // Cursor.lockState = CursorLockMode.Locked;
        // Time.timeScale = 1f;
        sp.gameObject.SetActive(false);
        relayScreen.SetActive(true);
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Time.timeScale = 1f;

    }

       private string joincode;
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            joincode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            // Debug.Log("JoinCode: " + joincode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

    }

    private async void JoinRelay(string code)
    {
        try
        {
            Debug.Log("Joing Relay with code: " + code);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void CreateRelayCode(){
        CreateRelay();
    }

    public void JoinRelayWithCode(){
        if(inpcode.text.Length != 0){
            JoinRelay(inpcode.text);
        }
    }

    public void HideRelayScreen(){
        relayScreen.SetActive(false);
    }

}
