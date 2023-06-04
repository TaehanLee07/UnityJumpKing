using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;  // ���� ����Ʈ
    public int stagePoint;  // ���� ������������ ���� ����Ʈ
    public int stageIndex;  // ���� �������� �ε���
    public int health;      // �÷��̾��� ���� ü��
    public PlayerMove player;   // �÷��̾� ��ü
    public GameObject[] Stages; // ��ü ��������

    public Image[] UIhealth;   // ü�� UI �̹��� ��ü �迭
    public TextMeshProUGUI UIPoint; // ���� ����Ʈ UI ��ü
    public TextMeshProUGUI UIStage; // �������� ��ȣ ǥ�� UI ��ü
    public GameObject UIRestartBtn; // ���� ���� �� Ȱ��ȭ�Ǵ� ����ŸƮ ��ư

    // �����Ӵ� ����Ǵ� �Լ�. ���� ����Ʈ�� ������Ʈ�Ѵ�.
    private void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    // �������� Ŭ���� �� �ҷ����� �Լ�
    public void NextStage()
    {
        // �������� ����
        if (stageIndex < Stages.Length - 1)
        {

            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            // �������� UI ������Ʈ
            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        // ����Ŭ����
        else
        {
            Time.timeScale = 0;
            Debug.Log("���� Ŭ����!");
        }

        // Calc Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    // ü�� ���� �Լ�
    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        { // �÷��̾� ��� ��
            player.OnDie();
            Debug.Log("�׾����ϴ� ��");

            // ����ŸƮ ��ư Ȱ��ȭ
            UIRestartBtn.SetActive(true);

            // ����ŸƮ ��ư�� �ؽ�Ʈ ����
            TextMeshProUGUI btnText = UIRestartBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Restart";
        }
    }

    // �÷��̾�� �ٴ��浹 �� �ҷ����� �Լ�
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1) // ü���� �������� ���� �÷��̾ ��������Ŵ
                PlayerReposition();
        }

        HealthDown(); // �浹 �� ü�� ����
    }

    // �ٴڿ� �÷��̾ ���������� ������ ��ġ ���� �Լ�
    void PlayerReposition()
    {
        player.transform.position = new Vector3(-8, -8, -1);    //Vector3(-8,-8,-1) �̺κ��� �����ϸ� ��������ġ�� �ٲ��
        player.VelocityZero();
    }

    // ����ŸƮ ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
