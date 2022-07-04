using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adventure2D
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] int _hp;
        [SerializeField] Animator _anim;
        bool _isDead;
        public int Hp 
        { 
            get => _hp;
            private set
            {
                if (value < 0) return;
                _hp = value;
                _isDead = _hp == 0;
                if (_isDead)
                    //_anim.SetTrigger("Dead");
                    Debug.Log("skeleton is dead");
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
           if (_isDead) return;
        }
        

        public void TakeDame(int _damg){
            Hp -= _damg;
        }
    }
}
