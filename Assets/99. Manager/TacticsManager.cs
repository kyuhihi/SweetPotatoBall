using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class TacticsManager : MonoBehaviour
{
    public static TacticsManager Instance;
    
    [Header("Pass Settings")]
    public float maxPassDistance = 15f;
    public float minPassDistance = 2f;
    
    // BlueTeam 플레이어 리스트 캐싱
    private List<GameObject> blueTeamPlayers = new List<GameObject>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 시작할 때 BlueTeam 플레이어 리스트 캐싱
        CacheBlueTeamPlayers();
    }

    private void CacheBlueTeamPlayers()
    {
        GameObject[] blueTeam = GameObject.FindGameObjectsWithTag("BlueTeam");
        blueTeamPlayers.AddRange(blueTeam);
        
        Debug.Log($"BlueTeam: {blueTeamPlayers.Count}명 캐싱 완료");
    }

    public void RequestPass(GameObject passer)
    {
        GameObject targetPlayer = FindBestPassTarget(passer);
        
        if (targetPlayer != null)
        {
            ExecutePass(passer, targetPlayer);
        }
        else
        {
            Debug.Log($"{passer.name}: 패스할 대상이 없습니다.");
        }
    }

    private GameObject FindBestPassTarget(GameObject passer)
    {
        GameObject bestTarget = null;
        float bestScore = 0f;

        foreach (GameObject teammate in blueTeamPlayers)
        {
            if (teammate == passer) continue; // 자기 자신 제외

            float distance = Vector3.Distance(passer.transform.position, teammate.transform.position);
            
            // 거리 제한 확인
            if (distance > maxPassDistance || distance < minPassDistance) continue;

            // 패스 점수 계산
            float score = CalculatePassScore(passer, teammate, distance);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = teammate;
            }
        }

        return bestTarget;
    }

    private float CalculatePassScore(GameObject passer, GameObject target, float distance)
    {
        // 거리 점수 (적당한 거리일수록 높음)
        float distanceScore = 1f - (distance / maxPassDistance);
        
        // 방향 점수 (앞쪽 방향일수록 높음)
        Vector3 toTarget = (target.transform.position - passer.transform.position).normalized;
        float directionScore = Vector3.Dot(passer.transform.forward, toTarget);
        directionScore = Mathf.Max(0, directionScore); // 음수 제거
        
        return distanceScore * 0.6f + directionScore * 0.4f;
    }

    private void ExecutePass(GameObject passer, GameObject receiver)
    {
        Debug.Log($"{passer.name}이(가) {receiver.name}에게 패스!");
        //passer.transform.GetChild
        
        // 여기에 실제 패스 로직 추가
        // - 공 이동
        // - 애니메이션 재생
        // - 사운드 재생 등

        // 패스 받은 플레이어로 컨트롤 전환
        SwitchControlToPlayer(receiver);
    }

    private void SwitchControlToPlayer(GameObject targetPlayer)
    {
        GameObject inputManagerGameObject = GameObject.FindWithTag("InputManager");

        InputManager inputManager = inputManagerGameObject.GetComponent<InputManager>();
        if (inputManager != null)
        {
            inputManager.SwitchToPlayer(targetPlayer);            
        }
    }
}