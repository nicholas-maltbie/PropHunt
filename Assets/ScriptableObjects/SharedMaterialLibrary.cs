using UnityEngine;
namespace PropHunt.Scriptable
{
    [CreateAssetMenu(fileName = "SharedMaterialLibrary", menuName = "ScriptableObjects/SharedMaterialLibrary", order = 1)]
    public class SharedMaterialLibrary : ScriptableObject
    {
        [SerializeField]
        public SharedMaterial[] SharedMaterials;
    }

    [System.Serializable]
    public class SharedMaterial
    {
        public Material Material;
    }
}