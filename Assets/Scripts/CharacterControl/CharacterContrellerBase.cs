using UnityEngine;

namespace Game.CharacterControl
{
    public abstract class CharacterContrellerBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected int MaxHealth = 100;
        [SerializeField] protected float MoveSpeed = 5;
        [SerializeField] protected Rigidbody2D Rigidbody;
        [SerializeField] protected Animator Animator;
        [SerializeField] protected AnimationEvent AnimationEvent;
        [SerializeField] protected SpriteRenderer BodySprite;
        [SerializeField] protected Color DeathColor;

        private static readonly int IsMovingParameter = Animator.StringToHash("IsMoving");
        private static readonly int BeHitParameter = Animator.StringToHash("BeHit");

        protected Vector2 MoveVector;
        protected int CurrentHealth;

        protected bool DamageTrigger;
        protected bool IsTakingDamage;

        public bool IsDied { get; protected set; }

        public Vector2 Position => Rigidbody.position;

        protected virtual void Awake()
        {
            CurrentHealth = MaxHealth;
            AnimationEvent.OnHitAnimationFinshed += OnHitAnimationFinshed;
        }

        protected virtual void FixedUpdate()
        {
            
        }

        protected void UpdateAnimation()
        {
            if (IsDied) return;
            
            if (DamageTrigger)
            {
                DamageTrigger = false;
                Animator.SetTrigger(BeHitParameter);
            }
                
            Animator.SetBool(IsMovingParameter, MoveVector.sqrMagnitude > 0);
        }

        public void KnockBack(Vector2 direction, int force)
        {
            var knockbackForce = direction.normalized * force;
            Rigidbody.AddForce(knockbackForce);
        }

        public virtual void TakeDamage(int damageAmount)
        {
            CurrentHealth -= damageAmount;
            IsTakingDamage = true;
            DamageTrigger = true;

            if (CurrentHealth <= 0)
            {
                Death();
            }
        }

        protected virtual void Death()
        {
            IsDied = true;
            
            BodySprite.color = DeathColor;
        }

        private void OnHitAnimationFinshed()
        {
            IsTakingDamage = false;
        }
    }
}