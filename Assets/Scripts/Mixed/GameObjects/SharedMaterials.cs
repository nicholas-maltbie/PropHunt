using System.Collections.Generic;
using PropHunt.Scriptable;
using UnityEngine;

namespace PropHunt.Mixed
{
    /// <summary>
    /// Shared Materias: will be an object that will be instantiated with a library of materials, 
    /// from which the user will derive the id.
    ///</summary>
    public class SharedMaterials : MonoBehaviour
    {
        public static SharedMaterials Instance;
        public SharedMaterialLibrary MaterialLibrary;
        private Dictionary<int, Material> materialDictionary;
        public void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Detected more than one Shared Material on scene, this might cause unexpected results.");
                return;
            }

            Instance = this;
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            materialDictionary = new Dictionary<int, Material>();

            // Add materials to a dictionary to make querying faster.
            foreach (var materialLib in MaterialLibrary.SharedMaterials)
            {
                materialDictionary.Add(materialLib.MaterialId, materialLib.Material);
            }
        }

        public virtual Material GetMaterialById(int id)
        {

            if (materialDictionary.TryGetValue(id, out var material))
            {
                return material;
            }
            throw new System.Exception($"Material with id {id} was not found on the library.");
        }

        public int GetIdForMaterial(Material material)
        {
            foreach (var key in materialDictionary.Keys)
            {
                if (materialDictionary[key] == material)
                {
                    return key;
                }
            }
            throw new System.Exception($"Given material was not found on the material library. Make sure to Add the material to the Scriptable Object Material library on scene.");
        }
    }
}