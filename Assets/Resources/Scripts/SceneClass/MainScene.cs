using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainScene : Scene
{
    [SerializeField]
    private GameObject mainUI;          // 메인 유아이 오브젝트
    private bool b_OpenWindow;          // 윈도우가 켜져있는지 안켜져있는지 체크해주는 변수.

    [SerializeField]
    private Image SoundButton;
    [SerializeField]
    private Image VibrationButton;

    [SerializeField]
    private Sprite[] sounds;
    [SerializeField]
    private Sprite[] vibrations;

    [SerializeField]
    private GameObject developWindow;

    public override void Initialize()
    {
        mainUI.SetActive(true);
        SoundManager.soundMgr.PlayBGM("MainBGM");
    }

    public override void Updated()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !b_OpenWindow)
            GameManager.gameMgr.QuitGame();

        else if (Input.GetKeyDown(KeyCode.Escape) && b_OpenWindow)
        {
            b_OpenWindow = false;
        }

        if(!DataManager.b_Sound && SoundManager.soundMgr.audioSource.isPlaying)
        {
            SoundManager.soundMgr.audioSource.Stop();
        }

        else if (DataManager.b_Sound && !SoundManager.soundMgr.audioSource.isPlaying)
        {
            SoundManager.soundMgr.audioSource.Play();
        }
    }

    public override void Exit()
    {
        mainUI.SetActive(false);
    }

    #region Buttons
    public void Button_Start()
    {
        SoundManager.soundMgr.PlayES("Click");
        SceneManager.sceneMgr.ChangeScene(SceneState.PLAY);
    }
    public void Button_PlayerInfo()
    {
        GameManager.gameMgr.SaveData();
        SoundManager.soundMgr.PlayES("Click");
        SceneManager.sceneMgr.ChangeScene(SceneState.INFO);
    }
    public void Button_Developer()
    {
        SoundManager.soundMgr.PlayES("Click");
        developWindow.SetActive(true);
    }
    public void Button_Sound()
    {
        DataManager.b_Sound = !DataManager.b_Sound;

        if(DataManager.b_Sound)
        {
            SoundButton.sprite = sounds[1];
        }

        else
        {
            SoundButton.sprite = sounds[0];
        }
    }
    public void Button_Vibration()
    {
        DataManager.b_Vibration = !DataManager.b_Vibration;

        if (DataManager.b_Vibration)
        {
            VibrationButton.sprite = vibrations[1];
        }

        else
        {
            VibrationButton.sprite = vibrations[0];
        }
    }
    public void Button_DeveloperBack()
    {
        developWindow.SetActive(false);
        SoundManager.soundMgr.PlayES("Click");
    }
    #endregion
}
