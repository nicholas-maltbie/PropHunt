using System.Collections.Generic;
using System.Linq;
using PropHunt.Scriptable;
using Unity.Entities;
using UnityEngine;

namespace PropHunt.Mixed
{
    /// <summary>
    /// Interface to lookup and read materials by ID
    /// </summary>
    public interface ISharedMaterialLookup
    {
        /// <summary>
        /// Get a material by it's ID
        /// </summary>
        /// <param name="id">integer id of material</param>
        /// <returns>Material stored under this integer ID</returns>
        Material GetMaterialById(int id);

        /// <summary>
        /// Gets the reference ID of a material
        /// </summary>
        /// <param name="material">Material to lookup</param>
        /// <returns>Id of the material</returns>
        int GetIdForMaterial(Material material);
    }

    /// <summary>
    /// Shared Materias: will be an object that will be instantiated with a library of materials, 
    /// from which the user will derive the id.
    ///</summary>
    public class SharedMaterials : MonoBehaviour, ISharedMaterialLookup, IConvertGameObjectToEntity
    {
        public static ISharedMaterialLookup Instance;
        public SharedMaterialLibrary MaterialLibrary;
        private Dictionary<string, int> materialKeyByNameDictionary;
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

        public void OnDestroy()
        {
            materialDictionary.Clear();
            materialKeyByNameDictionary.Clear();
            Instance = null;
        }

        private void InitializeDictionary()
        {
            materialDictionary = new Dictionary<int, Material>();
            materialKeyByNameDictionary = new Dictionary<string, int>();

            var orderedMaterials = MaterialLibrary.SharedMaterials.OrderBy(m => m.Material.name).ToList();

            // Add materials to a dictionary to make querying faster.
            for(var i = 0; i < orderedMaterials.Count; i++){
                var material = orderedMaterials[i];
                materialKeyByNameDictionary.Add(material.Material.name, i);
                materialDictionary.Add(i, material.Material);
                Debug.Log($"Added material: {material.Material.name} to library.");
            }
        }

        public Material GetMaterialById(int id)
        {
            if (materialDictionary.TryGetValue(id, out var material))
            {
                return material;
            }
            throw new System.Exception($"Material with id {id} was not found on the library.");
        }

        public int GetIdForMaterial(Material material)
        {
            var matName = material.name;
            if(materialKeyByNameDictionary.TryGetValue(matName, out var materialId)){
                return materialId;
            }

            throw new System.Exception($"Given material {material.name} was not found on the material library. Make sure to Add the material to the Scriptable Object Material library on scene.");
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            BlobAssetReference<SharedMaterialBlobAsset> sharedMaterialBlobAssetReference;
            using(var blobBuilder = new BlobBuilder(Unity.Collections.Allocator.Temp)){
                ref var blobAsset = ref blobBuilder.ConstructRoot<SharedMaterialBlobAsset>();
                var sharedMaterialArray = blobBuilder.Allocate(ref blobAsset.Materials, materialDictionary.Count);

                foreach(var dicVal in materialDictionary){
                    sharedMaterialArray[dicVal.Key] = new SharedMaterial{Value = dicVal.Value};
                }

                sharedMaterialBlobAssetReference = blobBuilder.CreateBlobAssetReference<SharedMaterialBlobAsset>(Unity.Collections.Allocator.Persistent);
            }

            dstManager.AddComponentData(entity, new SharedMaterialData{
                sharedMaterialsBlobAssetRef = sharedMaterialBlobAssetReference
            });
        }
    }

    public struct SharedMaterial
    {
        public Material Value;
    }

    public struct SharedMaterialBlobAsset
    {
        public BlobArray<SharedMaterial> Materials;
    }

    public struct SharedMaterialData : IComponentData{
        public BlobAssetReference<SharedMaterialBlobAsset> sharedMaterialsBlobAssetRef;
    }
}