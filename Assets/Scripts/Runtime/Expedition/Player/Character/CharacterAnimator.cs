using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class CharacterAnimator : NetworkBehaviourExtended {
        public Animator animator;
        public float damping;

        private readonly NetworkVariable<bool> _swimming = new(writePerm: NetworkVariableWritePermission.Owner);
        private readonly NetworkVariable<bool> _moving = new(writePerm: NetworkVariableWritePermission.Owner);
        private readonly NetworkVariable<Vector2> _movement = new(writePerm: NetworkVariableWritePermission.Owner);

        public bool Swimming {
            set => _swimming.Value = value;
        }
        
        public bool Moving {
            set => _moving.Value = value;
        }
        
        public Vector2 MovementInput {
            set {
                Vector2 current = new Vector2(animator.GetFloat("Move X"), animator.GetFloat("Move Y"));
                Vector2 next = Vector2.MoveTowards(current, value, Time.deltaTime * damping);
                _movement.Value = next;
            }
        }

        private void Update() {
            animator.SetBool("Moving", _moving.Value);
            animator.SetFloat("Move X", _movement.Value.x);
            animator.SetFloat("Move Y", _movement.Value.y);
            animator.SetBool("Swimming", _swimming.Value);
        }
    }
}