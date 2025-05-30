using UnityEngine;

public class RandomColorGenerator_ColorChangerScript : MonoBehaviour
{

    [SerializeField]
    [Range(.1f, 15f)]
    private float simulationSpeed = 2f;

    private float lastSimulationSpeed;

    private void Start()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // Get all child particle systems and set their start color
        var systems = GetComponentsInChildren<ParticleSystem>();
        foreach (var system in systems)
        {
            ParticleSystem.MainModule main = system.main;
            main.startColor = randomColor;
            main.simulationSpeed = simulationSpeed;
        }
    }

    // private void Update()
    // {
    //     if (Mathf.Abs(simulationSpeed - lastSimulationSpeed) > Mathf.Epsilon)
    //     {
    //         // The value was changed in the Inspector during play mode
    //         var systems = GetComponentsInChildren<ParticleSystem>();
    //         foreach (var system in systems)
    //         {
    //             var main = system.main;
    //             main.simulationSpeed = simulationSpeed;
    //         }
    //         lastSimulationSpeed = simulationSpeed;
    //     }
    // }




}
