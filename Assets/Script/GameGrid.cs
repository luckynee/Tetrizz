using System;
using System.Collections;
using Script.EventBus;
using UnityEngine;

namespace Script
{
    public class GameGrid : MonoBehaviour
    {
        [SerializeField] private int height = 20;
        [SerializeField] private int width = 10;
        [SerializeField] private float cellSize = 1f; // Size of each cell

        private Transform[,] _filledGrid;

        private EventBindings<OnBlockReachBottomEvent> _onBlockReachBottomEvent;
        
        public static GameGrid Instance;

        private void Awake()
        {
            if(!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            _filledGrid = new Transform[width, height];
            _onBlockReachBottomEvent = new EventBindings<OnBlockReachBottomEvent>(AddToGrid);
        }

        private void OnEnable()
        {
            Bus<OnBlockReachBottomEvent>.Register(_onBlockReachBottomEvent);
        }
        
        private void OnDisable()
        {
            Bus<OnBlockReachBottomEvent>.Unregister(_onBlockReachBottomEvent);
        }

        private void AddToGrid(OnBlockReachBottomEvent evt)
        {
            var pieceTransform = evt.transform;
            foreach (Transform child in pieceTransform)
            {
                var newPos = child.position;
                var roundX = Mathf.FloorToInt(newPos.x);
                var roundY = Mathf.FloorToInt(newPos.y);

                _filledGrid[roundX, roundY] = child;
            }
            
            ClearAndMoveRows();

            StartCoroutine(WaitAfterClear());
            Bus<OnDoneCheckingRow>.Raise(new OnDoneCheckingRow());
        }

        private IEnumerator WaitAfterClear()
        {
            yield return new WaitForSeconds(0.1f);
        }

        public bool IsInsideGrid(Transform pieceTransform, Vector3 offset)
        {
            foreach (Transform child in pieceTransform)
            {
                var newPos = child.position + offset;
                var roundX = Mathf.FloorToInt(newPos.x);
                var roundY = Mathf.FloorToInt(newPos.y);

                if (roundX < 0 || roundX >= width || roundY < 0 || roundY >= height)
                {
                    return false;
                }
                
                if (_filledGrid[roundX, roundY])
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsRowFull(int y)
        {
            for (var x = 0; x < width; x++)
            {
                if (!_filledGrid[x, y])
                {
                    return false; // Row is not full
                }
            }
            return true; // Row is full
        }
        
        private void ClearRow(int y)
        {
            for (var x = 0; x < width; x++)
            {
                if (_filledGrid[x, y])
                {
                    _filledGrid[x, y].gameObject.SetActive(false);
                    _filledGrid[x, y] = null;
                }
            }
        }

        
        private IEnumerator MoveRowDown(int y, int shiftAmount)
        {
            yield return new WaitForSeconds(0.5f);
            
            for (int x = 0; x < width; x++)
            {
                if (_filledGrid[x, y])
                {
                    _filledGrid[x, y - shiftAmount] = _filledGrid[x, y]; // Move block down
                    _filledGrid[x, y - shiftAmount].position += Vector3.down * shiftAmount; // Update position
                    _filledGrid[x, y] = null; // Clear old position
                }
            }
        }
        
        private void ClearAndMoveRows()
        {
            var clearedRows = 0;

            for (var y = 0; y < height; y++)
            {
                if (IsRowFull(y))
                {
                    ClearRow(y);
                    clearedRows++;
                }
                else if (clearedRows > 0)
                {
                    StartCoroutine(MoveRowDown(y,clearedRows));
                }
            }
        }
        
        public bool IsPositionFilled(Vector3 spawnPosition)
        {
            var roundX = Mathf.FloorToInt(spawnPosition.x);
            var roundY = Mathf.FloorToInt(spawnPosition.y);

            if (roundX < 0 || roundX >= width || roundY < 0 || roundY >= height)
            {
                return true; // Out of bounds = considered filled
            }

            return _filledGrid[roundX, roundY] != null; // Returns true if occupied
        }


        
        private void OnDrawGizmos()
        {
            // Draw Grid Lines
            Gizmos.color = Color.gray;
            for (var x = 0; x <= width; x++)
            {
                var start = transform.position + new Vector3(x * cellSize, 0, 0);
                var end = start + new Vector3(0, height * cellSize, 0);
                Gizmos.DrawLine(start, end);
            }

            for (var y = 0; y <= height; y++)
            {
                var start = transform.position + new Vector3(0, y * cellSize, 0);
                var end = start + new Vector3(width * cellSize, 0, 0);
                Gizmos.DrawLine(start, end);
            }

            // Draw Filled Blocks
            // for (int x = 0; x < width; x++)
            // {
            //     for (int y = 0; y < height; y++)
            //     {
            //         if (_filledGrid[x, y])
            //         {
            //             Gizmos.color = Color.red; // Color for filled blocks
            //             Vector3 cellCenter = transform.position + new Vector3(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f, 0);
            //             Gizmos.DrawCube(cellCenter, Vector3.one * (cellSize * 1f)); // Draw filled cell
            //         }
            //     }
            // }
        }
    }
}