using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // EventTrigger

public class PlayerController : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer body;

    [Header("Control")]
    public bool canMove = true;

    private Vector2 movement;

    [Header("Mobile Buttons")]
    public EventTrigger upButton;
    public EventTrigger downButton;
    public EventTrigger leftButton;
    public EventTrigger rightButton;

    private Vector2 mobileInput = Vector2.zero;

    // 数据存储
    private CharacterData data;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        BindButton(upButton, Vector2.up);
        BindButton(downButton, Vector2.down);
        BindButton(leftButton, Vector2.left);
        BindButton(rightButton, Vector2.right);
    }

    void BindButton(EventTrigger trigger, Vector2 dir)
    {
        if (trigger == null) return;

        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { mobileInput = dir; });
        trigger.triggers.Add(pointerDown);

        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { mobileInput = Vector2.zero; });
        trigger.triggers.Add(pointerUp);
    }

    // ✅ ApplyCharacter 一定要 public，别改名字
    public void ApplyCharacter(CharacterData c)
    {
        data = c;
        if (body != null && c.avatar != null)
            body.sprite = c.avatar;
        // 其他属性可扩展
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            if (animator != null)
                animator.SetBool("IsMoving", false);
            return;
        }

        Vector2 keyboardInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movement = keyboardInput != Vector2.zero ? keyboardInput : mobileInput;

        if (animator != null)
        {
            if (movement != Vector2.zero)
            {
                animator.SetFloat("LastMoveX", movement.x);
                animator.SetFloat("LastMoveY", movement.y);
            }
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetBool("IsMoving", movement.sqrMagnitude > 0);
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        if (rb != null)
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

public void MoveUp()    { mobileInput = Vector2.up; }
public void MoveDown()  { mobileInput = Vector2.down; }
public void MoveLeft()  { mobileInput = Vector2.left; }
public void MoveRight() { mobileInput = Vector2.right; }
public void StopMove()  { mobileInput = Vector2.zero; }

}

