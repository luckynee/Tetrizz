using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.EventBus;
using UnityEngine;

namespace Script
{
    public class GameGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int height = 20;
        [SerializeField] private int width = 10;
        [SerializeField] private float cellSize = 1f; // Size of each cell

        [Header("References")]
        [SerializeField] private Transform ghostBlock;
        [SerializeField] private Transform currentBlock;

        [Header("Grid Visuals")]
        [SerializeField] private GameObject gridPrefabs;

        private Transform[,] _filledGrid;

        private EventBindings<OnBlockReachBottomEvent> _onBlockReachBottomEvent;
        
        public static GameGrid Instance;
        
        public int Width => width;
        public int Height => height;

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
            GenerateGridVisuals();
            Bus<OnBlockReachBottomEvent>.Register(_onBlockReachBottomEvent);
        }
        
        private void OnDisable()
        {
            Bus<OnBlockReachBottomEvent>.Unregister(_onBlockReachBottomEvent);
        }

        #region Grid Method
        
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
                
                if (_filledGrid[roundX, roundY] && _filledGrid[roundX, roundY].parent != ghostBlock && _filledGrid[roundX, roundY].parent != currentBlock)
                {
                    return false;
                }
            }
            return true;
        }
        
        public bool IsPositionFilled(Vector3 spawnPosition)
        {
            var roundX = Mathf.FloorToInt(spawnPosition.x);
            var roundY = Mathf.FloorToInt(spawnPosition.y);

            if (roundX < 0 || roundX >= width || roundY < 0 || roundY >= height)
            {
                return true; // Out of bounds = considered filled
            }

            return _filledGrid[roundX, roundY]; // Returns true if occupied
        }

        public int GetCurrentBlockYPosition()
        {
            return (from Transform child in currentBlock.GetChild(0).transform select Mathf.FloorToInt(child.position.y)).Prepend(height).Min();
        }
        
        private void GenerateGridVisuals()
        {
            if (!gridPrefabs) return; 

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var position = transform.position + new Vector3(x * cellSize + 0.5f, y * cellSize + 0.5f, 0);
                    var gridInstance = Instantiate(gridPrefabs, position, Quaternion.identity, transform);
                }
            }
        }

        
        #endregion


        #region Row Checking
        
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
            yield return new WaitForSeconds(0.3f);
            
            for (var x = 0; x < width; x++)
            {
                if (!_filledGrid[x, y]) continue;
                _filledGrid[x, y - shiftAmount] = _filledGrid[x, y]; // Move block down
                _filledGrid[x, y - shiftAmount].position += Vector3.down * shiftAmount; // Update position
                _filledGrid[x, y] = null; // Clear old position
            }
        }
        
        private void ClearAndMoveRows()
        {
            var clearedRows = 0;
            var activeCoroutines = new List<Coroutine>();
            var deletedRows = new List<int>(); // Store deleted row indices

            for (var y = 0; y < height; y++)
            {
                if (IsRowFull(y))
                {
                    ClearRow(y);
                    deletedRows.Add(y); // Store deleted row index
                    clearedRows++;
                }
                else if (clearedRows > 0)
                {
                    var moveCoroutine = StartCoroutine(MoveRowDown(y, clearedRows));
                    activeCoroutines.Add(moveCoroutine);
                }
            }

            if (activeCoroutines.Count > 0)
            {
                Bus<OnDestroyRow>.Raise(new OnDestroyRow(deletedRows));
                StartCoroutine(WaitForRowMovement(activeCoroutines));
            }
            else
            {
                Bus<OnDoneCheckingRow>.Raise(new OnDoneCheckingRow());
            }
        }

        private IEnumerator WaitForRowMovement(List<Coroutine> activeCoroutines)
        {
            foreach (var coroutine in activeCoroutines)
            {
                yield return coroutine; // Wait for each coroutine to finish
            }

            yield return new WaitForSeconds(0.1f);
            Bus<OnDoneCheckingRow>.Raise(new OnDoneCheckingRow());
        }
        #endregion
        
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