﻿using System.Collections;
using BattleCity.Common;
using BattleCity.Tanks;
using UnityEngine;

namespace BattleCity.AI
{
    public class FieldContainer
    {
        private readonly int _rows;
        private readonly int _columns;
        private readonly GameObject[] _walls;
        public readonly Vector3 TopLeftPointPosition;
        public readonly float DistanceBetweenPoints;
        public readonly SceneFieldCoordinatesConverter Converter;
        
        private Vector2Int[] _wallCoordinates;

        public bool[,] Field { get; private set; }
        
        public FieldContainer(int rows, int columns, GameObject[] walls, Vector3 topLeftPointPosition, float distanceBetweenPoints)
        {
            _rows = rows;
            _columns = columns;
            _walls = walls;
            TopLeftPointPosition = topLeftPointPosition;
            DistanceBetweenPoints = distanceBetweenPoints;

            Converter = new SceneFieldCoordinatesConverter(TopLeftPointPosition, DistanceBetweenPoints);

            HandleWalls();
            InitializeField();
        }

        private void HandleWalls()
        {
            _wallCoordinates = new Vector2Int[_walls.Length];
            for (int i = 0; i < _walls.Length; i++)
            {
                GameObject wall = _walls[i];
                Vector3 scenePosition = wall.transform.position;
                Vector2Int fieldCoordinates = Converter.Convert(scenePosition);
                _wallCoordinates[i] = fieldCoordinates;

                if (wall.TryGetComponent(out DamageableComponent damageableComponent))
                {
                    Damageable damageable = damageableComponent.Damageable;
                    damageable.OnDestroy += () => DestroyWall(fieldCoordinates);
                }
            }
        }
        private void InitializeField()
        {
            Field = new bool[_rows, _columns];
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    Field[i, j] = !PointIsAWall(new Vector2Int(i, j));
                }
            }
        }

        private bool PointIsAWall(in Vector2Int pointPosition)
        {
            return ((IList) _wallCoordinates).Contains(pointPosition);
        }
        private void DestroyWall(in Vector2Int wallPosition)
        {
            Field[wallPosition.x, wallPosition.y] = true;
        }

        /*public Vector2Int SceneToFieldCoordinates(in Vector3 scenePosition)
        {
            int row = Mathf.RoundToInt((TopLeftPointPosition.z - scenePosition.z) / DistanceBetweenPoints);
            int column = Mathf.RoundToInt((scenePosition.x - TopLeftPointPosition.x) / DistanceBetweenPoints);

            return new Vector2Int(row, column);
        }
        public Vector3 FieldToSceneCoordinates(in Vector2Int fieldPosition)
        {
            float x = TopLeftPointPosition.x + fieldPosition.y * DistanceBetweenPoints;
            float z = TopLeftPointPosition.z - fieldPosition.x * DistanceBetweenPoints;
            
            return new Vector3(x, TopLeftPointPosition.y, z);
        }*/
    }

    public class SceneFieldCoordinatesConverter : IConverter<Vector3, Vector2Int>, IConverter<Vector2Int, Vector3>
    {
        private readonly Vector3 _topLeftFieldPointPosition;
        private readonly float _distanceBetweenPoints;

        public SceneFieldCoordinatesConverter(Vector3 topLeftFieldPointPosition, float distanceBetweenPoints)
        {
            _topLeftFieldPointPosition = topLeftFieldPointPosition;
            _distanceBetweenPoints = distanceBetweenPoints;
        }

        public Vector3 Convert(Vector2Int fieldPosition)
        {
            float x = _topLeftFieldPointPosition.x + fieldPosition.y * _distanceBetweenPoints;
            float z = _topLeftFieldPointPosition.z - fieldPosition.x * _distanceBetweenPoints;
            
            return new Vector3(x, _topLeftFieldPointPosition.y, z);
        }

        public Vector2Int Convert(Vector3 scenePosition)
        {
            int row = Mathf.RoundToInt((_topLeftFieldPointPosition.z - scenePosition.z) / _distanceBetweenPoints);
            int column = Mathf.RoundToInt((scenePosition.x - _topLeftFieldPointPosition.x) / _distanceBetweenPoints);

            return new Vector2Int(row, column);
        }
    }
}