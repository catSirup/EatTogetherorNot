using UnityEngine;
using System.Collections;

public class DeveloperScene : Scene
{
    [SerializeField]
    private GameObject developerUI;

    public override void Initialize()
    {
        developerUI.SetActive(true);
    }

    public override void Updated()
    {

    }

    public override void Exit()
    {
        developerUI.SetActive(false);
    }

    #region Buttons
    public void Button_Exit()
    {
        SceneManager.sceneMgr.ChangeScene(SceneState.MAIN);
    }
    #endregion
}