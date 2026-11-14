using TMPro;
using UnityEngine;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;

    void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        scoreText.text = gm.lastScore.ToString();
        highscoreText.text = gm.lastWasNewHighscore ? "New Highscore" : "";
    }
}
