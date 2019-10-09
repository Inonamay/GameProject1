using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public int numOfHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public PlayerController controller;

    public void Start()
    {
        controller = FindObjectOfType<PlayerController>();
    }
    private void Update()
    {

        for (int i = 0; i <hearts.Length; i++)
        {
        if (controller.Hp > numOfHealth)
        {
            controller.Hp = numOfHealth;
        }
            if (i < controller.Hp)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
