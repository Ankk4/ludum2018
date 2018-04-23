using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vittu : MonoBehaviour
{
    [SerializeField]
    public Vector3 target;
    [SerializeField]
    public GameObject gm;
    [SerializeField]
    private float yOffset, zOffset;
    private Vector3 offset;
    private bool toggleTargets;

    private void Start()
    {
        yOffset = 8.07f;
        zOffset = -7.18f;
        //offset = transform.position - gm.GetComponent<GameManager>().ghost_rb.transform.position;
        offset = new Vector3(0, yOffset, zOffset);
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        GameManager.GameState gs = gm.GetComponent<GameManager>().CurrentGameState;
        switch (gs)
        {
            case GameManager.GameState.action:
                target = gm.GetComponent<GameManager>().ghost_rb.transform.position;
                break;
            case GameManager.GameState.simulation:
                target = gm.GetComponent<GameManager>().player_rb.transform.position;
                break;
        }
        transform.position = target + offset;
    }
}
