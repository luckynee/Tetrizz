using System.Collections;
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
        
        [SerializeField] private InputReader inputReader;
        
        public PoolHandler poolHandler;
        
        private const int Speed = 1;
        private const float RotationAngle = 90f;

        private float _previousFallTime;
        private float _lockTime;
        private Coroutine _lockCoroutine;
        
        private bool _hasDropped = false;

        private void OnEnable()
        {
            _lockTime = fallTime;
            _hasDropped = false;
            
            inputReader.HardDrop += HardDrop;
            inputReader.LeftPressed += MoveLeft;
            inputReader.RightPressed += MoveRight;
            inputReader.DownPressed += MoveBlockDown;

        }

        private void Update()
        {
            //TODO -> Use new Input System ( CUSTOM INPUT )
            
            // if (Input.GetKeyDown(KeyCode.LeftArrow))
            // {
            //     TryMove(Vector3.left);
            // }
            // else if (Input.GetKeyDown(KeyCode.RightArrow))
            // {
            //     TryMove(Vector3.right);
            // }
            // else if(Input.GetKey(KeyCode.DownArrow))
            // {
            //     MoveBlockDown(true);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     TryRotate();
            // }
            //
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     HardDrop();
            // }
            //
            // if(Input.GetKeyDown(KeyCode.C))
            // {
            //     StorageManager.Instance.StoreBlock(poolHandler);
            // }

            MoveBlockDown();
        }
        
        private void OnDisable()
        {
            inputReader.HardDrop -= HardDrop;
            inputReader.LeftPressed -= MoveLeft;
            inputReader.RightPressed -= MoveRight;
            inputReader.DownPressed -= MoveBlockDown;
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

            foreach (var shift in shiftOffsets)
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
        
        private void MoveLeft()
        {
            TryMove(Vector3.left);
        }
        
        private void MoveRight()
        {
            TryMove(Vector3.right);
        }
        
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
                _previousFallTime = Time.time; 

                if (_lockCoroutine == null) return;
                StopCoroutine(_lockCoroutine);
                _lockCoroutine = null;
            }
            else
            {
                if (_hasDropped) return; // Prevent double triggering

                // Wait for certain times before lock block in place
                _lockCoroutine ??= StartCoroutine(LockBlockAfterDelay());
            }
        }
        
        private IEnumerator LockBlockAfterDelay()
        {
            yield return new WaitForSeconds(_lockTime); // Delay before locking

            Bus<OnBlockReachBottomEvent>.Raise(new OnBlockReachBottomEvent(transform));
            enabled = false; // Disable script
        }

        private void HardDrop()
        {
            if (_hasDropped) return;  // Prevent multiple executions
            _hasDropped = true;

            // Cancel any ongoing locking coroutine (prevent double event trigger)
            if (_lockCoroutine != null)
            {
                StopCoroutine(_lockCoroutine);
                _lockCoroutine = null;
            }

            while (GameGrid.Instance.IsInsideGrid(transform, Vector3.down))
            {
                transform.position += Vector3.down * Speed;
            }

            Bus<OnBlockReachBottomEvent>.Raise(new OnBlockReachBottomEvent(transform));
            enabled = false; // Disable movement script
        }
        #endregion

        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.TransformPoint(rotationPoint), 0.1f);
        }

    }
}