using Unity.Entities;
using Unity.Transforms;

namespace PropHunt.Utils
{
    public class ModifyEntityUtilities
    {
        /// <summary>
        /// Will set the data for an entity and any children of that entity
        /// </summary>
        /// <param name="sortKey">Sorting key value for writing to parallel buffer.</param>
        /// <param name="entity">Entity to add data to.</param>
        /// <param name="componentGetter">Readonly component getter to check if data is already added to the entity</param>
        /// <param name="childBufferData">Readonly buffer getter to check if the component has a child and modify the children entities</param>
        /// <param name="newData">New data to set in the entity</param>
        /// <typeparam name="T">Component data type being modified</typeparam>
        /// <returns>True if the component was set (already exists). False if the component was added (did not exist).</returns>
        public static void AddOrSetDataRecursive<T>(int sortKey, Entity entity, T newData,
            ComponentDataFromEntity<T> componentGetter, BufferFromEntity<Child> childBufferData, EntityCommandBuffer.ParallelWriter writer)
            where T : struct, IComponentData
        {
            AddOrSetData<T>(sortKey, entity, newData, componentGetter, writer);
            if (childBufferData.HasComponent(entity))
            {
                foreach (Child child in childBufferData[entity])
                {
                    AddOrSetDataRecursive<T>(sortKey, child.Value, newData, componentGetter, childBufferData, writer);
                }
            }
        }

        /// <summary>
        /// Set the data of an entity if the data is not already set.
        /// </summary>
        /// <param name="sortKey">Sorting key value for writing to parallel buffer.</param>
        /// <param name="entity">Entity to add data to.</param>
        /// <param name="componentGetter">Readonly component getter to check if data is already added to the entity</param>
        /// <param name="newData">New data to set in the entity</param>
        /// <typeparam name="T">Component data type being modified</typeparam>
        /// <returns>True if the component was set (already exists). False if the component was added (did not exist).</returns>
        public static bool AddOrSetData<T>(int sortKey, Entity entity, T newData,
            ComponentDataFromEntity<T> componentGetter, EntityCommandBuffer.ParallelWriter writer)
            where T : struct, IComponentData
        {
            if (componentGetter.HasComponent(entity))
            {
                writer.SetComponent<T>(sortKey, entity, newData);
                return true;
            }
            else
            {
                writer.AddComponent<T>(sortKey, entity, newData);
                return false;
            }
        }
    }
}