using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerMover : MonoBehaviour
{
    private const string Horizontal = nameof(Horizontal);
    private const string Vertical = nameof(Vertical);
    private const string Speed = nameof(Speed);

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private PlayerStats _playerStats;

    private Vector2 _direction;
    private float _moveX;
    private float _moveY;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerStats = GetComponent<PlayerStats>();

        if (_playerStats == null)
            Debug.LogError("PlayerStats не найден на игроке!");
    }

    private void Update()
    {
        _moveX = Input.GetAxisRaw(Horizontal);
        _moveY = Input.GetAxisRaw(Vertical);

        _direction = new Vector2(_moveX, _moveY).normalized;

        float animSpeed = _direction.magnitude;
        if (_playerStats != null && _playerStats.isSprinting)
        {
            animSpeed *= 2; 
        }
        _animator.SetFloat(Speed, animSpeed);

        if (_moveX != 0)
        {
            _spriteRenderer.flipX = _moveX < 0;
        }
    }

    private void FixedUpdate()
    {
        if (_playerStats != null)
        {
            float currentMoveSpeed = _playerStats.currentSpeed;
            _rigidbody2D.linearVelocity = _direction * currentMoveSpeed * Time.fixedDeltaTime;

            if (_direction.magnitude > 0)
            {
                string speedType = _playerStats.isSprinting ? "БЕГ" : "ходьба";
                Debug.Log($"{speedType}: скорость {currentMoveSpeed}");
            }
        }
        else
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float speed = isSprinting ? 1000f : 300f;
            _rigidbody2D.linearVelocity = _direction * speed * Time.fixedDeltaTime;
        }
    }
}
/*using UnityEngine;
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerMover : MonoBehaviour
{
    private const string Horizontal = nameof(Horizontal);
    private const string Vertical = nameof(Vertical);
    private const string Speed = nameof(Speed);


    [SerializeField] private float _speed;


    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector2 _direction;
    private float _minSpeed = 0;
    private float _maxSpeed = 5;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw(Horizontal) * _speed * Time.fixedDeltaTime;
        float moveY = Input.GetAxisRaw(Vertical) * _speed * Time.fixedDeltaTime;
        _direction = new Vector2(moveX, moveY).normalized;
        _animator.SetFloat(Speed, _direction.magnitude * _speed * Time.fixedDeltaTime);

        if(moveX != 0)
        {
            _spriteRenderer.flipX = moveX < 0;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _direction * _speed * Time.fixedDeltaTime;

        if (_rigidbody2D.linearVelocity.x != 0)
        {
            _animator.SetFloat(Speed, _maxSpeed);
        }
        else
        {
            _animator.SetFloat(Speed, _minSpeed);
        }
    }

}*/
