using System;
using Game.Common;
using Game.Shared;
using UnityEngine;

namespace Game.CharacterControl
{
    public class AiController : CharacterContrellerBase
    {
        [SerializeField] private int _damageAmount;
        [SerializeField] private Collider2D _collider;

        private float _restTime;
        private PlayerController _target;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (_target == null) _target = GameManager.Instance.Player;

            if (IsTakingDamage == false)
            {
                // 朝角色移动
                if (_restTime > 0)
                {
                    _restTime -= Time.deltaTime;
                    MoveVector = Vector2.zero;
                }
                else
                {
                    MoveVector = (_target.Position - Position).normalized;
                }
                
                Rigidbody.MovePosition(Position + MoveVector * MoveSpeed * Time.deltaTime);
            }
            
            UpdateAnimation();
        }

        protected override void Death()
        {
            base.Death();
            
            _collider.enabled = false;
            Rigidbody.Sleep();

            BodySprite.sortingOrder = -100;
            StartCoroutine(Helper.DelayToInvoke(() =>
            {
                Animator.enabled = false;
            }, 0.5f));

            enabled = false;
            
            GameManager.Instance.OnKillEnemy();
        }

        private void DisableAnimator()
        {
            Animator.enabled = false;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(_target.tag))
            {
                var playerPos = _target.Position;
                var myPos = Rigidbody.position;
                var knockDirection = myPos - playerPos;
                
                var player = col.gameObject.GetComponent<CharacterContrellerBase>();
                player.KnockBack(-knockDirection, 500);
                player.TakeDamage(_damageAmount);
                
                KnockBack(knockDirection, 100);

                _restTime = 1;
            }
        }
    }
}