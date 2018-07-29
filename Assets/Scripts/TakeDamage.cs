using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    private Stats stats;
    private PlayerEquipement playerEquipement;
    public Transform LifeHud;

    public bool isPlayer;
    private int maxHealth = 200;
    public int currentHealth { get; private set; }

    public bool isDead { get; private set; }

    public delegate void HealHandler();
    public delegate void HitHandler(Transform other);
    public delegate void DieHandler();
    public event HealHandler Heal;
    public event HitHandler Hit;
    public event DieHandler Die;

    public int damageCooldown = 10;
    private int baseDamageCooldown;

    public void HealBy(int health)
    {
        if (isDead || currentHealth == maxHealth) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + health);
        if (Heal != null) Heal();
    }

    private void TakeHit(MakeDamage damageMaker, Transform other)
    {
        if (isDead) return;

        int resistance = stats.Defence;

        if (isPlayer)
        {
            Stuff head = playerEquipement.head;
            Stuff torso = playerEquipement.torso;
            Stuff boots = playerEquipement.boots;

            if (head != null)
                resistance += head.GetComponent<Stats>().Defence;
            if (torso != null)
                resistance += torso.GetComponent<Stats>().Defence;
            if (boots != null)
                resistance += boots.GetComponent<Stats>().Defence;
        }

        stats.Life = damageMaker.isKiller ? 0 : Mathf.Max(0, stats.Life - (damageMaker.damage - resistance));

        currentHealth = stats.Life;

        if (!isPlayer)
        {
            LifeHud.GetComponent<LifeHud>().SetLife((float)stats.Life / (float)stats.MaxLife);
        }
        else
        {
            PlayerLifeHud.SetLife(stats.MaxLife, stats.Life);
        }

        if (Hit != null) Hit(other);

        if (currentHealth == 0)
        {
            isDead = true;
            if (!isPlayer)
                LifeHud.gameObject.SetActive(false);
            if (Die != null) Die();
        }
    }

    private void Start()
    {
        baseDamageCooldown = damageCooldown;
        damageCooldown = 0;
        stats = GetComponent<Stats>();
        playerEquipement = GetComponent<PlayerEquipement>();
        currentHealth = stats.MaxLife;
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        damageCooldown--;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.tag);
        //Debug.Log(other.name);
        if (other.CompareTag("Weapon") && damageCooldown <= 0)
        {
            damageCooldown = baseDamageCooldown;
            MakeDamage damageMaker = other.GetComponent<MakeDamage>();

            if (damageMaker == null)
            {
                Debug.LogWarningFormat("Missing MakeDamage on {0}", other.name);
                return;
            }

            TakeHit(damageMaker, other.transform);
            other.GetComponent<Collider2D>().enabled = false;

            if (damageMaker.isUnique)
                Destroy(damageMaker.gameObject);

            return;
        }
    }
}
