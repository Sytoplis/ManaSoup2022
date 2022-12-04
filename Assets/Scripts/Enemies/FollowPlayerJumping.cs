using System.Collections;
using System.Collections.Generic;
using ManaSoup2022.Assets.Scripts.Systematic;
using UnityEngine;

public class FollowPlayerJumping : MonoBehaviour
{
    [HideInInspector] private PollingStation pollingStation;
    [HideInInspector] private PlayerController player;
    [SerializeField] private MovementController movementController;
    [SerializeField][Range(0,1)] private float jumpingOffset;

    private void Start()
    {
        if (PollingStation.TryGetPollingStation(out pollingStation, this.gameObject))
        {
            player = pollingStation.playerController;
            if (player == null)
            {
                Debug.LogWarning($"player is null");
            }
        };
        if (movementController == null)
        {
            movementController = GetComponent<MovementController>();
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        var playerDistance = player.transform.position - transform.position;
        var newVerticalSpeed = playerDistance.normalized.y;
        var needsJump = newVerticalSpeed > 0 + jumpingOffset;
        var newHorizontalSpeed = playerDistance.normalized.x;
        AssignValuesToJumpingMovement(needsJump, newHorizontalSpeed);
        
    }

    private void AssignValuesToJumpingMovement(bool needsToJump, float horizontalMovement)
    {
        if (movementController is IJumpingMovement jumpingMovement)
        {
            jumpingMovement.CurrentHorizontalMovement = horizontalMovement;
            jumpingMovement.NeedsJumping = needsToJump;
        }
    }
}
