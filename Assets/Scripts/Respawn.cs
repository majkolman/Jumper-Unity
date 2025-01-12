using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Respawn : MonoBehaviour
{
    public Vector3 spawnPoint;
    public GameObject UI_canvas;
    private Transform player_transform;
    private KeyCode menuKey = KeyCode.Escape;
    // Start is called before the first frame update
    void Start()
    {
        // set the spawnpoint to the player's starting position and turn off the UI
        spawnPoint = gameObject.transform.position;
        UI_canvas = GameObject.FindGameObjectWithTag("UI-canvas");
        UI_canvas.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        player_transform = gameObject.transform.parent.transform.parent.transform;
        
        if (player_transform.position.y < 0)
        {
            // the player is "dead" if y is lower than 0
            // stop the time and turn on the death screen
            Time.timeScale = 0;

            UI_canvas.transform.GetChild(0).gameObject.SetActive(true);
            Transform menuText = UI_canvas.transform.GetChild(0).Find("menuText");
            menuText.GetComponent<TMP_Text>().text = "You died";
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyUp(menuKey))
        {
            // if player presses escape turn on pause screen
            Transform menuText = UI_canvas.transform.GetChild(0).Find("menuText");
            menuText.GetComponent<TMP_Text>().text = "Game paused";
            // turns on/off the pause scren
            TogglePanel();
        }
    }

    public void RespawnPlayer() 
    {
        // set the player position to the checkpoint we also make the rigidbody Sleep() otherwise the player won't be able to change position
        player_transform.GetComponent<Rigidbody>().Sleep();
        player_transform.position = spawnPoint;
        
        UI_canvas.transform.GetChild(0).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    private void TogglePanel()
    {
        if(UI_canvas.transform.GetChild(0).gameObject.activeSelf == false)
        {
            Time.timeScale = 0;
            UI_canvas.transform.GetChild(0).gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            UI_canvas.transform.GetChild(0).gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }
}
