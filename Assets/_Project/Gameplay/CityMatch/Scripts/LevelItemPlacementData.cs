using System;
using TMC._Project.Gameplay.Common.Scripts.Enums;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    [Serializable]
    public struct LevelItemPlacementData
    {
        public int ItemId;
        public Vector2 Position;
        public IsometricFaceDirection FaceDirection;
    }
}