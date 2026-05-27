using UnityEngine;

namespace GhostSignal.UI
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "GhostSignal/UI/Level Data")]
    public class LevelData : ScriptableObject
    {
        public enum LevelStatus { Locked, Available, Current, Completed }

        [Header("Configuración de Identificación")]
        public int id;
        public string code = "01_AMBER";
        public string displayName = "AMBER";
        [TextArea(2, 4)]
        public string description = "Subred industrial · señales térmicas";
        
        [Header("Estética y Escena")]
        public Color accentColor = new Color(0.96f, 0.65f, 0.14f); // #F5A623
        public Sprite thumbnail;
        public string sceneName;

        [Header("Métricas de Progreso")]
        public int fragmentsTotal = 6;
    }
}