using System;
using UnityEngine;

public class ScriptableObjectsManager : MonoBehaviour
{
    #region Serialize Fields
    public PrefabContainer _prefabContainer = null;
    #endregion

    #region Public Fields
    public static ScriptableObjectsManager instance = null;
    #endregion

    #region Private Methods
    private void Awake()
    {
        instance = this;
    }
    #endregion
}
