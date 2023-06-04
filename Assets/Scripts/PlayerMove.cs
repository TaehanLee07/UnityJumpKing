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
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")) // 점프 버튼을 입력했을 때 && 점프 애니매이션이 실행되고 있지 않을 때 (현재 점프하고 있지 않을 때)
        {
            // 함수가 실행되었을 때 기적으로 실행할 코드
            chargingTime = 0; // 스페이스 바를 누른 시간 초기화
        }
        if (Input.GetButtonUp("Jump") && !anim.GetBool("isJumping")) //프 버튼을 입력했을 때 && 점프 애니매이션이 실행되고 있지 않을 때 (현재 점프하고 있지 않을 때) && 걷기 애니매이션이 실행되고 있지 않을 때 (현재 걷고 있지 않을 때)
        {
            if (chargingTime >= 1.0f) // 기를 모은 시이 1보다 클 때 (점프 버튼을 누른 시간에 따라서 점프의 세기를르게 하기 위한 조건)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower * 2.0f);        // 점프의 세기를 다르게 해주고
                anim.SetBool("isJumping", true);                                  // 점프하는 애니매이션을 실행함
            }

            else if (chargingTime >= 0.5f)
            {
                rigid.velocity = new Vector2(rigid.velocity.x * 1.2f, jumpPower * 1.5f); // 점프의 세기를 다르게 해주고
                anim.SetBool("isJumping", true); // 점프하는 애니매이션을 실행함
            }

            else
            {
                rigid.velocity = new Vector2(rigid.velocity.x * 1.5f, jumpPower);       // 점프의 세기를 다르게 해주고
                anim.SetBool("isJumping", true);                                 // 점프하는 애니매이션을 실행함
            }
        }
        chargingTime += Time.deltaTime;

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // 방향 전환
        if (Input.GetButton("Horizontal"))
            spriteRender.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3) // 캐릭터의 이동 속도를 기반으로 애니메이션을 재생하는 부분
            anim.SetBool("isWalking", false);   // 캐릭터의 이동속도가 0.3보다 작으면 걷는 모션 애니메이션을 중합니다.
        else
            anim.SetBool("isWalking", true);    // 캐릭터의 이동속도가 0.3보다 크면 걷는 모션 애니메이션을 재생합니다.
    }

    void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if (rigid.velocity.x > maxSpeed)    // 오른쪽
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))    // 왼쪽
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
                // 공격
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

        // 무적 상태
        gameObject.layer = 11;
        spriteRender.color = new Color(1, 1, 1, 0.4f);
            
        // 밀려나는 효과
        int direction = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(direction, 1) * 15, ForceMode2D.Impulse);

        // 코루틴으로 무적 상태 종료
        StartCoroutine(OffDamaged());

        // 애니메이션
        anim.SetTrigger("doDamaged");
    }

    IEnumerator OffDamaged()
    {
        // 무적 상태 종료
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
