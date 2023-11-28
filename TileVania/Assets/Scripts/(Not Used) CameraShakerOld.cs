using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraShakerOld : MonoBehaviour
{
    private GameObject player;
    
    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(this.player.GetComponent<PlayerMovement>().isAlive == false)
        {
            StartCoroutine(CameraShakerCoroutine(1f));
        }
    }

    private IEnumerator CameraShakerCoroutine(float duration)
    {
        float elapsed = 0f;
        float currentMagnitude = 1f;

        while (elapsed < duration)
        {
            float x = (Random.value - 0.5f) * currentMagnitude;
            float y = (Random.value - 0.5f) * currentMagnitude;
            
            transform.localPosition = new Vector2(x,y);

            elapsed += Time.deltaTime;
            currentMagnitude = (1-(elapsed/duration)) * (1-(elapsed/duration));

            yield return null;
        }

        //transform.localPosition = Vector2.Zero;
    }
}