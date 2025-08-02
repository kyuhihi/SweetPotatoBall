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
            Debug.LogWarning("TacticsManager 인스턴스가 이미 존재합니다. 중복 제거.");
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

    private MultiAimConstraint GetHeadAimConstraint(GameObject player)
    {
        // 플레이어 하위의 모든 MultiAimConstraint 찾기
        MultiAimConstraint[] allConstraints = player.GetComponentsInChildren<MultiAimConstraint>();

        if (allConstraints.Length == 0)
        {
            Debug.LogWarning($"{player.name}에서 MultiAimConstraint를 찾을 수 없습니다.");
            return null;
        }
        // 첫 번째 찾은 것 사용 (또는 특정 조건으로 필터링)
        Debug.Log($"{player.name}에서 MultiAimConstraint 찾음: {allConstraints[0].name}");
        return allConstraints[0];
    }

    private void ExecutePass(GameObject passer, GameObject receiver)
    {
        Debug.Log($"{passer.name}이(가) {receiver.name}에게 패스!");

        MultiAimConstraint headAimConstraintByPasser = GetHeadAimConstraint(passer);
        MultiAimConstraint headAimConstraintByReceiver = GetHeadAimConstraint(receiver);

        // null 체크 개선
        if (headAimConstraintByPasser != null && headAimConstraintByReceiver != null)
        {
            var dataByPasser = headAimConstraintByPasser.data;
            var dataByReceiver = headAimConstraintByReceiver.data;

            // 새 WeightedTransformArray 생성
            WeightedTransformArray newSourcesByPasser = new WeightedTransformArray();
            WeightedTransformArray newSourcesByReceiver = new WeightedTransformArray();

            // 서로 바라보도록 설정
            newSourcesByPasser.Add(new WeightedTransform(receiver.transform, 1f));
            newSourcesByReceiver.Add(new WeightedTransform(passer.transform, 1f));

            dataByPasser.sourceObjects = newSourcesByPasser;
            dataByReceiver.sourceObjects = newSourcesByReceiver;

            // 데이터 재할당
            headAimConstraintByPasser.data = dataByPasser;
            headAimConstraintByReceiver.data = dataByReceiver;

            // Weight도 확인
            headAimConstraintByPasser.weight = 1f;
            headAimConstraintByReceiver.weight = 1f;

            // RigBuilder 업데이트 (중복 제거)
            RigBuilder rigBuilderByPasser = passer.GetComponent<RigBuilder>();
            RigBuilder rigBuilderByReceiver = receiver.GetComponent<RigBuilder>();
            
            if (rigBuilderByPasser != null)
            {
                rigBuilderByPasser.Build();
            }
            if (rigBuilderByReceiver != null)
            {
                rigBuilderByReceiver.Build();
            }

            Debug.Log($"{passer.name}과 {receiver.name}이 서로 바라보도록 설정됨.");
        }
        else
        {
            Debug.LogWarning("HeadAimConstraint를 찾을 수 없습니다.");
        }


        // 여기에 실제 패스 로직 추가
        // - 물리를 어떻게 할것인가
        // - 공이 도착하면 sourceobjects를 초기화
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