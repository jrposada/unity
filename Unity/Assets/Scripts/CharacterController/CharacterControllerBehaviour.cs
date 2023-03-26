using Character;
using Infrastructure;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementState
{
    Wasd
}


[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class CharacterControllerBehaviour : MonoBehaviour
{
    [SerializeField]
    private LayerMask environmentLayer;

    [SerializeField]
    private float ikDistance = default;
    [SerializeField]
    private string leftFootIKWeightParameter = default;
    [SerializeField]
    private string rightFootIKWeightParameter = default;

    private CharacterController characterController;
    private Animator animator;

    private readonly IMovementSystem<MovementState> movementSystem = new MovementSystem<MovementState>();
    private readonly IAnimationSystem<MovementState> animationSystem = new AnimationSystem<MovementState>();

    private MovementData movementInput = new();

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        movementSystem.Initialize(characterController, environmentLayer);
        movementSystem.Add(MovementState.Wasd, new WasdMovment());
        movementSystem.ChangeState(MovementState.Wasd);
        
        animationSystem.Initialize(animator, ikDistance, environmentLayer, Animator.StringToHash(leftFootIKWeightParameter), Animator.StringToHash(rightFootIKWeightParameter));
    }

    private void Update()
    {
        movementInput.Time = Time.deltaTime;
        movementSystem.OnUpdate(movementInput);
    }


    /// <summary>
    /// Input action Move
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 wasd = context.ReadValue<Vector2>();
        movementInput.Horizontal = wasd.x;
        movementInput.Vertical = wasd.y;
    }
}
