using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BaseCharacterClass : MonoBehaviour
{
    public int currentHealth;
    public bool CanBeKnockedBack;
    public AudioMixerGroup SFXMixer;
    
    protected AudioClip footstepsAudio;
    protected  AudioClip attackAudio;
    protected  AudioClip hurtAudio;
    protected  AudioClip deathAudio;


    protected AudioSource footstepsAudioSource;
    protected AudioSource attackAudioSource;
    protected AudioSource hurtAudioSource;
    protected AudioSource deathAudioSource;
    
    protected enum AttackStyle
    {
        SWORD,
        BOW,
        SPEAR,
    }
    
    [SerializeField]
    protected AttackStyle _attackStyle;
    
    public CharacterScriptableObject _characterScriptableObject;
    
    

    //This is for damage pop-ups on player and AI alike
    [SerializeField] private OnDmgPopup dmgPopup;
    
    
    //Sets up audio objects correctly
    //Cannot be modified in inspector this way, but keeps characters consistent
    private void Awake()
    {
        footstepsAudioSource = gameObject.AddComponent<AudioSource>();
        footstepsAudioSource.outputAudioMixerGroup = SFXMixer;
        footstepsAudioSource.playOnAwake = false;
        footstepsAudioSource.loop = false;
        footstepsAudioSource.volume = 0.4f;
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.playOnAwake = false;
        attackAudioSource.loop = false;
        attackAudioSource.outputAudioMixerGroup = SFXMixer;
        hurtAudioSource = gameObject.AddComponent<AudioSource>();
        hurtAudioSource.playOnAwake = false;
        hurtAudioSource.loop = false;
        hurtAudioSource.outputAudioMixerGroup = SFXMixer;
        deathAudioSource = gameObject.AddComponent<AudioSource>();
        deathAudioSource.playOnAwake = false;
        deathAudioSource.loop = false;
        deathAudioSource.outputAudioMixerGroup = SFXMixer;
    }
    
    
    // Start is called before the first frame update 
    protected virtual void Start()
    {
        currentHealth = (int)_characterScriptableObject.maxHealth.BaseValue; // current health becomes base health stat
        
        #region Audio

        if (footstepsAudio != null)
        {
            footstepsAudio = _characterScriptableObject.Footsteps;
            footstepsAudioSource.clip = footstepsAudio;
        }

        attackAudio = _characterScriptableObject.Attack;
        attackAudioSource.clip = attackAudio;

        hurtAudio = _characterScriptableObject.Hurt;
        hurtAudioSource.clip = hurtAudio;

        deathAudio = _characterScriptableObject.Death;
        deathAudioSource.clip = deathAudio;
        
        #endregion
    }




 



    #region General Character Functions
    
        #region Getters
            /// <summary>
            /// Gets the max health of the character - includes any modifiers.
            /// </summary>
            /// <returns>Modified max health value.</returns>
            public int GetMaxHealth()
            {
                return (int)_characterScriptableObject.maxHealth.Value;
            }
        
            /// <summary>
            /// Gets the current health of the character.
            /// </summary>
            /// <returns>Character's current health.</returns>
            public int GetCurrentHealth()
            {
                return currentHealth;
            }

            /// <summary>
            /// Gets the attack power of the character.
            /// </summary>
            /// <returns>The modified attack power of the character.</returns>
            public float GetAttackPower()
            {
                return _characterScriptableObject.attackPower.Value;
            }

            public float GetAttackSpeed()
            {
                return _characterScriptableObject.attackSpeed.Value;
            }
            
            /// <summary>
            /// Gets the speed of the character. 
            /// </summary>
            /// <returns>The modified speed of the character.</returns>
            public float GetSpeed()
            {
                return _characterScriptableObject.speed.Value;
            }
        #endregion

        #region Setters

        public void SetSpeed(float speed)
        {
            _characterScriptableObject.speed.Value = speed;
        }

        #endregion

        /// <summary>
        /// Adds health to the character.
        /// </summary>
        /// <param name="addedHealth">The health to add to the character's current health.</param>
        /// <returns>The new current health.</returns>
        public virtual int AddHealth(int addedHealth)
        {
            currentHealth += addedHealth;
            if (currentHealth > GetMaxHealth())
            {
                addedHealth = GetMaxHealth() - currentHealth;
                currentHealth = GetMaxHealth();
            }
            OnDmgPopup popup = OnDmgPopup.Create(transform.position, addedHealth, Color.green); // creates the dmg popup 

            return currentHealth;
        }
        
        /// <summary>
        /// Damages the character.
        /// </summary>
        /// <param name="damage">The damage to decrease the health by.</param>
        public virtual void TakeHealth(int damage)
        {
            OnDmgPopup popup = OnDmgPopup.Create(transform.position, damage, Color.red); // creates the dmg popup 
            currentHealth -= damage;
        }

        /// <summary>
        /// Checks to see if the character is dead.
        /// </summary>
        /// <returns>Whether the character is dead.</returns>
        public bool IsDead()
        {
            return currentHealth <= 0;
        }
        public void ApplyKnockback(Vector3 dir, float mult)
        {
            if(!CanBeKnockedBack) return;
            GetComponent<Rigidbody>().AddForce(dir.normalized * mult, ForceMode.Impulse);
            Wait(0.3f, () => GetComponent<Rigidbody>().velocity = Vector3.zero);
        }

    
    #endregion

    #region AI Declarations

        /// <summary>
        /// Gets the sight of the AI agent. Does not affect player.
        /// </summary>
        /// <returns>Modified sight value.</returns>
        public virtual float GetSight()
        {
            return _characterScriptableObject.sight.Value;
        }
    
    #endregion
    
    
    
    //Utility methods that allow for actions to be delayed
    public void Wait(float seconds, Action action){
        StartCoroutine(_wait(seconds, action));
    }
    IEnumerator _wait(float time, Action callback){
        yield return new WaitForSeconds(time);
        callback();
    }
}
