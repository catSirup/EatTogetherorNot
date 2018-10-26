using UnityEngine;
using System.Collections;

public class LogoScene : Scene
{
    private float time;
    private bool b_LogoSound;

    public override void Initialize()
    {
        time = 0;
        b_LogoSound = false;
        SoundManager.soundMgr.PlayES("Logo");
    }

    public override void Updated()
    {
        time += Time.deltaTime;

        if(!b_LogoSound)
        {
            //AudioSource.PlayClipAtPoint(SoundManager.soundMgr.clip[3], Camera.main.transform.position);
            b_LogoSound = true;
        }

        if (time > 3.0f)
            SceneManager.sceneMgr.ChangeScene(SceneState.MAIN);
    }

    public override void Exit()
    {

    }
}

