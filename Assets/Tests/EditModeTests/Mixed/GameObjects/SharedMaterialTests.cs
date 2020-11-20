using NUnit.Framework;
using PropHunt.Scriptable;
using PropHunt.Mixed;
using Unity.Entities.Tests;
using UnityEngine;

namespace PropHunt.EditMode.Tests.Mixed.GameObjects
{
    [TestFixture]
    public class SharedMaterialTests : ECSTestsFixture
    {
        private GameObject sharedContainer;

        private SharedMaterials sharedMaterialComponent;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.sharedContainer = new GameObject();
            this.sharedMaterialComponent = sharedContainer.AddComponent<SharedMaterials>();
        }

        [TearDown]
        public override void TearDown()
        {
            GameObject.DestroyImmediate(this.sharedContainer);
        }

        [Test]
        public void VerifyProperSetupOfShaderMaterialsFromLibrary()
        {
            SharedMaterial mat1 = new SharedMaterial();
            SharedMaterial mat2 = new SharedMaterial();
            SharedMaterial mat3 = new SharedMaterial();

            mat1.MaterialId = 1234;
            mat2.MaterialId = 42;
            mat3.MaterialId = 10;

            mat1.Material = new Material(Shader.Find("Shader Graphs/LitDOTS"));
            mat2.Material = new Material(Shader.Find("Shader Graphs/LitDOTS"));
            mat3.Material = new Material(Shader.Find("Shader Graphs/LitDOTS"));
            mat1.Material.color = Color.red;
            mat2.Material.color = Color.black;
            mat3.Material.color = Color.blue;

            SharedMaterial[] mats = new SharedMaterial[3] { mat1, mat2, mat3 };
            SharedMaterialLibrary matLibrary = new SharedMaterialLibrary();
            matLibrary.SharedMaterials = mats;

            sharedMaterialComponent.MaterialLibrary = matLibrary;

            // Initialize shared material componenet
            sharedMaterialComponent.Awake();
            // Initialize with a non null instance
            sharedMaterialComponent.Awake();

            // Get the material from a key
            Assert.IsTrue(SharedMaterials.Instance.GetMaterialById(mat1.MaterialId) == mat1.Material);
            Assert.IsTrue(SharedMaterials.Instance.GetMaterialById(mat2.MaterialId) == mat2.Material);
            Assert.IsTrue(SharedMaterials.Instance.GetMaterialById(mat3.MaterialId) == mat3.Material);
            Assert.Throws(typeof(System.Exception), () => SharedMaterials.Instance.GetMaterialById(0));

            // Get the key from a material
            Assert.IsTrue(SharedMaterials.Instance.GetIdForMaterial(mat1.Material) == mat1.MaterialId);
            Assert.IsTrue(SharedMaterials.Instance.GetIdForMaterial(mat2.Material) == mat2.MaterialId);
            Assert.IsTrue(SharedMaterials.Instance.GetIdForMaterial(mat3.Material) == mat3.MaterialId);
            Assert.Throws(typeof(System.Exception), () => SharedMaterials.Instance.GetIdForMaterial(new Material(Shader.Find("Shader Graphs/LitDOTS"))));
        }
    }
}