using System;
using TMC._Project.Gameplay.Common.Scripts.Enums;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    [Serializable]
    public struct LevelItemPlacementData
    {
        public string ItemId;
        public Vector2 Position;
        public IsometricFaceDirection FaceDirection;
    }
}