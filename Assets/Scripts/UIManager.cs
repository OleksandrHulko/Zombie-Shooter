using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private List<MonoBehaviour> _elements = null;
    
    [SerializeField]
    private DialogWindow _dialogWindow = null;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _elements.Select(x => x.GetComponent<IUserInterface>()).ToList().ForEach(x => x.Init());

        Player.onDead += ShowDialogDead;

        SetVisibleCursor(false);
    }

    private void OnDestroy()
    {
        _elements.Select(x => x.GetComponent<IUserInterface>()).ToList().ForEach(x => x.Deinit());

        Player.onDead -= ShowDialogDead;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowOrClosePauseDialog();
    }

    private void ShowOrClosePauseDialog()
    {
        if (Player.instance.IsDead())
            return;
        
        if(!_dialogWindow.isVisible)
        {
            SetGamePause();
            
            _dialogWindow.Init(Localization.PAUSE, Localization.QUIT, Localization.RESUME, QuitToMenu, () => SetGamePause(false));
            _dialogWindow.SetVisible();
        }
        else
        {
            SetGamePause(false);
            _dialogWindow.SetVisible(false);
        }
    }

    private void ShowDialogDead()
    {
        SetGamePause();
        _dialogWindow.Init(Localization.GAME_OVER, Localization.QUIT, Localization.TRY_AGAIN, QuitToMenu, ReloadGameScene);
        _dialogWindow.SetVisible();
    }

    private void QuitToMenu()
    {
        SetGamePause(false, true);
        SceneManager.LoadScene("Main Menu");
    }

    private void ReloadGameScene()
    {
        SetGamePause(false, true);
        SceneManager.LoadScene("SampleScene");
    }

    private void SetGamePause( bool pause = true, bool? visibleCursor = null )
    {
        SetVisibleCursor(visibleCursor ?? pause);
        
        Player.instance.enabled = !pause;
        Time.timeScale = pause ? 0.0f : 1.0f;
    }

    private void SetVisibleCursor( bool visible = true )
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion
}
