using UnityEngine;
using UnityEngine.UI;

public class WeaponInfo : MonoBehaviour, IUserInterface
{
    #region Serialize Fields
    [SerializeField]
    private Text _insertedText = null;
    [SerializeField]
    private Text _inStockText = null;
    #endregion

    #region Public Methods
    public void Init()
    {
        AutomaticRifle.onInsertedBulletsCountChanged += SetInsertedText;
        AutomaticRifle.onInStockBulletsCountChanged  += SetInStockText;
    }

    public void Deinit()
    {
        AutomaticRifle.onInsertedBulletsCountChanged -= SetInsertedText;
        AutomaticRifle.onInStockBulletsCountChanged  -= SetInStockText;
    }
    #endregion


    #region Private Methods
    private void SetInsertedText(int count)
    {
        SetText(_insertedText, count);
    }

    private void SetInStockText(int count)
    {
        SetText(_inStockText, count);
    }
    
    private void SetText(Text text, int count)
    {
        string countStr = count.ToString();

        if (text.text != countStr)
            text.text = countStr;
    }
    #endregion
}
