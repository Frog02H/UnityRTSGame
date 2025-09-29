using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrLoseGoToWhere : MonoBehaviour
{
    public int WinOrLose;
    public GameObject backGround;
    public GameObject Win;
    public GameObject Lose;
    public GameObject Pause;
    public GameObject bottonPanel;
    public bool isPauseActive = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        backGround = transform.GetChild(0).gameObject;
        Win = transform.GetChild(1).gameObject;
        Lose = transform.GetChild(2).gameObject;
        Pause = transform.GetChild(3).gameObject;
        bottonPanel = transform.GetChild(4).gameObject;

        backGround.gameObject.SetActive(false);
        Win.gameObject.SetActive(false);
        Lose.gameObject.SetActive(false);
        Pause.gameObject.SetActive(false);
        bottonPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ComBuildingManager.instance.allComBuildings.Count <= 0)
        {
            WinOrLose = 1;
        }

        if (BuildingManager.instance.allBuildings.Count <= 0)
        {
            WinOrLose = 2;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            isPauseActive = !isPauseActive;

            if (isPauseActive)
            {
                WinOrLose = 3;
            }
            else
            {
                WinOrLose = 4;
            }
        }

        if (WinOrLose > 0)
        {
            gameObject.SetActive(true);
            backGround.gameObject.SetActive(true);
            bottonPanel.gameObject.SetActive(true);

            if (WinOrLose == 1)
            {
                Win.gameObject.SetActive(true);
                Lose.gameObject.SetActive(false);
                Pause.gameObject.SetActive(false);
                Time.timeScale = 0;
            }

            if (WinOrLose == 2)
            {
                Lose.gameObject.SetActive(true);
                Win.gameObject.SetActive(false);
                Pause.gameObject.SetActive(false);
                Time.timeScale = 0;
            }

            if (WinOrLose == 3)
            {
                Lose.gameObject.SetActive(false);
                Win.gameObject.SetActive(false);
                Pause.gameObject.SetActive(true);
                Time.timeScale = 0;
            }

            if (WinOrLose == 4)
            {
                backGround.gameObject.SetActive(false);
                Win.gameObject.SetActive(false);
                Lose.gameObject.SetActive(false);
                Pause.gameObject.SetActive(false);
                bottonPanel.gameObject.SetActive(false);
                Time.timeScale = 1;
                WinOrLose = 0;
            }

            if (WinOrLose == 1 && WinOrLose == 2)
            {
                ActorManager.instance.KillThemAll();
                ComActorManager.instance.KillThemAll();
                Time.timeScale = 0;
            }

        }
    }

    public void Retry()
    {
        Time.timeScale = 1;
        isPauseActive = false;
        SceneManager.LoadScene(1);
    }

    public void JumpToMainMenu()
    {
        Time.timeScale = 1;
        isPauseActive = false;
        SceneManager.LoadScene(0);
    }
}
