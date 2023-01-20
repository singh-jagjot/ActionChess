using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkScript : NetworkBehaviour
{
    private bool blackTurn = false;
    public struct NetworkPlayerMove : INetworkSerializable
    {
        public int source;
        public int destination;

        public bool isBlack;

        public NetworkPlayerMove(int source, int destination, bool isBlack)
        {
            this.source = source;
            this.destination = destination;
            this.isBlack = isBlack;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // throw new System.NotImplementedException();
            serializer.SerializeValue(ref source);
            serializer.SerializeValue(ref destination);
            serializer.SerializeValue(ref isBlack);
        }

        public string toString()
        {
            return "isblack:" + isBlack + ",source:" + source + ",destination:" + destination;
        }
    }

    private NetworkVariable<NetworkPlayerMove> serverMoved = new NetworkVariable<NetworkPlayerMove>(new NetworkPlayerMove(0, 0, false), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<NetworkPlayerMove> clientMoved = new NetworkVariable<NetworkPlayerMove>(new NetworkPlayerMove(0, 0, false), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private bool isCameraMoved = false;
    private bool doWrite = false;
    void Start()
    {
        GameObject.Find("ChessBoardMainNetwork").GetComponent<NetworkBoard>().setNetworkScript(this);
        // if(OwnerClientId != 0){
        //     blackTurn = true;
        // }
    }
    void Update()
    {
        // Debug.Log(OwnerClientId + ":" + IsOwner + ":" + nPlayerMove.Value.toString());
        // Debug.Log("DW:"+doWrite);
        // Debug.Log(OwnerClientId + ":" + IsOwner + ":" + writeValue.toString());
        if (!IsOwner)
        {
            return;
        }

        if (IsClient && !IsHost && !isCameraMoved)
        {
            var ob1 = GameObject.Find("Main Camera");
            ob1.transform.position = new Vector3(-1.5f, 20.5f, 15f);
            ob1.transform.eulerAngles = new Vector3(115f, 0f, 180f);
            isCameraMoved = true;
        }
        // if(Input.GetKey(KeyCode.Space)){
        //     nPlayerMove.Value = new NetworkPlayerMove(1, Random.Range(0,100), true);
        // }
        // if(Input.GetAxis("Fire1") == 1){
        //     if(doWrite){
        //     doWrite = false;
        //     nPlayerMove.Value = writeValue;
        // }
        // }

    }

    public override void OnNetworkSpawn()
    {
        serverMoved.OnValueChanged += (NetworkPlayerMove previousValue, NetworkPlayerMove newValue) =>
        {
            if (IsOwner)
            {
                Debug.Log(OwnerClientId + ":" + serverMoved.Value.toString());
                blackTurn = true;
                var script = GameObject.Find("ChessBoardMainNetwork").GetComponent<NetworkBoard>();
                script.moveWhite(serverMoved.Value.source, serverMoved.Value.destination);
            }

        };

        clientMoved.OnValueChanged += (NetworkPlayerMove previousValue, NetworkPlayerMove newValue) =>
        {
            if (IsHost)
            {
                Debug.Log(OwnerClientId + ":" + clientMoved.Value.toString());
                blackTurn = false;
                var script = GameObject.Find("ChessBoardMainNetwork").GetComponent<NetworkBoard>();
                script.moveBlack(clientMoved.Value.source, clientMoved.Value.destination);
            }

        };
    }

    public void move(int source, int destination, bool isBlack, string l)
    {

        var val = new NetworkPlayerMove(source, destination, isBlack);
        this.blackTurn = !isBlack;
        doWrite = true;
        Debug.Log("blackTurn:"+blackTurn+" C:"+l);
        if (!isBlack)
        {
            serverMoved.Value = val;
        }
        else
        {
            clientMoved.Value = val;
        }
    }

    public bool isBlackTurn()
    {
        return blackTurn;
    }

}
