using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Sprite halfHeart;
    public Image[] hearts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = GameManager.Instance.maxLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance == null)
            return;

        health = GameManager.Instance.playerLives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = halfHeart;
                if (i + 0.5f < health)
                {
                    hearts[i].sprite = fullHeart;
                }
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}
