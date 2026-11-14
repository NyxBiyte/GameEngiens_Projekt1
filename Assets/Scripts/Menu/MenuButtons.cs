using System;
using System.Collections;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private Animator catAnimator;
    [SerializeField] float transitionDuration = 2.14f;

    private bool startingGame = false;

    public void StartGame()
    {
        if (startingGame) return;
        startingGame = true;
        StartCoroutine(startAnimation());
    }

    private IEnumerator startAnimation()
    {
        if (catAnimator != null)
        {
            catAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(transitionDuration);
            GameManager.Instance.NewGame();
        }
    }

    public void LoadLevel(int level)
    {
        GameManager.Instance.LoadLevel(level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}