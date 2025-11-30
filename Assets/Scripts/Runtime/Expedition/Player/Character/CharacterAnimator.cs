using System;
using Unity.Netcode;
using UnityEngine;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class CharacterAnimator : MonoBehaviour {
        public Animator animator;
        public float damping;

        public bool Moving {
            set => animator.SetBool("Moving", value);
        }
        
        public Vector2 MovementInput {
            set {
                Vector2 current = new Vector2(animator.GetFloat("Move X"), animator.GetFloat("Move Y"));
                Vector2 next = Vector2.MoveTowards(current, value, Time.deltaTime * damping);
                
                animator.SetFloat("Move X", next.x);
                animator.SetFloat("Move Y", next.y);
            }
        }
    }
}