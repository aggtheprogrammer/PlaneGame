using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    [Header("Scripts to enable on Start")]
    public Airplane airplane;              // drag the Airplane GAMEOBJECT here
    public EnvironmentMover environmentMover; // drag the EnvMover GAMEOBJECT here

    [Header("UI")]
    public GameObject startButton; // drag your Button object here (optional)

    public void OnStartPressed()
    {
        if (airplane != null) 
            airplane.enabled = true;

        if (environmentMover != null)  
            environmentMover.enabled = true;

        if (startButton != null) 
            startButton.SetActive(false);

        Debug.Log("START pressed: enabling scripts"); // for testing
        
        if (environmentMover != null)
        {
            environmentMover.enabled = true;   // keep this if you want
            environmentMover.StartMoving();    // IMPORTANT
        }

    }
}
