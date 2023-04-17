using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Button _buttonPlay = null;
    
    [SerializeField]
    private Button _buttonExit = null;

    [SerializeField]
    private Text _loadingText = null;
    
    [SerializeField]
    private CanvasGroup _canvasGroup = null;
    
    [SerializeField]
    private DialogWindow _dialogWindow = null;
    #endregion

    #region Private Methods
    private void Awake()
    {
        _buttonPlay.onClick.AddListener(Play);
        _buttonExit.onClick.AddListener(() => _dialogWindow.SetVisible());

        _dialogWindow.Init(Localization.QUIT_THE_GAME, Localization.YES, Localization.NO, Exit);
    }

    private void OnDestroy()
    {
        _buttonPlay.onClick.RemoveAllListeners();
        _buttonExit.onClick.RemoveAllListeners();
    }

    private void Play()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SampleScene");
        
        showProgressText();
        StartCoroutine(chekProgress());

        IEnumerator chekProgress()
        {
            while (asyncOperation.progress < 0.9f)
            {
                _loadingText.text = string.Format(Localization.LOADING_TEXT, getProgress());
                yield return null;
            }

            int getProgress()
            {
                return Mathf.RoundToInt(asyncOperation.progress / 0.9f) * 100;
            }
        }

        void showProgressText()
        {
            _canvasGroup.alpha = 0.0f;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    private void Exit()
    {
        Application.Quit();
    }
    #endregion
}
