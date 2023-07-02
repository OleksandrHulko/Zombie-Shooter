using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private CanvasGroup _canvasGroup = null;
    
    [SerializeField]
    private Text _descriptionText = null;
    
    [SerializeField]
    private UIButton _uiButtonLeft = null;
    
    [SerializeField]
    private UIButton _uiButtonRight = null;
    #endregion

    #region Public Fields
    public bool isVisible = false;
    #endregion

    #region Public Methods
    public void Init( string description, string leftBtnTxt, string rightBtnTxt, Action onLeftBtnClick, Action onRightBtnClick = null )
    {
        _uiButtonLeft .onClick().RemoveAllListeners();
        _uiButtonRight.onClick().RemoveAllListeners();
        
        _descriptionText.text = description;
        
        _uiButtonLeft .SetText(leftBtnTxt);
        _uiButtonRight.SetText(rightBtnTxt);

        _uiButtonLeft .onClick().AddListener(() => onClick(onLeftBtnClick));
        _uiButtonRight.onClick().AddListener(() => onClick(onRightBtnClick));

        void onClick(Action onBtnClick = null)
        {
            onBtnClick?.Invoke();
            SetVisible(false);
        }
    }

    public void SetVisible(bool visible = true)
    {
        _canvasGroup.alpha = visible ? 1.0f : 0.0f;
        _canvasGroup.blocksRaycasts = visible;
        
        isVisible = visible;
    }
    #endregion
}
