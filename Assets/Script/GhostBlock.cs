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

        private void EnableGhostBlock(OnBlockEnabled args)
        {
            _currentBlockType = args.blockType;
            var block = _blockDictionary[_currentBlockType];

            MoveGhostBlockX(args.xPosition);
            MoveGhostBlockY();
            block.SetActive(true);
        }

        private void DisabledGhostBlock()
        {
            var block = _blockDictionary[_currentBlockType];
            block.SetActive(false);
            transform.position = _defaultPosition;
            transform.rotation = Quaternion.Euler(0, 0, _defaultRotation);
        }

        private void UpdateGhostBlockPosition(OnBlockMoved args)
        {
            MoveGhostBlockX(args.xPosition);
            MoveGhostBlockY();
        }

        private void MoveGhostBlockY()
        {
            // Move ghost block down until it collides or exceeds grid height
            var moveDownAttempts = 0;
            while (GameGrid.Instance.IsInsideGrid(_blockDictionary[_currentBlockType].gameObject.transform,
                       Vector3.down) &&
                   !IsCollidingWithBlock() &&
                   moveDownAttempts < GameGrid.Instance.Height)
            {
                transform.position += Vector3.down;
                moveDownAttempts++;
            }


            // Move up if colliding, with a safety limit
            var moveUpAttempts = 0;
            Debug.Log("Block above is Empty" + BlockAboveIsEmpty());
            while (!BlockAboveIsEmpty() && moveUpAttempts < GameGrid.Instance.Height)
            {
                transform.position += Vector3.up;
                moveUpAttempts++;
            }

            while (IsCollidingWithBlock() && moveUpAttempts < GameGrid.Instance.Height)
            {
                transform.position += Vector3.up;
                moveUpAttempts++;
            }
        }

        private bool IsCollidingWithBlock()
        {
            return (from Transform child in _blockDictionary[_currentBlockType].transform
                    select new Vector2(child.position.x, child.position.y))
                .Any(checkPos => GameGrid.Instance.IsPositionFilled(checkPos));
        }


        private void MoveGhostBlockX(float xPosition)
        {
            var newPos = transform.position;
            newPos.x = xPosition;

            transform.position = newPos;
        }

        private void UpdateGhostBlockRotation(OnBlockRotated args)
        {
            var currentBlockRotationPoint = _rotationPointDictionary[_currentBlockType].rotationPoint;
            var worldRotationPoint = transform.TransformPoint(currentBlockRotationPoint);
            transform.RotateAround(worldRotationPoint, Vector3.back, args.RotationAngle);
            MoveGhostBlockY();
        }

        private bool BlockAboveIsEmpty()
        {
            if (!_blockDictionary.TryGetValue(_currentBlockType, out var value)) return false;

            // Find the farthest left and right blocks
            var farLeft = value.transform.Cast<Transform>()
                .OrderBy(child => child.position.x)
                .FirstOrDefault();

            var farRight = _blockDictionary[_currentBlockType].transform.Cast<Transform>()
                .OrderByDescending(child => child.position.x)
                .FirstOrDefault();

            if (farLeft == null || farRight == null) return false;

            // Convert world position to grid coordinates
            var leftX = Mathf.FloorToInt(farLeft.position.x);
            var rightX = Mathf.FloorToInt(farRight.position.x);
            var startY = Mathf.FloorToInt(farLeft.position.y); // Same y for both blocks

            // 🔹 Check if any block exists above (looping upwards)
            return !Enumerable.Range(startY + 1, GameGrid.Instance.Height - startY - 1)
                .Any(y => GameGrid.Instance.IsPositionFilled(new Vector3(leftX, y, 0)) ||
                          GameGrid.Instance.IsPositionFilled(new Vector3(rightX, y, 0)));
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
