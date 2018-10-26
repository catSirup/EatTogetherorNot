using UnityEngine;
using System.Collections;

public class StopWindow : MonoBehaviour {
    #region Buttons
    public void Button_Continue()
    {
        Time.timeScale = 1.0f;
        SoundManager.soundMgr.PlayES("Click");
        gameObject.SetActive(false);
    }
    public void Button_Restart()
    {
        Time.timeScale = 1.0f;
        SoundManager.soundMgr.PlayES("Click");
        gameObject.SetActive(false);
        SceneManager.sceneMgr.prevState = SceneState.PLAY;
        SceneManager.sceneMgr.ChangeScene(SceneState.TEMP);
    }
    public void Button_Exit()
    {
        Time.timeScale = 1.0f;
        SoundManager.soundMgr.PlayES("Click");
        gameObject.SetActive(false);
        SceneManager.sceneMgr.ChangeScene(SceneState.MAIN);
    }
    #endregion
}
