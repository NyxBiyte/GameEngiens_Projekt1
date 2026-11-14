using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public String playerName;

    private float sneekHeightFactor = 0.6f;
    private float sneekSpriteOffset = -0.2f;

    public float moveSpeed = 2f;
    public float jumpForce = 5f;

    public bool grounded = false;
    public bool isSprinting = false;
    public bool isSneeking = false;

    private float moveInput;

    private Vector2 _standSize, _standOffset;
    private Vector3 _spriteStartPos;

    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _collider2D;
    private SpriteRenderer _sprite;
    private Animator _animator;
    public ParticleSystem _dustParticleSystem;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Material _flashMaterial;
    private Material _originalMaterial;

    public TMP_Text coinCountText;

    [SerializeField] private AudioSource _audioSourceDamage;
    [SerializeField] private AudioSource _audioSourceCoin;
    private AudioSource _audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<BoxCollider2D>();

        _sprite = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();

        if (_collider2D != null)
        {
            _standSize = _collider2D.size;
            _standOffset = _collider2D.offset;
        }

        _spriteStartPos = _sprite.transform.localPosition;
        _originalMaterial = _spriteRenderer.material;

        _audioSource = GetComponent<AudioSource>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
        _animator.SetFloat("Speed", Mathf.Abs(moveInput));

        if (moveInput > 0) _sprite.flipX = false;
        else if (moveInput < 0) _sprite.flipX = true;
    }

    

    

    public void OnJump(InputValue value)
    {
        if (value.isPressed && grounded)
        {
            if (isSprinting)
            {
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpForce + 2);
            }else if (isSneeking)
            {
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpForce - 2);
            }else{
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpForce);
            }
            _animator.SetTrigger("Jump");
            if (_dustParticleSystem != null)
            {
                _dustParticleSystem.Play();
            }
            
        }
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
        _animator.SetBool("Sprint", isSprinting);
    }

    public void OnSneek(InputValue value)
    {
        isSneeking = value.isPressed;
        _animator.SetBool("Sneek", isSneeking);
        SetSneekVisual(isSneeking);
        SetSneekCollider(isSneeking);
    }
    
    void FixedUpdate()
    {
        coinCountText.text = GameManager.Instance.coinCount.ToString();
        float multiplier = isSprinting ? 1.5f : isSneeking ? 0.5f : 1f;
        _rigidbody2D.linearVelocity = new Vector2(moveInput * moveSpeed * multiplier, _rigidbody2D.linearVelocity.y);
    }
    
    private void SetSneekVisual(bool sneeking)
    {
        _sprite.transform.localPosition =
            _spriteStartPos + (sneeking ? new Vector3(0f, sneekSpriteOffset, 0f) : Vector3.zero);
    }

    private void SetSneekCollider(bool sneeking)
    {
        Vector2 size = _standSize;
        Vector2 offset = _standOffset;

        if (sneeking)
        {
            size.y = _standSize.y * sneekHeightFactor;
            // Unterkante bleibt auf gleicher Höhe, sonst schwebt der Spieler
            offset.y = _standOffset.y - (_standSize.y - size.y) / 2f;
        }

        if (_collider2D != null) { _collider2D.size = size;     _collider2D.offset = offset; }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                grounded = true;
                return;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance == null)
            {
                Debug.LogError("Kein GameManager in der Szene. Starte über die MenuScene.");
                return;
            }
        if (collision.gameObject.CompareTag("Death"))
        {
            DamageEffect.Instance.Play();
            StartCoroutine(FlashEffect(0.2f));
            _audioSourceDamage.PlayOneShot(_audioSourceDamage.clip);
            if (GameManager.Instance.playerLives > 1)
            {
                GameManager.Instance.playerLives--;
                GameManager.Instance.RespawnPlayer();
            }
            else
            {
                GameManager.Instance.GameOver();
            }
        }

        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            GameManager.Instance.LevelComplete();
        }else if (other.CompareTag("Trap"))
        {
            DamageEffect.Instance.Play();
            StartCoroutine(FlashEffect(0.2f));
            if (GameManager.Instance.playerLives > 0.5f)
            {
                GameManager.Instance.playerLives -= 0.5f;
            }
            else
            {
                GameManager.Instance.GameOver();
            }
        }else if (other.CompareTag("Coin"))
        {
            GameManager.Instance.coinCount++;
            _audioSourceCoin.PlayOneShot(_audioSourceCoin.clip);
            Destroy(other.gameObject);
        }
    }

    IEnumerator FlashEffect(float duration)
    {
        _spriteRenderer.material = _flashMaterial;
        yield return new WaitForSeconds(duration);
        _spriteRenderer.material = _originalMaterial;
    }

}
