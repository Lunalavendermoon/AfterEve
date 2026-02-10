using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

    [field: SerializeField] public EventReference playerTakeDamage { get; private set; }

    [field: Header("Gun SFX")]
    [field: SerializeField] public EventReference gunshot { get; private set; }

    [field: SerializeField] public EventReference reload { get; private set; }

    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference blobAttack { get; private set; }
    [field: SerializeField] public EventReference enemyTakeDamage { get; private set; }


    [field: Header("Music")]
    [field: SerializeField] public EventReference combat { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
       if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in one scene");
        }
        instance = this;
    }
}
