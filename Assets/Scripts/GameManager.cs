using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;  // 누적 포인트
    public int stagePoint;  // 현재 스테이지에서 얻은 포인트
    public int stageIndex;  // 현재 스테이지 인덱스
    public int health;      // 플레이어의 남은 체력
    public PlayerMove player;   // 플레이어 객체
    public GameObject[] Stages; // 전체 스테이지

    public Image[] UIhealth;   // 체력 UI 이미지 객체 배열
    public TextMeshProUGUI UIPoint; // 누적 포인트 UI 객체
    public TextMeshProUGUI UIStage; // 스테이지 번호 표시 UI 객체
    public GameObject UIRestartBtn; // 게임 오버 시 활성화되는 리스타트 버튼

    // 프레임당 실행되는 함수. 누적 포인트를 업데이트한다.
    private void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    // 스테이지 클리어 후 불려지는 함수
    public void NextStage()
    {
        // 스테이지 변경
        if (stageIndex < Stages.Length - 1)
        {

            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            // 스테이지 UI 업데이트
            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        // 게임클리어
        else
        {
            Time.timeScale = 0;
            Debug.Log("게임 클리어!");
        }

        // Calc Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    // 체력 감소 함수
    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        { // 플레이어 사망 시
            player.OnDie();
            Debug.Log("죽었습니다 ㅋ");

            // 리스타트 버튼 활성화
            UIRestartBtn.SetActive(true);

            // 리스타트 버튼의 텍스트 수정
            TextMeshProUGUI btnText = UIRestartBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Restart";
        }
    }

    // 플레이어와 바닥충돌 시 불려지는 함수
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1) // 체력이 남아있을 때는 플레이어를 리스폰시킴
                PlayerReposition();
        }

        HealthDown(); // 충돌 시 체력 감소
    }

    // 바닥에 플레이어가 떨어졌을때 리스폰 위치 설정 함수
    void PlayerReposition()
    {
        player.transform.position = new Vector3(-8, -8, -1);    //Vector3(-8,-8,-1) 이부분을 수정하면 리스폰위치가 바뀐다
        player.VelocityZero();
    }

    // 리스타트 버튼 클릭 시 호출되는 함수
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
