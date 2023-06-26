using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler , IPointerClickHandler
{
    #region Serialize Fields
    [SerializeField]
    private AudioSource _audioSource = null;
    
    [SerializeField]
    private AudioClip _audioPointing = null;
    
    [SerializeField]
    private AudioClip _audioClick = null;
    
    [SerializeField]
    private Text _buttonText = null;
    
    [SerializeField]
    private Button _button = null;
    #endregion
    
    #region Private Fields
    private static Vector3 _newScale = new Vector3(1.15f, 1.15f, 1.15f);
    #endregion
    
    #region Public Methods
    public void SetText(string text)
    {
        _buttonText.text = text;
    }

    public Button.ButtonClickedEvent onClick() => _button.onClick;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = _newScale;
        Play(_audioPointing);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        Play(_audioClick);
    }
    #endregion

    #region Private Methods
    private void Play(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
    #endregion
}
