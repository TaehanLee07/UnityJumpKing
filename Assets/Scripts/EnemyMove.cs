using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider;
    public int nextMove;

    void Awake()
    {
        // 필요한 컴포넌트 가져오기d
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();

        // 5초 뒤에 Think 함수 실행
        Invoke("Think", 5);
    }


    void FixedUpdate()
    {
        // 적 이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 바닥에 닿지 않을 경우 방향 전환
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));   // Scene 화면에서 확인하기 위한 디버깅 코드
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    // 재귀함수 : 무작위 이동 방향 결정
    void Think()
    {
        // -1, 0, 1 중 랜덤하게 방향 결정
        nextMove = Random.Range(-1, 2);

        // 애니메이션 변경
        anim.SetInteger("walkSpeed", nextMove);

        // 캐릭터 방향 변경
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        // 다음 무작위 이동까지의 시간 설정
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); // 재귀함수를 딜레이 없이 사용하면 위험하므로 반드시 딜레이를 둬야 함 
    }

    // 적 이동 방향 전환 함수
    void Turn()
    {
        // 이동 방향 반전
        nextMove *= -1;

        // 캐릭터 방향 변경
        spriteRenderer.flipX = nextMove == 1;

        // Think 함수 취소
        CancelInvoke();

        // 재귀 함수 Think 실행
        Invoke("Think", 2);
    }

    public void OnDamaged()    // 적이 피해를 입을 때 호출는 함수
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);  // 피격 시 색을 변경 피격 효과를 줌
        spriteRenderer.flipY = true;    // 피격 시 아래쪽으로 뒤집어졌다가 원래대로 돌아옴
        collider.enabled = false;         // 피격 시 충돌 처리 제거
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);    // 피격 시 위쪽으로 튀어오름
        Invoke("DeActive", 5);   // 5초 뒤에 DeActive 함수 실행
    }

    void DeActive() // 오브젝트 비활성화 함수
    {
        gameObject.SetActive(false);
    }
}
