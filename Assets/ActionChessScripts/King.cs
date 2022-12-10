using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class King : MonoBehaviour
{
    private ChessAI.ChessBoard boardScript;
    private CharacterController controller;
    private Transform tile;
    private NavMeshAgent agent;
    private Animator animator;
    private char piece;
    private AudioSource audioSource;
    public AudioClip deathAudio;
    int count = 0;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetTile();
        piece = Board.cb.GetCurrentBoardDict()[Board.GetTileNumberFromTileTransform(tile)];
        audioSource = transform.gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Fire1") > 0)
        {
            SetTile();
        }
    }

    private void SetTile()
    {
        RaycastHit hit;
        if (Physics.Raycast(agent.destination, Vector3.down, out hit, 100, 1 << 6))
        {
            if (hit.transform.name.Contains("Tile"))
            {
                tile = hit.transform;
            }
        }
    }

    void OnTriggerEnter(Collider body)
    {
        if (body.transform.name.StartsWith("Chess_") && body.transform.position != transform.position)
        {
            animator.SetBool("doAttack", true);
        }
        if ((body.transform.name.Contains("word") || body.transform.name.Contains("Axe")))
        {
            if (Board.cb.GetCurrentBoardDict()[Board.GetTileNumberFromTileTransform(tile)] == piece || Board.cb.GetCurrentBoardDict()[Board.GetTileNumberFromTileTransform(tile)] == ' ')
            {
                return;
            }
            animator.SetBool("isDead", true);
            audioSource.clip = deathAudio;
            count++;
            if(count == 1){
                // foreach (var item in transform.GetComponentsInChildren<ParticleSystem>())
                // {
                //     item.Play();
                // }
                audioSource.Play();
            }
            else if(count == 2){
                audioSource.Play();
                Destroy(transform.gameObject, 3f);
            }
        }
    }

    void OnTriggerExit(Collider body)
    {
        if (body.transform.name.StartsWith("Chess_") && body.transform.position != transform.position)
        {
            animator.SetBool("doAttack", false);
            animator.SetBool("doWalk", false);
        }
    }
}
