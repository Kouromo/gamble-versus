using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Vector3 offset = new Vector3(0, 2, 0);
    public Vector3 localScale = new Vector3(0.01f, 0.01f, 0.01f);
    private CombatEntity target;
    private Camera mainCamera;

    public void SetTarget(CombatEntity entity)
    {
        target = entity;
        mainCamera = Camera.main;
        transform.localScale = localScale;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        if (target != null && slider != null)
        {
            slider.maxValue = target.baseData.maxHealth;
            slider.value = target.currentHealth;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Positionne le World Space Canvas au-dessus de l'entité
            transform.position = target.transform.position + offset;
            
            // Fait face à la caméra
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.transform.forward);
            }
        }
        else
        {
            // Si l'entité est détruite/désactivée, on cache la barre de vie
            gameObject.SetActive(false);
        }
    }
}
