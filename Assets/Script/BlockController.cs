using System.Collections;
using Script.EventBus;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class BlockController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float fallTime = 1f;
        
        [Header("Rotation Settings")]
        [SerializeField] private RotationPointData rotationPointData;
        
        [SerializeField] private InputReader inputReader;
        
        public PoolHandler poolHandler;
        
        private const int Speed = 1;
        private const float RotationAngle = 90f;

        private float _previousFallTime;
        private float _lockTime;
        private Coroutine _lockCoroutine;
        
        private bool _hasDropped = false;
        
        private bool _isDownPressed = false;

        private void OnEnable()
        {
            _lockTime = fallTime;
            _hasDropped = false;
            
            inputReader.HardDrop += HardDrop;
            inputReader.LeftPressed += MoveLeft;
            inputReader.RightPressed += MoveRight;
            inputReader.DownPressed += DownPressed;
            inputReader.RotatePressed += TryRotate;
            inputReader.StorePressed += StoreThisBlock;
            
        }

        private void Update()
        {
            MoveBlockDown(_isDownPressed);
        }
        
        private void OnDisable()
        {
            inputReader.HardDrop -= HardDrop;
            inputReader.LeftPressed -= MoveLeft;
            inputReader.RightPressed -= MoveRight;
            inputReader.DownPressed -= MoveBlockDown;
            inputReader.RotatePressed -= TryRotate;
            inputReader.StorePressed -= StoreThisBlock;
        }

        #region Rotation
        private void TryRotate()
        {
            var worldRotationPoint = transform.TransformPoint(rotationPointData.rotationPoint);

            // Simulate rotation
            transform.RotateAround(worldRotationPoint, Vector3.back, RotationAngle);
            Bus<OnBlockRotated>.Raise(new OnBlockRotated(RotationAngle));

            // Check if still inside grid
            if (GameGrid.Instance.IsInsideGrid(transform, Vector3.zero)) return;
            
            // Try adjusting position to fit inside the grid
            if (TryWallKick()) return;
            // If no valid position found, undo rotation
            transform.RotateAround(worldRotationPoint, Vector3.back, -RotationAngle);
            Bus<OnBlockRotated>.Raise(new OnBlockRotated(-RotationAngle)); 

        }
        
        private bool TryWallKick()
        {
            int[] shiftOffsets = { 1, -1, 2, -2 }; // Prioritize small shifts first

            foreach (var shift in shiftOffsets)
            {
                transform.position += new Vector3(shift, 0, 0); // Move left or right
                
                Bus<OnBlockMoved>.Raise(new OnBlockMoved(transform.position.x));
                
                if (GameGrid.Instance.IsInsideGrid(transform, Vector3.zero))
                {
                    return true; // Found a valid position
                }

                // Undo movement if still out of bounds
                transform.position -= new Vector3(shift, 0, 0);
                Bus<OnBlockMoved>.Raise(new OnBlockMoved(transform.position.x));
            }
            return false; // No valid position found
        }
        #endregion

        #region Event
        private void StoreThisBlock()
        {
            StorageManager.Instance.StoreBlock(poolHandler);
        }

        private void DownPressed(bool isPressed)
        {
            _isDownPressed = isPressed;
        }
        
        private void MoveLeft()
        {
            TryMove(Vector3.left);
        }
        
        private void MoveRight()
        {
            TryMove(Vector3.right);
        }
        #endregion

        
        #region Movement
        
        private void TryMove(Vector3 direction)
        {
            if (GameGrid.Instance.IsInsideGrid(transform, direction))
            {
                transform.position += direction * Speed;
                Bus<OnBlockMoved>.Raise(new OnBlockMoved(transform.position.x));
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
            Gizmos.DrawWireSphere(transform.TransformPoint(rotationPointData.rotationPoint), 0.1f);
        }

    }
}