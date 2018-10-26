using UnityEngine;
using System.Collections;

public class RankScene : Scene
{
    [SerializeField]
    private GameObject rankUI;

    public override void Initialize()
    {
        rankUI.SetActive(true);
    }

    public override void Updated()
    {

    }

    public override void Exit()
    {
        rankUI.SetActive(false);
    }

    #region Buttons
    public void Button_Exit()
    {
        SoundManager.soundMgr.PlayES("Click");
        SceneManager.sceneMgr.ChangeScene(SceneState.MAIN);
    }
    public void Button_PlayerInfo()
    {
        SoundManager.soundMgr.PlayES("Click");
        SceneManager.sceneMgr.ChangeScene(SceneState.INFO);
    }
    #endregion

}