using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Player UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image armorBar;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("UI Extra")]
    [SerializeField] private CanvasGroup fadePanel;
    //[SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI completedTMP;
    [SerializeField] private TextMeshProUGUI coinsTMP;

    [Header("UI Weapon")]
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponEnergyTMP;

    private void Update()
    {
        UpdatePlayerUI();
        coinsTMP.text = CoinManager.Instance.Coins.ToString();
    }

    public void UpdateLevelText(string levelText)
    {
        levelTMP.text = levelText;
    }

    private void UpdatePlayerUI()
    {
        PlayerConfig playerConfig = GameManager.Instance.Player;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount,
            playerConfig.CurrentHealth / playerConfig.MaxHealth, 10f * Time.deltaTime);
        armorBar.fillAmount = Mathf.Lerp(armorBar.fillAmount,
            playerConfig.Armor / playerConfig.MaxArmor, 10f * Time.deltaTime);
        energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount,
            playerConfig.Energy / playerConfig.MaxEnergy, 10f * Time.deltaTime);

        healthText.text = $"{playerConfig.CurrentHealth}/{playerConfig.MaxHealth}";
        armorText.text = $"{playerConfig.Armor}/{playerConfig.MaxArmor}";
        energyText.text = $"{playerConfig.Energy}/{playerConfig.MaxEnergy}";
    }

    private void RoomCompletedCallback()
    {
        StartCoroutine(IERoomCompleted());
    }

    private IEnumerator IERoomCompleted()
    {
        completedTMP.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        completedTMP.gameObject.SetActive(false);
    }

    public void FadeNewDungeon(float value)
    {
        StartCoroutine(Helpers.IEFade(fadePanel, value, 1.5f));
    }

    private void WeaponUIUpdateCallback(Weapon currentWeapon)
    {
        if (weaponPanel.activeSelf == false)
        {
            weaponPanel.SetActive(true);
        }

        weaponEnergyTMP.text = currentWeapon.ItemWeapon.RequiredEnergy.ToString();
        weaponIcon.sprite = currentWeapon.ItemWeapon.Icon;
    }

    private void OnEnable()
    {
        PlayerWeapon.OnWeaponUIUpdateEvent += WeaponUIUpdateCallback;
        //PlayerHealth.OnPlayerDeadEvent += PlayerDeadCallback;
        LevelManager.OnRoomCompletedEvent += RoomCompletedCallback;
    }

    private void OnDisable()
    {
        PlayerWeapon.OnWeaponUIUpdateEvent -= WeaponUIUpdateCallback;
        //PlayerHealth.OnPlayerDeadEvent -= PlayerDeadCallback;
        LevelManager.OnRoomCompletedEvent -= RoomCompletedCallback;
    }
}
