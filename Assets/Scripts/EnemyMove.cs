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
        // �ʿ��� ������Ʈ ��������
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();

        // 5�� �ڿ� Think �Լ� ����
        Invoke("Think", 5);
    }


    void FixedUpdate()
    {
        // �� �̵�
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // �ٴڿ� ���� ���� ��� ���� ��ȯ
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));   // Scene ȭ�鿡�� Ȯ���ϱ� ���� ����� �ڵ�
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    // ����Լ� : ������ �̵� ���� ����
    void Think()
    {
        // -1, 0, 1 �� �����ϰ� ���� ����
        nextMove = Random.Range(-1, 2);

        // �ִϸ��̼� ����
        anim.SetInteger("walkSpeed", nextMove);

        // ĳ���� ���� ����
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        // ���� ������ �̵������� �ð� ����
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); // ����Լ��� ������ ���� ����ϸ� �����ϹǷ� �ݵ�� �����̸� �־� �� 
    }

    // �� �̵� ���� ��ȯ �Լ�
    void Turn()
    {
        // �̵� ���� ����
        nextMove *= -1;

        // ĳ���� ���� ����
        spriteRenderer.flipX = nextMove == 1;

        // Think �Լ� ���
        CancelInvoke();

        // ��� �Լ� Think ����
        Invoke("Think", 2);
    }

    public void OnDamaged()    // ���� ���ظ� ���� �� ȣ��� �Լ�
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);  // �ǰ� �� ���� ���� �ǰ� ȿ���� ��
        spriteRenderer.flipY = true;    // �ǰ� �� �Ʒ������� ���������ٰ� ������� ���ƿ�
        collider.enabled = false;         // �ǰ� �� �浹 ó�� ����
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);    // �ǰ� �� �������� Ƣ�����
        Invoke("DeActive", 5);   // 5�� �ڿ� DeActive �Լ� ����
    }

    void DeActive() // ������Ʈ ��Ȱ��ȭ �Լ�
    {
        gameObject.SetActive(false);
    }
}
