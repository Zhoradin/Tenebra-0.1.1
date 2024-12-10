using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
    }

    public AudioSource menuMusic;
    public AudioSource hubMusic;
    public AudioSource barMusic;
    public AudioSource battleMusic;
    private AudioClip[] musicQueue;
    private int currentMusicIndex = 0;

    public AudioSource[] sfx;

    public void StopMusic()
    {
        menuMusic.Stop();
        barMusic.Stop();
        hubMusic.Stop();
        battleMusic.Stop();
    }

    public void PullMenuMusic()
    {
        menuMusic.clip = MainMenu.instance.menuMusic;
        PlayMenuMusic();
    }

    private void PlayMenuMusic()
    {
        StopMusic();
        menuMusic.Play();
    }

    public void PullHubMusic()
    {
        hubMusic.clip = HubController.instance.hubMusic;
        PlayHubMusic();
    }

    private void PlayHubMusic()
    {
        StopMusic();
        hubMusic.Play();
    }

    public void PullBarMusic()
    {
        barMusic.clip = BarController.instance.barMusic;
        PlayBarMusic();
    }


    private void PlayBarMusic()
    {
        StopMusic();
        barMusic.Play();
    }

    public void PullBattleMusic()
    {
        musicQueue = DataCarrier.instance.enemy.backgroundMusic;
        StopMusic();
        StartCoroutine(PlayBattleMusicCo());
    }

    private IEnumerator PlayBattleMusicCo()
    {
        while (musicQueue != null && musicQueue.Length > 0)
        {
            // �u anki m�zi�i ayarla ve �al
            battleMusic.clip = musicQueue[currentMusicIndex];
            battleMusic.Play();

            // M�zik �alma s�resi boyunca bekle
            yield return new WaitForSeconds(battleMusic.clip.length);

            // Sonraki m�zi�e ge�
            currentMusicIndex = (currentMusicIndex + 1) % musicQueue.Length;
        }
    }

    public void PlaySFX(int sfxToPlay)
    {
        sfx[sfxToPlay].Stop();
        sfx[sfxToPlay].Play();
    }

    /*public void StopBattleMusic()
    {
        // M�zi�i durdur ve coroutine'i iptal et
        StopCoroutine(PlayBattleMusicCo());
        battleMusic.Stop();
    }*/
}
