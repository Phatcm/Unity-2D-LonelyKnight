using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adventure2D{
    
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Animator _anim;
        [SerializeField] Rigidbody2D _rigid2D;
        [SerializeField] float _movementSpeed;
        [SerializeField] EPlayerState _state;
        [SerializeField] CheckGround _checkGround;
        [SerializeField] float _jumpCooldown;
        [SerializeField] EPJumpState _jumpState;
        [SerializeField] LayerMask _targetMask;
        [SerializeField] Transform _damageObj;
        [SerializeField] Vector2 _attackSize;

        [SerializeField] int _damage;

        float _axis;
        bool _canAttack;
        float _jumpTime;
        

        public EPlayerState State{
            get => _state;
            set{
                _state = value;
                switch (_state)
                {
                    case EPlayerState.Idle:
                        _anim.SetBool("isIdle",true);
                        _canAttack = true;
                        break;
                    case EPlayerState.MoveRight:
                        _anim.SetBool("isIdle",false);
                        if(_jumpState == EPJumpState.OnAir){
                            _anim.Play("Player_jump");
                        }
                        _canAttack = true;
                        break;
                    case EPlayerState.MoveLeft:
                        if(_jumpState == EPJumpState.OnAir){
                            _anim.Play("Player_jump");
                        }
                        _anim.SetBool("isIdle",false);
                        _canAttack = true;
                        break;
                    case EPlayerState.Jump:
                        _anim.SetBool("isIdle",false);
                        _canAttack = false;
                        break;
                    case EPlayerState.Attack:
                        _anim.SetBool("isIdle",false);
                        _anim.SetTrigger("Attack");
                        _canAttack = false;
                        break;
                    default:
                        break;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }
        private void Awake() {
            
        }
        // Update is called once per frame
        // Movement
        void Update()
        {
            _axis = Input.GetAxis("Horizontal");
            if (State != EPlayerState.Attack){
                if(_axis > 0){
                    FaceScaling(1);
                    State = EPlayerState.MoveRight;
                }
                else if (_axis < 0){
                    FaceScaling(-1);
                    State = EPlayerState.MoveLeft;
                    
                }
                else{
                    if(_jumpState == EPJumpState.OnGround)
                        State = EPlayerState.Idle;
                    else if (_jumpState == EPJumpState.OnAir)
                        _anim.Play("Player_jump");
                }
            }
                
            bool isJump = Input.GetKeyDown(KeyCode.Space);
            if(isJump && _checkGround.IsGrounded && _jumpTime<=0){
                State = EPlayerState.Jump;
            }
            bool isAttack = Input.GetKeyDown(KeyCode.L);
            if(isAttack && _canAttack){
                State = EPlayerState.Attack;
                _canAttack = false;
                
            }
            if(_jumpTime>0) _jumpTime -= Time.deltaTime;

            _anim.SetBool("isGrounded", _checkGround.IsGrounded );
            if(_checkGround.IsGrounded){
                _jumpState = EPJumpState.OnGround;
            }else{
                _jumpState = EPJumpState.OnAir;
            }
        }
        private void handle_EndAttack(){
            State = EPlayerState.Idle;
        }
        private void handle_StartAttack(){
            //Debug.Log("Attack");
            
            var hit = Physics2D.BoxCast(_damageObj.position, _attackSize, 0f, Vector2.zero, 1.0f, _targetMask );
            if(hit.collider != null){
                Debug.Log($"{hit.collider.name} take damage");
                if(hit.collider.CompareTag("Enemy/Skeleton")){
                    var enemy = hit.collider.GetComponent<EnemyController>();
                    enemy.TakeDame(_damage);
                }
            }
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(_damageObj.position, _attackSize);
        }

        private void FaceScaling(int value){
            var scale = transform.localScale;
            scale.x = value;
            transform.localScale = scale;
        }

        private void FixedUpdate(){
            UpdateMovement();
        }
        private void UpdateMovement() {
            switch(State){
                case EPlayerState.Idle:
                    var vel = _rigid2D.velocity;
                    if(vel.x != 0f){
                        vel.x = Mathf.Lerp(vel.x, 0, Time.fixedDeltaTime);
                    }
                    _rigid2D.velocity = vel;
                    //_rigid2D.velocity = new Vector2(0f, _rigid2D.velocity.y); //chua co giam dan
                    break;
                case EPlayerState.MoveRight:
                    _rigid2D.velocity = new Vector2(_movementSpeed * _axis * Time.fixedDeltaTime, _rigid2D.velocity.y);
                    //var velRight = _rigid2D.velocity;
                    //velRight += Vector2.right * _axis * _movementSpeed * Time.deltaTime;
                    //_rigid2D.velocity = velRight;
                    break;
                case EPlayerState.MoveLeft:
                    _rigid2D.velocity = new Vector2(_movementSpeed * _axis * Time.fixedDeltaTime, _rigid2D.velocity.y);
                    //var velLeft = _rigid2D.velocity;
                    //velLeft += Vector2.right * _axis * _movementSpeed * Time.deltaTime;
                    //_rigid2D.velocity = velLeft;
                    break;
                case EPlayerState.Jump:
                    _rigid2D.velocity = Vector2.zero;
                    _rigid2D.AddForce(new Vector2(0f,3f), ForceMode2D.Impulse);
                    _jumpTime = _jumpCooldown;
                    break;
                case EPlayerState.Attack:
                    break;
                default:
                    break;
                
            }
        }
        
        public enum EPlayerState
        {
            Idle = 0,
            MoveRight = 1,
            MoveLeft = 2,
            Jump = 3,
            Attack = 4,
        }

        public enum EPJumpState
        {
            OnGround = 0,
            OnAir = 1,
        }

    }
}
