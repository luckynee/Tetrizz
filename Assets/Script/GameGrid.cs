using System;
using UnityEngine;

namespace Script
{
    public class GameGrid : MonoBehaviour
    {
        [SerializeField] private int height = 20;
        [SerializeField] private int width = 10;
        [SerializeField] private float cellSize = 1f; // Size of each cell

        private Transform[,] _filledGrid;

        public static GameGrid Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            _filledGrid = new Transform[width, height];
        }

        public Transform[,] GetFilledGrid()
        {
            return _filledGrid;
        }
        
        public void AddToGrid(Transform pieceTransform)
        {
            foreach (Transform child in pieceTransform)
            {
                var newPos = child.position;
                var roundX = Mathf.FloorToInt(newPos.x);
                var roundY = Mathf.FloorToInt(newPos.y);

                _filledGrid[roundX, roundY] = child;
            }
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

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
        }
    }
}