using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public static DamagePopup Create(Vector3 position, int damageAmount, bool isHeal = false)
    {
        // Create an empty GameObject
        GameObject damagePopupTransform = new GameObject("DamagePopup");
        damagePopupTransform.transform.position = position;

        // Add TextMeshPro
        TextMeshPro textMesh = damagePopupTransform.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 5;
        textMesh.fontStyle = FontStyles.Bold;
        textMesh.text = damageAmount.ToString();
        textMesh.color = isHeal ? Color.green : Color.red;

        // Add DamagePopup script
        DamagePopup damagePopup = damagePopupTransform.AddComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isHeal);

        return damagePopup;
    }

    public static DamagePopup CreateText(Vector3 position, string text, Color color)
    {
        GameObject damagePopupTransform = new GameObject("DamagePopup");
        damagePopupTransform.transform.position = position;

        TextMeshPro textMesh = damagePopupTransform.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 4;
        textMesh.fontStyle = FontStyles.Bold;
        textMesh.text = text;
        textMesh.color = color;

        DamagePopup damagePopup = damagePopupTransform.AddComponent<DamagePopup>();
        damagePopup.SetupText(color);

        return damagePopup;
    }

    private void Setup(int damageAmount, bool isHeal)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        textMesh.text = isHeal ? $"+{damageAmount}" : damageAmount.ToString();
        textColor = isHeal ? Color.green : Color.red;
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        // Randomize upward movement
        moveVector = new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-0.5f, 0.5f)).normalized * 2f;
    }

    private void SetupText(Color color)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        textColor = color;
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;
        moveVector = new Vector3(0, 2f, 0).normalized * 1.5f;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 2f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            // First half of the popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Start fading out
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
        
        // Face the camera
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }
}
