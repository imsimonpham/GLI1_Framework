using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private FPS_Controller _controller;
    private bool _isGamePaused;
    private bool _playerWon;
    private bool _playerLost;
    private bool _isGameOver = false;

    private void Update()
    {
        //Loss
        if (_player.GetLifePoint() <= 0)
        {
            if (!_isGameOver)
            {
                StartCoroutine(WaitRoutine("loss"));
                _isGameOver = true;
            }
        }

        //Win
        if (SpawnManager.Instance.DidPlayerSurvive())
        {
            if (!_isGameOver)
            {
                StartCoroutine(WaitRoutine("win"));
                _isGameOver = true;
            }
        }

        //Pause Menu
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            _isGamePaused = !_isGamePaused;
        }

        if(_isGamePaused)
        {
            FreezeScreen();
            UIManager.Instance.TogglePauseMenu(true);
        }else
        {
            UnfreezeScreen();
            UIManager.Instance.TogglePauseMenu(false);
        }

        //Win - Loss Menu
        if (_playerLost)
        {
           
            FreezeScreen();
            UIManager.Instance.ToggleWinLossMenu("loss");
        }

        if (_playerWon)
        {       
            FreezeScreen();
            UIManager.Instance.ToggleWinLossMenu("win");
        }
    }

    void ToggleCursor(bool show)
    {
        if(show)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    } 

    void ToggleLockCursor(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }else
        {
            Cursor.lockState = CursorLockMode.None;
        }   
    }

    public void ResumeGame()
    {
        _isGamePaused = false;
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    void FreezeScreen()
    {
        _player.TogglePlayerEnablement(false);
        ToggleLockCursor(false);
        _controller.enabled = false;
        ToggleCursor(true);
        Time.timeScale = 0f;
    }

    void UnfreezeScreen()
    {
        _player.TogglePlayerEnablement(true);
        _controller.enabled = true;
        Time.timeScale = 1f;
        ToggleCursor(show: false);
    }

    IEnumerator WaitRoutine(string conditionText)
    {
        yield return new WaitForSeconds(2f);
        if(conditionText == "win")
        {
            _playerWon = true;
        }else
        {
            _playerLost = true;
        }
        
    }
}
