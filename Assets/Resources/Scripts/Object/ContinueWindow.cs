using UnityEngine;
using System.Collections;

public class ContinueWindow : MonoBehaviour {
    #region Buttons
    public void Buttton_Yes()
    {
        DataManager.playCount += 1;
        SoundManager.soundMgr.PlayES("Click");
        SceneManager.sceneMgr.prevState = SceneState.PLAY;
        SceneManager.sceneMgr.ChangeScene(SceneState.TEMP);

        Time.timeScale = 1.0f;
    }

    public void Button_No()
    {
        DataManager.playCount += 1;
        SoundManager.soundMgr.PlayES("Click");
        SceneManager.sceneMgr.ChangeScene(SceneState.MAIN);

        Time.timeScale = 1.0f;
    }
    #endregion
}
