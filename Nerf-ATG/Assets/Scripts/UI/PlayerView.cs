using System;
using UnityEngine;
using UnityEngine.UI;

// Die View kennt NUR Unity-UI-Elemente – keine Spiel-Logik!
public class PlayerView : MonoBehaviour
{
    // Inspector-Zuweisungen
    [Header("Health")]
    [SerializeField] private Text _healthText;
    [SerializeField] private RectTransform _healthBar;

    [Header("Ammo")]
    [SerializeField] private Text _ammoText;
    [SerializeField] private RectTransform _ammoBar;

    // Events für Benutzerinteraktionen
    public event Action OnAbilityButtonClicked;

    // Öffentliche Methoden zur UI-Aktualisierung
    public void UpdateHealthUI(int health, int maxHealth)
    {
        _healthText.text = $"Health: {health}";
        float width = (_healthBar.parent.GetComponent<RectTransform>().rect.width - 20) * health / maxHealth;
        _healthBar.sizeDelta = new Vector2(width, _healthBar.rect.height);
    }

    // Wird vom Button im Unity-Editor aufgerufen
    public void HandleAbilityButtonClick()
    {
        OnAbilityButtonClicked?.Invoke();
    }
}