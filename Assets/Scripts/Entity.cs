using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{

    #region Components
    public UI ui { get; private set; }
    public Animator anim { get; private set; }
    public Rigidbody2D rb {get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd {get; private set; }
    #endregion
    
    [Header("Knockback Info")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    #region Collision Info
    [Header("Collision Info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsPlatform;
    #endregion

    public int facingDir {get; private set; } = 1;
    protected bool facingRight = true;
    public bool isDead = false;

    public System.Action onFlipped = delegate {};

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
        ui = GameObject.Find("Canvas").GetComponent<UI>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageEffect(bool _knockback)
    {
        if(_knockback)
            StartCoroutine("HitKnockback");  
    }

    protected virtual IEnumerator HitKnockback()
    {
        // if (GetComponent<Player>() != null)
        //     yield break;

        isKnocked = true;
        int modifier = 1;
        if (facingDir == 1 && transform.position.x > PlayerManager.instance.player.transform.position.x ||
            facingDir == -1 && transform.position.x < PlayerManager.instance.player.transform.position.x)
            modifier = -1;
        rb.velocity = new Vector2(knockbackDirection.x * -facingDir * modifier, knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;

        ZeroVelocity();
    }

    #region Velocity
    public void ZeroVelocity()
    {
        if(isKnocked)
            return;
            
        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if(isKnocked)
            return;
        
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Colllision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsPlatformDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsPlatform);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance , whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        onFlipped();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip(); 
    }
    #endregion

    public virtual void Die()
    {

    }
}
