using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    [Header("Config")]
    [SerializeField] private PlayerCreation[] players;

    [Header("UI")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private Image playerIcon;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerHealthMaxStat;
    [SerializeField] private TextMeshProUGUI playerArmorMaxStat;
    [SerializeField] private TextMeshProUGUI playerEnergyMaxStat;
    [SerializeField] private TextMeshProUGUI playerCriticalMaxStat;
    [SerializeField] private TextMeshProUGUI coinsTMP;
    [SerializeField] private TextMeshProUGUI playerUpgradeCostTMP;
    [SerializeField] private TextMeshProUGUI playerUnlockCostTMP;

    [Header("Bars")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image armorBar;
    [SerializeField] private Image energyBar;
    [SerializeField] private Image criticalBar;

    [Header("Buttons")]
    [SerializeField] private GameObject unlockButton;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject selectButton;

    private SelectablePlayer currentPlayer;
    private bool playerSelected;

    private void Start()
    {
        CreatePlayer();
    }

    private void Update()
    {
        coinsTMP.text = CoinManager.Instance.Coins.ToString();
    }

    private void CreatePlayer()
    {
        for (int i = 0; i < players.Length; i++)
        {
            PlayerMovement player = Instantiate(players[i].Player, players[i].CreationPos.position,
                Quaternion.identity, players[i].CreationPos);
            player.enabled = false;
        }
    }

    public void ClickPlayer(SelectablePlayer selectablePlayer)
    {
        currentPlayer = selectablePlayer;
        VerifyPlayer();
        ShowPlayerStats();
    }

    public void SelectPlayer()
    {
        if (playerSelected) return;
        if (currentPlayer.Config.Unlocked)
        {
            currentPlayer.GetComponent<PlayerMovement>().enabled = true;
            currentPlayer.Config.ResetPlayerStats();
            GameManager.Instance.Player = currentPlayer.Config;
            ClosePlayerPanel();
            playerSelected = true;
        }
    }

    public void UpgradePlayer()
    {
        if (CoinManager.Instance.Coins >= currentPlayer.Config.UpgradeCost)
        {
            CoinManager.Instance.RemoveCoins(currentPlayer.Config.UpgradeCost);
            UpgradePlayerStats();
            ShowPlayerStats();
        }
    }

    public void UnlockPlayer()
    {
        if (CoinManager.Instance.Coins >= currentPlayer.Config.UnlockCost)
        {
            CoinManager.Instance.RemoveCoins(currentPlayer.Config.UnlockCost);
            currentPlayer.Config.Unlocked = true;
            VerifyPlayer();
            ShowPlayerStats();
        }
    }

    private void UpgradePlayerStats()
    {
        PlayerConfig config = currentPlayer.Config;
        config.Level++;
        config.MaxHealth++;
        config.MaxArmor++;
        config.MaxEnergy += 10f;
        config.CriticalChance += 2f;
        config.CriticalDamage += 5f;

        config.MaxHealth = Mathf.Min(config.MaxHealth, config.HealthMaxUpgrade);
        config.MaxArmor = Mathf.Min(config.MaxArmor, config.ArmorMaxUpgrade);
        config.MaxEnergy = Mathf.Min(config.MaxEnergy, config.EnergyMaxUpgrade);
        config.CriticalChance = Mathf.Min(config.CriticalChance, config.CriticalMaxUpgrade);

        float upgrade = config.UpgradeCost;
        config.UpgradeCost = upgrade + (upgrade * (config.UpgradeMultiplier / 100f));
    }

    private void ShowPlayerStats()
    {
        playerPanel.SetActive(true);
        playerIcon.sprite = currentPlayer.Config.Icon;
        playerName.text = currentPlayer.Config.Name;
        playerLevel.text = $"Level {currentPlayer.Config.Level}";
        playerHealthMaxStat.text = currentPlayer.Config.MaxHealth.ToString();
        playerArmorMaxStat.text = currentPlayer.Config.MaxArmor.ToString();
        playerEnergyMaxStat.text = currentPlayer.Config.MaxEnergy.ToString();
        playerCriticalMaxStat.text = currentPlayer.Config.CriticalChance.ToString();

        playerUnlockCostTMP.text = currentPlayer.Config.UnlockCost.ToString("0.00");
        playerUpgradeCostTMP.text = currentPlayer.Config.UpgradeCost.ToString("0.00");

        // Update Bars
        healthBar.fillAmount = currentPlayer.Config.MaxHealth
                               / currentPlayer.Config.HealthMaxUpgrade;
        armorBar.fillAmount = currentPlayer.Config.MaxArmor
                               / currentPlayer.Config.ArmorMaxUpgrade;
        energyBar.fillAmount = currentPlayer.Config.MaxEnergy
                               / currentPlayer.Config.EnergyMaxUpgrade;
        criticalBar.fillAmount = currentPlayer.Config.CriticalChance
                               / currentPlayer.Config.CriticalMaxUpgrade;
    }

    private void VerifyPlayer()
    {
        if (currentPlayer.Config.Unlocked == false)
        {
            upgradeButton.SetActive(false);
            selectButton.SetActive(false);
            unlockButton.SetActive(true);
        }
        else
        {
            upgradeButton.SetActive(true);
            selectButton.SetActive(true);
            unlockButton.SetActive(false);
        }
    }

    public void ClosePlayerPanel()
    {
        playerPanel.SetActive(false);
    }
}

[Serializable]
public class PlayerCreation
{
    public PlayerMovement Player;
    public Transform CreationPos;
}








// OLD Version
/*using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    [Header("Config")]
    [SerializeField] private PlayerCreation[] players;

    [Header("UI")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private Image playerIcon;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerHealthMaxStat;
    [SerializeField] private TextMeshProUGUI playerArmorMaxStat;
    [SerializeField] private TextMeshProUGUI playerEnergyMaxStat;
    [SerializeField] private TextMeshProUGUI playerCriticalMaxStat;
    [SerializeField] private TextMeshProUGUI coinsTMP;
    [SerializeField] private TextMeshProUGUI playerUpgradeCostTMP;
    [SerializeField] private TextMeshProUGUI playerUnlockCostTMP;

    [Header("Bars")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image armorBar;
    [SerializeField] private Image energyBar;
    [SerializeField] private Image criticalBar;
    
    [Header("Buttons")]
    [SerializeField] private GameObject unlockButton;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject selectButton;
    
    private SelectablePlayer currentPlayer;
    private bool playerSelected;
    
    private void Start()
    {
        CreatePlayer();
    }

    private void Update()
    {
        coinsTMP.text = CoinManager.Instance.Coins.ToString();
    }

    private void CreatePlayer()
    {
        for (int i = 0; i < players.Length; i++)
        {
            PlayerMovement player = Instantiate(players[i].Player, players[i].CreationPos.position,
                Quaternion.identity, players[i].CreationPos);
            player.enabled = false;
        }
    }

    public void ClickPlayer(SelectablePlayer selectablePlayer)
    {
        currentPlayer = selectablePlayer;
        VerifyPlayer();
        ShowPlayerStats();
    }

    public void SelectPlayer()
    {
        if (playerSelected) return;
        if (currentPlayer.Config.Unlocked)
        {
            currentPlayer.GetComponent<PlayerMovement>().enabled = true;
            currentPlayer.Config.ResetPlayerStats();
            GameManager.Instance.Player = currentPlayer.Config;
            ClosePlayerPanel();
            playerSelected = true;
        }
    }

    public void UpgradePlayer()
    {
        if (CoinManager.Instance.Coins >= currentPlayer.Config.UpgradeCost)
        {
            CoinManager.Instance.RemoveCoins(currentPlayer.Config.UpgradeCost);
            UpgradePlayerStats();
            ShowPlayerStats();
        }
    }

    public void UnlockPlayer()
    {
        if (CoinManager.Instance.Coins >= currentPlayer.Config.UnlockCost)
        {
            CoinManager.Instance.RemoveCoins(currentPlayer.Config.UnlockCost);
            currentPlayer.Config.Unlocked = true;
            VerifyPlayer();
            ShowPlayerStats();
        }
    }
    
    private void UpgradePlayerStats()
    {
        PlayerConfig config = currentPlayer.Config;
        config.Level++;
        config.MaxHealth++;
        config.MaxArmor++;
        config.MaxEnergy += 10f;
        config.CriticalChance += 2f;
        config.CriticalDamage += 5f;

        config.MaxHealth = Mathf.Min(config.MaxHealth, config.HealthMaxUpgrade);
        config.MaxArmor = Mathf.Min(config.MaxArmor, config.ArmorMaxUpgrade);
        config.MaxEnergy = Mathf.Min(config.MaxEnergy, config.EnergyMaxUpgrade);
        config.CriticalChance = Mathf.Min(config.CriticalChance, config.CriticalMaxUpgrade);
        
        float upgrade = config.UpgradeCost;
        config.UpgradeCost = upgrade + (upgrade * (config.UpgradeMultiplier / 100f));
    }
    
    private void ShowPlayerStats()
    {
        playerPanel.SetActive(true);
        playerIcon.sprite = currentPlayer.Config.Icon;
        playerName.text = currentPlayer.Config.Name;
        playerLevel.text = $"Level {currentPlayer.Config.Level}";
        playerHealthMaxStat.text = currentPlayer.Config.MaxHealth.ToString();
        playerArmorMaxStat.text = currentPlayer.Config.MaxArmor.ToString();
        playerEnergyMaxStat.text = currentPlayer.Config.MaxEnergy.ToString();
        playerCriticalMaxStat.text = currentPlayer.Config.CriticalChance.ToString();

        playerUnlockCostTMP.text = currentPlayer.Config.UnlockCost.ToString("0.00");
        playerUpgradeCostTMP.text = currentPlayer.Config.UpgradeCost.ToString("0.00");

        // Update Bars
        healthBar.fillAmount = currentPlayer.Config.MaxHealth 
                               / currentPlayer.Config.HealthMaxUpgrade;
        armorBar.fillAmount = currentPlayer.Config.MaxArmor 
                               / currentPlayer.Config.ArmorMaxUpgrade;
        energyBar.fillAmount = currentPlayer.Config.MaxEnergy 
                               / currentPlayer.Config.EnergyMaxUpgrade;
        criticalBar.fillAmount = currentPlayer.Config.CriticalChance 
                               / currentPlayer.Config.CriticalMaxUpgrade;
    }

    private void VerifyPlayer()
    {
        if (currentPlayer.Config.Unlocked == false)
        {
            upgradeButton.SetActive(false);
            selectButton.SetActive(false);
            unlockButton.SetActive(true);
        }
        else
        {
            upgradeButton.SetActive(true);
            selectButton.SetActive(true);
            unlockButton.SetActive(false);
        }
    }
    
    public void ClosePlayerPanel()
    {
        playerPanel.SetActive(false);
    }
}

[Serializable]
public class PlayerCreation
{
    public PlayerMovement Player;
    public Transform CreationPos;
}*/