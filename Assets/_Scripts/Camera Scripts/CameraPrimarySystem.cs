using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraPrimarySystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera orbitalCam;
    [SerializeField] private CinemachineVirtualCamera aerialCam;
    [SerializeField] private CinemachineVirtualCamera charCam;
    [SerializeField] private BattleStateMachine stateMachine;
    // Start is called before the first frame update

    private CinemachineVirtualCamera activeCam;
    private Transform oldLookAt;
    
    void Start() {
        orbitalCam.Priority = 5;
        activeCam = orbitalCam;
        oldLookAt = charCam.m_LookAt;
        stateMachine.OnStateTransition += UpdateCamera;
    }

    private void UpdateCamera(BattleStateMachine.BattleState state, BattleStateInput input) {
        if (state is BattleStateMachine.TurnState) { FocusActor(input); }
        else if (state is BattleStateMachine.AnimateState) { ViewAnimate(input); }
    }

    private void FocusActor(BattleStateInput input) {
        charCam.m_LookAt = oldLookAt;
        if (input.ActiveActor() is not CharacterActor) {
            ReturnToBattleView(input);
            return;
        }

        Transform target = input.ActiveActor().transform.GetChild(0);
        charCam.m_Follow = target;
        SetActiveCam(charCam);
    }

    private void ReturnToBattleView(BattleStateInput input) {
        if (input.ActiveActor() is not EnemyActor) return;
        SetActiveCam(orbitalCam);
    }

    private void ViewAnimate(BattleStateInput input) {
        
        Transform target = input.ActiveSkill().Target().transform.GetChild(0);
        Transform user = input.ActiveActor().transform.GetChild(0);

        if (input.ActiveActor() is EnemyActor) {
            charCam.m_LookAt = user;
            charCam.m_Follow = target;
        }
        else {
            charCam.m_LookAt = target;
            charCam.m_Follow = user;
        }

        SetActiveCam(charCam);
    }

    private void SetActiveCam(CinemachineVirtualCamera cam) {
        if (cam == activeCam) return;
        activeCam.m_Priority = 0;
        activeCam = cam;
        activeCam.m_Priority = 5;
    }
}
