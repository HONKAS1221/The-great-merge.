using UnityEngine;

public class Target : MonoBehaviour
{
    public float MaxHealth = 20f;
    private float CurrentHealth;


    public bool HasTakenDamage { get; set; }
    public float health = 50f;

    [SerializeField] public AudioClip damageSoundC;
    [SerializeField] public AudioClip deathSoundC;




    void Start()
    {
        CurrentHealth = MaxHealth;
    }
    public void takeDamage(float amount)
    {

        SoundFX_Manager.instance.PlaySoundFXClip(damageSoundC, transform, 1f);

        CurrentHealth -= amount;
        Debug.Log(CurrentHealth);
        if (CurrentHealth <= 0f)
        {
            
            SoundFX_Manager.instance.PlaySoundFXClip(deathSoundC, transform, 1f);
        }
    }

    private void Update()
    {
        if (transform.position.y <= -20)
        {
            Destroy(gameObject);
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}
