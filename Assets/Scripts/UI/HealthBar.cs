using UnityEngine;
using UnityEngine.UI;

//Requires this component to function
[RequireComponent(typeof(Slider))]

public class HealthBar : MonoBehaviour
{
    //slider used to display the players current health
    private Slider healthBarSlider;

    private void Awake()
    {
        healthBarSlider = GetComponent<Slider>();
    }

    //called to set the max health that the player has
    public void SetMaxHealth(float maxHealth)
    {
        //checks if there is a fill rect set for the slider
        if (healthBarSlider!= null && healthBarSlider.fillRect != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = maxHealth;
        }
        else
        {
            Debug.LogWarning("No slider fill for health bar was found, player health cannot be updated. Please add a slider fill in the slider component");
        }
    }

    //updates the players current health
    public void SetCurrentHealth(float currentHealth)
    {
        //checks if there is a fill rect set for the slider
        if (healthBarSlider != null && healthBarSlider.fillRect != null)
        {
            healthBarSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("No slider fill for health bar was found, player health cannot be updated. Please add a slider fill in the slider component");
        }
    }
}
