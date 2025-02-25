using Script.EventBus;
using UnityEngine;

namespace Script
{
    public class BlockController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float fallTime = 1f;
        
        [Header("Rotation Settings")]
        [SerializeField] private Vector2 rotationPoint;
        
        private const int Speed = 1;
        private const float RotationAngle = 90f;

        private float _previousFallTime;

        private void Update()
        {
            //TODO -> Use new Input System ( CUSTOM INPUT )
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TryMove(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                TryMove(Vector3.right);
            }
            else if(Input.GetKey(KeyCode.DownArrow))
            {
                MoveBlockDown(true);
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                TryRotate();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                HardDrop();
            }

            MoveBlockDown();
        }

        #region Rotation
        private void TryRotate()
        {
            var worldRotationPoint = transform.TransformPoint(rotationPoint);

            // Simulate rotation
            transform.RotateAround(worldRotationPoint, Vector3.back, RotationAngle);

            // Check if still inside grid
            if (!GameGrid.Instance.IsInsideGrid(transform, Vector3.zero))
            {
                // Try adjusting position to fit inside the grid
                if (!TryWallKick())
                {
                    // If no valid position found, undo rotation
                    transform.RotateAround(worldRotationPoint, Vector3.back, -RotationAngle);
                }
            }
        }
        
        private bool TryWallKick()
        {
            int[] shiftOffsets = { 1, -1, 2, -2 }; // Prioritize small shifts first

            foreach (int shift in shiftOffsets)
            {
                transform.position += new Vector3(shift, 0, 0); // Move left or right

                if (GameGrid.Instance.IsInsideGrid(transform, Vector3.zero))
                {
                    return true; // Found a valid position
                }

                // Undo movement if still out of bounds
                transform.position -= new Vector3(shift, 0, 0);
            }

            return false; // No valid position found
        }
        #endregion

        #region Movement
        private void TryMove(Vector3 direction)
        {
            if (GameGrid.Instance.IsInsideGrid(transform, direction))
            {
                transform.position += direction * Speed;
            }
        }
        
        private void MoveBlockDown(bool speedUp = false)
        {
            var fallSpeed = speedUp ? fallTime / 10 : fallTime;
            
            if (Time.time - _previousFallTime < fallSpeed) return; // Ensure enough time has passed

            // Check if moving down is allowed before applying movement
            if (GameGrid.Instance.IsInsideGrid(transform, Vector3.down))
            {
                transform.position += Vector3.down * Speed;
                _previousFallTime = Time.time; // Update fall time **after successful move**
            }
            else
            {
                GameGrid.Instance.AddToGrid(transform);
                enabled = false; // Disable script
                
                Bus<OnBlockReachBottomEvent>.Raise(new OnBlockReachBottomEvent());
            }
        }
        
        private void HardDrop()
        {
            while (GameGrid.Instance.IsInsideGrid(transform, Vector3.down))
            {
                transform.position += Vector3.down * Speed;
            }

            // Once the block can no longer move, lock it in place
            GameGrid.Instance.AddToGrid(transform);
            enabled = false; // Disable movement script

            // Raise event to notify the system that block has reached the bottom
            Bus<OnBlockReachBottomEvent>.Raise(new OnBlockReachBottomEvent());

            Debug.Log("Hard Drop Completed");
        }

        
        #endregion

        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.TransformPoint(rotationPoint), 0.1f);
        }

    }
}