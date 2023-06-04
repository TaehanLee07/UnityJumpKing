using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    public float chargingTime;

    CapsuleCollider2D capsuleCollider;
    Rigidbody2D rigid;
    SpriteRenderer spriteRender;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")) // ���� ��ư�� �Է����� �� && ���� �ִϸ��̼��� ����ǰ� ���� ���� �� (���� �����ϰ� ���� ���� ��)
        {
            // �Լ��� ����Ǿ��� �� �������� ������ �ڵ�
            chargingTime = 0; // �����̽� �ٸ� ���� �ð� �ʱ�ȭ
        }
        if (Input.GetButtonUp("Jump") && !anim.GetBool("isJumping")) //�� ��ư�� �Է����� �� && ���� �ִϸ��̼��� ����ǰ� ���� ���� �� (���� �����ϰ� ���� ���� ��) && �ȱ� �ִϸ��̼��� ����ǰ� ���� ���� �� (���� �Ȱ� ���� ���� ��)
        {
            if (chargingTime >= 1.0f) // �⸦ ���� ���� 1���� Ŭ �� (���� ��ư�� ���� �ð��� ���� ������ ���⸦���� �ϱ� ���� ����)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower * 2.0f);        // ������ ���⸦ �ٸ��� ���ְ�
                anim.SetBool("isJumping", true);                                  // �����ϴ� �ִϸ��̼��� ������
            }

            else if (chargingTime >= 0.5f)
            {
                rigid.velocity = new Vector2(rigid.velocity.x * 1.2f, jumpPower * 1.5f); // ������ ���⸦ �ٸ��� ���ְ�
                anim.SetBool("isJumping", true); // �����ϴ� �ִϸ��̼��� ������
            }

            else
            {
                rigid.velocity = new Vector2(rigid.velocity.x * 1.5f, jumpPower);       // ������ ���⸦ �ٸ��� ���ְ�
                anim.SetBool("isJumping", true);                                 // �����ϴ� �ִϸ��̼��� ������
            }
        }
        chargingTime += Time.deltaTime;

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // ���� ��ȯ
        if (Input.GetButton("Horizontal"))
            spriteRender.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3) // ĳ������ �̵� �ӵ��� ������� �ִϸ��̼��� ����ϴ� �κ�
            anim.SetBool("isWalking", false);   // ĳ������ �̵��ӵ��� 0.3���� ������ �ȴ� ��� �ִϸ��̼��� ���մϴ�.
        else
            anim.SetBool("isWalking", true);    // ĳ������ �̵��ӵ��� 0.3���� ũ�� �ȴ� ��� �ִϸ��̼��� ����մϴ�.
    }

    void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if (rigid.velocity.x > maxSpeed)    // ������
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))    // ����
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        // Landing platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                // ����
                OnAttack(collision.transform);
            }
            else // damaged
                OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isGold)
                gameManager.stagePoint += 300;
            Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            gameManager.NextStage();

        }
    }


    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 100;
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {

        // hp down
        gameManager.HealthDown();

        // ���� ����
        gameObject.layer = 11;
        spriteRender.color = new Color(1, 1, 1, 0.4f);
            
        // �з����� ȿ��
        int direction = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(direction, 1) * 15, ForceMode2D.Impulse);

        // �ڷ�ƾ���� ���� ���� ����
        StartCoroutine(OffDamaged());

        // �ִϸ��̼�
        anim.SetTrigger("doDamaged");
    }

    IEnumerator OffDamaged()
    {
        // ���� ���� ����
        yield return new WaitForSeconds(3);
        gameObject.layer = 10;
        spriteRender.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        spriteRender.color = new Color(1, 1, 1, 0.3f);
        spriteRender.flipY = true;
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
