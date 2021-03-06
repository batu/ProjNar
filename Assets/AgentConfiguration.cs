using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public class AgentConfiguration : MonoBehaviour
{
    public GameObject PlayerAgent; 
    private EnvironmentParameters _envParameters;
    List<float> _obsList = new List<float>();

    private NavigationAgent _navigationAgent;
    public void Configure()
    {
        _envParameters = Academy.Instance.EnvironmentParameters;
        _navigationAgent = PlayerAgent.GetComponent<NavigationAgent>();
        
        UpdateActionRandomization();
        UpdateRewards();
        UpdateVectorObs();
        UpdateWhiskerObs();
        UpdateDepthMaskObs();
        UpdateOccupancyGrid();
        UpdateDecisionFreq();
        UpdateBehaviorParam();
    }

    private void UpdateRewards()
    {
        _navigationAgent.useDense = 1 == (int) _envParameters.GetWithDefault("useDense", 1);
        _navigationAgent.usePBRS  = 1 == (int) _envParameters.GetWithDefault("usePBRS", 1);

    }

    private void UpdateActionRandomization()
    {
        _navigationAgent.randomizationPercentage = _envParameters.GetWithDefault("randomization_percentage", 0);
    }

    private void UpdateBehaviorParam()
    {
        BehaviorParameters behaviorParameters = PlayerAgent.GetComponent<BehaviorParameters>();
        int observationSize = _obsList.Count;
        behaviorParameters.BrainParameters.VectorObservationSize = observationSize;
    }

  
    private void UpdateDecisionFreq()
    {
        DecisionRequester decisionRequester = PlayerAgent.GetComponent<DecisionRequester>();
        decisionRequester.DecisionPeriod = (int) _envParameters.GetWithDefault("decision_frequency", 10);
    }

    private void UpdateOccupancyGrid()
    {
        OccupancyGridObservation occupancyGridObservation = PlayerAgent.GetComponent<OccupancyGridObservation>();

        _navigationAgent.useOccupancyGrid = 1 == (int) _envParameters.GetWithDefault("use_occupancy", 1);
        occupancyGridObservation.OccupancyObsCountXZ = (int) _envParameters.GetWithDefault("occupancy_xz_len", 4);
        occupancyGridObservation.OccupancyObsCountY = (int) _envParameters.GetWithDefault("occupancy_y_len", 2);
        
        if (_navigationAgent.useOccupancyGrid)
            _obsList.Add(occupancyGridObservation.GetObservation());
    }

    private void UpdateDepthMaskObs()
    {
        DepthMaskObservation depthMaskObservation = PlayerAgent.GetComponent<DepthMaskObservation>();

        _navigationAgent.useDepthMask = 1 == (int) _envParameters.GetWithDefault("use_depthmap", 1);
        
        depthMaskObservation.maxLen = _envParameters.GetWithDefault("depthmap_raylen", 50);
        depthMaskObservation.rayCount = (int) _envParameters.GetWithDefault("depthmap_raycount", 3);
        depthMaskObservation.rayDistance = _envParameters.GetWithDefault("depthmap_rayseperation", 2);
        
        if (_navigationAgent.useDepthMask)
            _obsList.Add(depthMaskObservation.GetObservation());
    }

    private void UpdateWhiskerObs()
    {
        WhiskerObservation whiskerObservation = PlayerAgent.GetComponent<WhiskerObservation>();
        
        _navigationAgent.useLocalRaycasts = 1 == (int) _envParameters.GetWithDefault("use_whiskers", 1);
        whiskerObservation.numSurroundingRaycasts = (int) _envParameters.GetWithDefault("whisker_raycount", 8);
        whiskerObservation.numVerticalRaycasts = (int) _envParameters.GetWithDefault("whisker_verticalraycount", 2);
        whiskerObservation.surroundingRaycastDistance = _envParameters.GetWithDefault("whisker_raylen", 2);

        whiskerObservation.groundMeshDistance = _envParameters.GetWithDefault("whisker_groundraylen", 0.5f);
        whiskerObservation.groundMeshSeperation = _envParameters.GetWithDefault("whisker_groundrayseperation", 1);
    
        if (_navigationAgent.useLocalRaycasts)
            _obsList.Add(whiskerObservation.GetObservation());
    }

    private void UpdateVectorObs()
    {
        VectorObservation vectorObservation = PlayerAgent.GetComponent<VectorObservation>();

        _navigationAgent.useRotation = 1 == (int) _envParameters.GetWithDefault("use_rotations", 0);
        _navigationAgent.useAbsolutePositions = 1 == (int) _envParameters.GetWithDefault("use_absolute_positions", 1);
        _obsList.Add(vectorObservation.GetObservation());
        return;
    }
}
