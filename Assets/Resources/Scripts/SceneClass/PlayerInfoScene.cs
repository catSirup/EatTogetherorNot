using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfoScene : Scene
{
    [SerializeField]
    private GameObject playerInfoUI;

    [SerializeField]
    private Text totalPlayTime;
    [SerializeField]
    private Text totalPlayCount;
    [SerializeField]
    private Text maximumPlayTime;

    public override void Initialize()
    {
        playerInfoUI.SetActive(true);

        float hour = (int)(PlayerPrefs.GetFloat("totalPlayTime") / 3600);
        float min = (int)((PlayerPrefs.GetFloat("totalPlayTime") - hour * 3600) / 60);
        float sec = (int)(PlayerPrefs.GetFloat("totalPlayTime") % 60);

        float maxHour = (int)(PlayerPrefs.GetFloat("maximumPlayTime") / 3600);
        float maxMin = (int)((PlayerPrefs.GetFloat("maximumPlayTime") - maxHour * 3600) / 60);
        float maxSec = (int)(PlayerPrefs.GetFloat("maximumPlayTime") % 60);

        totalPlayTime.text = hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
        totalPlayCount.text = PlayerPrefs.GetInt("totalPlayCount").ToString();
        maximumPlayTime.text = PlayerPrefs.GetFloat("maximumPlayTime").ToString();
    }

    public override void Updated()
    {

    }

    public override void Exit()
    {
        playerInfoUI.SetActive(false);
    }

    #region Buttons
    public void Button_Exit()
    {
        SoundManager.soundMgr.PlayES("Click");
        SceneManager.sceneMgr.ChangeScene(SceneState.MAIN);
    }
    #endregion
}
