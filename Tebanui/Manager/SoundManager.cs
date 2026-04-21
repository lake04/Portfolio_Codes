using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBgm
{
    BGM_TITLE,
    BGM_GAME,
}

public enum SoundType
{
    SFX_BUTTON,
    SFX_ENDING,
    SFX_BOTTLE,
    SFX_OPENDOOR,
    SFX_SHOOT,
    SFX_Attack,
    BOSS_BGM,
    SFX_Walk,
    SFX_swwing
}

[System.Serializable]
public struct SoundEntry
{
    public SoundType type;
    public EventReference fmodEvent;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("ui 소리")]
    public EventReference buttonSound;

    [Header("배경 음악")]
    public EventReference bgmEventRef;

    private EventInstance bgmInstance;

    private EventInstance walkInstance;

    public List<SoundEntry> soundList;

    private Dictionary<SoundType, EventReference> sfxs = new Dictionary<SoundType, EventReference>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
             DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        foreach (var entry in soundList)
        {
            if (!sfxs.ContainsKey(entry.type))
            {
                sfxs.Add(entry.type, entry.fmodEvent);
            }
        }
    }

    private void Start()
    {
        PlayBGM(bgmEventRef);
    }

    // BGM 재생 메서드
    public void PlayBGM(EventReference eventRef)
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            bgmInstance.release();
        }

        bgmInstance = RuntimeManager.CreateInstance(eventRef);

        bgmInstance.start();

    }

    public void StopBGM()
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            bgmInstance.release(); 
        }
    }


    public void ButtonSound()
    {
        RuntimeManager.CreateInstance(buttonSound).start();
    }

    public void PlaySFX(SoundType esfx)
    {
        if (sfxs.TryGetValue(esfx, out EventReference fmodEvent))
        {
            RuntimeManager.CreateInstance(fmodEvent).start();
            
        }
    }

    public void PlayWalkSFX()
    {
        if (sfxs.TryGetValue(SoundType.SFX_Walk, out EventReference fmodEvent))
        {
            if (!walkInstance.isValid())
            {
                walkInstance = RuntimeManager.CreateInstance(fmodEvent);
            }

            FMOD.Studio.PLAYBACK_STATE state;
            walkInstance.getPlaybackState(out state);

            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                walkInstance.start();
            }
        }
    }

    public void StopWalkSFX()
    {
        if (walkInstance.isValid())
        {
            walkInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
