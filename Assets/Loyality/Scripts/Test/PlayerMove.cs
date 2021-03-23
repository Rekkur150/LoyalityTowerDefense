using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    public GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            Camera.SetActive(true);
        } else
        {
            Camera.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            Vector3 move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
            transform.position += move * 10 * Time.deltaTime;
        }
    }
}
