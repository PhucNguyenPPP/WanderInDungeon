using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform weaponPos;

    private PlayerActions actions;
    private PlayerEnergy playerEnergy; 
    private PlayerMovement playerMovement;
    private Weapon currentWeapon;

    private Coroutine weaponCoroutine;
    private ItemText weaponNameText;
    
    private int weaponIndex; // 0 - 1
    private Weapon[] equippedWeapons = new Weapon[2];
    
    private void Awake()
    {
        actions = new PlayerActions();
        playerEnergy = GetComponent<PlayerEnergy>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        actions.Weapon.Shoot.performed += ctx => ShootWeapon();
        actions.Interactions.ChangeWeapons.performed += ctx => ChangeWeapon();
    }

    private void Update()
    {
        if (currentWeapon == null) return;
        if (playerMovement.MoveDirection != Vector2.zero)
        {
            RotateWeapon(playerMovement.MoveDirection);
        }
    }

    private void CreateWeapon(Weapon weaponPrefab)
    {
        currentWeapon = Instantiate(weaponPrefab, weaponPos.position,
            Quaternion.identity, weaponPos);
        equippedWeapons[weaponIndex] = currentWeapon;
        ShowCurrentWeaponName();
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (equippedWeapons[0] == null)
        {
            CreateWeapon(weapon);
            return;
        }

        if (equippedWeapons[1] == null)
        {
            weaponIndex++;
            equippedWeapons[0].gameObject.SetActive(false);
            CreateWeapon(weapon);
            return;
        }
        
        // Destroy current weapon
        currentWeapon.DestroyWeapon();
        equippedWeapons[weaponIndex] = null;
        
        // Create new weapon
        CreateWeapon(weapon);
    }

    private void ChangeWeapon()
    {
        if (equippedWeapons[1] == null) return;
        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            equippedWeapons[i].gameObject.SetActive(false);
        }

        weaponIndex = 1 - weaponIndex;
        currentWeapon = equippedWeapons[weaponIndex];
        currentWeapon.gameObject.SetActive(true);
        ResetWeaponForChange();
        ShowCurrentWeaponName();
    }
    
    private void ShootWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }

        if (CanUseWeapon() == false)
        {
            return;
        }
        
        currentWeapon.UseWeapon();
        playerEnergy.UseEnergy(currentWeapon.ItemWeapon.RequiredEnergy);
    }
    
    private void RotateWeapon(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (direction.x > 0f) // Facing Right
        {
            weaponPos.localScale = Vector3.one;
            currentWeapon.transform.localScale = Vector3.one;
        }
        else // Facing Left
        {
            weaponPos.localScale = new Vector3(-1, 1, 1);
            currentWeapon.transform.localScale = new Vector3(-1, -1, 1);
        }
        
        currentWeapon.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private void ShowCurrentWeaponName()
    {
        if (weaponCoroutine != null)
        {
            StopCoroutine(weaponCoroutine);
        }

        if (weaponNameText != null && weaponNameText.gameObject.activeInHierarchy)
        {
            Destroy(weaponNameText.gameObject);
        }

        weaponCoroutine = StartCoroutine(IEShowName());
    }
    
    private IEnumerator IEShowName()
    {
        Vector3 textPos = transform.position + Vector3.up;
        Color weaponNameColor = GameManager.Instance.
            GetWeaponNameColor(currentWeapon.ItemWeapon.Rarity);
        weaponNameText = ItemTextManager.Instance
            .ShowMessage(currentWeapon.ItemWeapon.ID, weaponNameColor, 
                textPos);
        weaponNameText.transform.SetParent(transform);
        yield return new WaitForSeconds(2f);
        Destroy(weaponNameText.gameObject);
    }
    
    private bool CanUseWeapon()
    {
        if (currentWeapon.ItemWeapon.WeaponType == WeaponType.Gun && 
            playerEnergy.CanUseEnergy)
        {
            return true;
        }

        if (currentWeapon.ItemWeapon.WeaponType == WeaponType.Melee)
        {
            return true;
        }

        return false;
    }

    private void ResetWeaponForChange()
    {
        Transform weaponTransform = currentWeapon.transform;
        weaponTransform.rotation = Quaternion.identity;
        weaponTransform.localScale = Vector3.one;
        weaponPos.rotation = Quaternion.identity;
        weaponPos.localScale = Vector3.one;
        playerMovement.FaceRightDirection();
    }
    
    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }
}
