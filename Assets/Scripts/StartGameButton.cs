using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    [Header("Scripts to enable on Start")]
    public Airplane airplane;  
    public EnvironmentMover environmentMover;
    public MonoBehaviour laneController;   

    [Header("Roots to reset (recommended)")]
    public Transform worldForwardRoot;    
    public Transform worldLateralRoot;        

    [Header("UI")]
    public GameObject startButton;           

    Vector3 forwardStartPos;
    Vector3 lateralStartLocalPos;

    void Awake()
    {
        if (worldForwardRoot != null) forwardStartPos = worldForwardRoot.position;
        if (worldLateralRoot != null) lateralStartLocalPos = worldLateralRoot.localPosition;
    }

    public void OnStartPressed()
    {
        // reset roots so offsets like X=200 never break lanes
        if (worldForwardRoot != null) worldForwardRoot.position = forwardStartPos;
        if (worldLateralRoot != null) worldLateralRoot.localPosition = lateralStartLocalPos;

        if (airplane != null) airplane.enabled = true;

        if (laneController != null) laneController.enabled = true;

        if (laneController != null) laneController.enabled = true;

        if (laneController is SwipeWorldLaneController swipe) swipe.SetSwipeEnabled(true);   // blocks swipe before Start, allow after Start


        if (environmentMover != null)
        {
            environmentMover.enabled = true;
            environmentMover.StartMoving();   // your project method
        }

        if (startButton != null) startButton.SetActive(false);

        Debug.Log("START pressed: enabled airplane + lane + mover");
    }
}
