using System;
using UnityEngine;

namespace Weapons
{
    public class PlayerProjectile : Projectile
    {
        private Vector3 _speedVector;
        [SerializeField] private float _gravity = -9.81f;
        
        protected override void SendFlying(Vector3 direction, float speed)
        {
            _speedVector = direction.normalized * speed;
        }

        private void FixedUpdate()
        {
            transform.position += _speedVector * Time.fixedDeltaTime;
            transform.position += Vector3.up * (_gravity * Time.fixedDeltaTime);
        }
    }
}