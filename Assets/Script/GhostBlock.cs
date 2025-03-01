using System;
using System.Collections.Generic;
using System.Linq;
using Script.EventBus;
using Unity.VisualScripting;
using UnityEngine;

namespace Script
{
    public class GhostBlock : MonoBehaviour
    {
        [SerializeField] private Block[] blocks;

        private EventBindings<OnBlockMoved> _onBlockMovedEvent;
        private EventBindings<OnBlockRotated> _onBlockRotatedEvent;
        private EventBindings<OnBlockEnabled> _onBlockEnabledEvent;
        private EventBindings<OnBlockReachBottomEvent> _onBlockReachBottomEvent;
        private EventBindings<OnBlockStored> _onBlockStoredEvent;

        private readonly Dictionary<BlockType, GameObject> _blockDictionary = new Dictionary<BlockType, GameObject>();

        private readonly Dictionary<BlockType, RotationPointData> _rotationPointDictionary =
            new Dictionary<BlockType, RotationPointData>();

        private Vector2 _rotationPoint;
        private BlockType _currentBlockType;

        private Vector2 _defaultPosition;
        private float _defaultRotation;

        private Transform _farLeftChild;
        private Transform _farRightChild;

        private void Awake()
        {
            _onBlockMovedEvent = new EventBindings<OnBlockMoved>(UpdateGhostBlockPosition);
            _onBlockRotatedEvent = new EventBindings<OnBlockRotated>(UpdateGhostBlockRotation);
            _onBlockEnabledEvent = new EventBindings<OnBlockEnabled>(EnableGhostBlock);
            _onBlockReachBottomEvent = new EventBindings<OnBlockReachBottomEvent>(DisabledGhostBlock);
            _onBlockStoredEvent = new EventBindings<OnBlockStored>(DisabledGhostBlock);

            _defaultPosition = transform.position;
            _defaultRotation = transform.rotation.z;
        }

        private void OnEnable()
        {
            Bus<OnBlockMoved>.Register(_onBlockMovedEvent);
            Bus<OnBlockRotated>.Register(_onBlockRotatedEvent);
            Bus<OnBlockEnabled>.Register(_onBlockEnabledEvent);
            Bus<OnBlockReachBottomEvent>.Register(_onBlockReachBottomEvent);
            Bus<OnBlockStored>.Register(_onBlockStoredEvent);

            foreach (var block in blocks)
            {
                _blockDictionary.Add(block.blockType, block.block);
                _rotationPointDictionary.Add(block.blockType, block.rotationPointData);
            }
        }

        private void OnDisable()
        {
            Bus<OnBlockMoved>.Unregister(_onBlockMovedEvent);
            Bus<OnBlockRotated>.Unregister(_onBlockRotatedEvent);
            Bus<OnBlockEnabled>.Unregister(_onBlockEnabledEvent);
            Bus<OnBlockReachBottomEvent>.Unregister(_onBlockReachBottomEvent);
            Bus<OnBlockStored>.Unregister(_onBlockStoredEvent);
        }
        
        private void Update()
        {
            AdjustGhostPositionIfColliding();
        }

        #region event delegates

        private void UpdateGhostBlockPosition(OnBlockMoved obj)
        {
            MoveGhostXPosition(obj.xPosition);
            MoveGhostYPosition();
        }
        
        private void UpdateGhostBlockRotation(OnBlockRotated obj)
        {
            TryRotate(obj.RotationAngle);
            MoveGhostYPosition();
        }
        
        private void EnableGhostBlock(OnBlockEnabled args)
        {
            _currentBlockType = args.blockType;
            var block = _blockDictionary[_currentBlockType];
            
            GetEdgeChild();
            
            MoveGhostYPosition();
            
            block.SetActive(true);
        }

        private void DisabledGhostBlock()
        {
            var block = _blockDictionary[_currentBlockType];
            block.SetActive(false);
            transform.position = _defaultPosition;
            transform.rotation = Quaternion.Euler(0, 0, _defaultRotation);
            
            _farLeftChild = null;
            _farRightChild = null;
        }
        
        #endregion

        #region Movement & Rotation
        
        private void TryRotate(float rotationAngle)
        {
            var worldRotationPoint = transform.TransformPoint(_rotationPointDictionary[_currentBlockType].rotationPoint);
            
            transform.RotateAround(worldRotationPoint, Vector3.back, rotationAngle);
            GetEdgeChild();
        }

        private void MoveGhostXPosition(float xPosition)
        {
            transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
        }

        private void MoveGhostYPosition()
        {
            while (GameGrid.Instance.IsInsideGrid(_blockDictionary[_currentBlockType].transform, Vector3.down))
            {
                transform.position += Vector3.down;
            }
            
            IsAnyBlockAbove();
            AdjustGhostPositionIfColliding();
        }

        #endregion

        #region Validator

        private void AdjustGhostPositionIfColliding()
        {
            if (!_blockDictionary.TryGetValue(_currentBlockType, out var value)) return;

            // Check if any child of the ghost block is colliding
            var isColliding = value.transform
                .Cast<Transform>()
                .Any(child => GameGrid.Instance.IsPositionFilled(child.position));

            // If colliding, move up once
            if (isColliding)
            {
                transform.position += Vector3.up;
            }

            // Prevent going out of bounds (edge case handling)
            if (transform.position.y >= GameGrid.Instance.Height)
            {
                transform.position = new Vector3(transform.position.x, GameGrid.Instance.Height - 1, transform.position.z);
            }
        }

        private void IsAnyBlockAbove()
        {
            var farLeftX = Mathf.FloorToInt(_farLeftChild.position.x);
            var farRightX = Mathf.FloorToInt(_farRightChild.position.x);
            var currentBlockY = GameGrid.Instance.GetCurrentBlockYPosition();

            var highestFilledY = float.MinValue; // Track the highest occupied block

            for (var x = farLeftX; x <= farRightX; x++) // Check across the entire width
            {
                for (var y = transform.position.y + 0.5f; y <= currentBlockY; y += 1f) // Check upwards
                {
                    if (GameGrid.Instance.IsPositionFilled(new Vector3(x, y, 0)))
                    {
                        highestFilledY = Mathf.Max(highestFilledY, y); // Track the highest filled Y position
                    }
                }
            }

            // Move the ghost block just above the detected highest filled block, considering the 0.5 offset
            if (Mathf.Approximately(highestFilledY, float.MinValue)) return;
            transform.position = new Vector3(transform.position.x, highestFilledY + 1.5f, transform.position.z);
        }


        private void GetEdgeChild()
        {
            _farLeftChild = null;
            _farRightChild = null;

            foreach (Transform child in _blockDictionary[_currentBlockType].transform)
            {
                if (!_farLeftChild || child.position.x < _farLeftChild.position.x)
                {
                    _farLeftChild = child;
                }

                if (!_farRightChild || child.position.x > _farRightChild.position.x)
                {
                    _farRightChild = child;
                }
            }
        }


        #endregion

        private void OnDrawGizmos()
        {
            if(!_farLeftChild || !_farRightChild) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_farLeftChild.transform.position, .5f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_farRightChild.transform.position, .5f);
        }
    }

    [Serializable]
    public class Block
    {
        public BlockType blockType;
        public RotationPointData rotationPointData;
        public GameObject block;
    }
}
