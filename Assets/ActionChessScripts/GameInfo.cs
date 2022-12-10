using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameInfo : MonoBehaviour
{
    public TextMeshProUGUI winMsg;
    private string msg;
    // Start is called before the first frame update
    void Start()
    {
    }
    void LateUpdate()
    {
        winMsg.text = this.msg;
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
        transform.Find("InfoPanel").gameObject.SetActive(false);
        Transform sp = transform.Find("StartPanel");
        // Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        sp.gameObject.SetActive(false);
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Time.timeScale = 1f;

    }
}
