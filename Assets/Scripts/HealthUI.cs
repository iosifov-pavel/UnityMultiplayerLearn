using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] GameObject healthBar = null;
    [SerializeField] Image healthImage = null;

    private void Start() {
        if(health==null){return;}
        health.ClientOnHealthChange += UpdateHealthBar;
    }

    private void OnMouseEnter() {
        healthBar.SetActive(true);
    }

    private void OnMouseExit() {
        healthBar.SetActive(false);
    }

    private void UpdateHealthBar(int oldValue, int newValue){
        healthImage.fillAmount = (float)newValue/(float)oldValue;
    }

    private void OnDestroy() {
        
    }

}
