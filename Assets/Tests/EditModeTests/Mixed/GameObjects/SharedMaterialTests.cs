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
            SharedMaterialLibrary matLibrary = ScriptableObject.CreateInstance<SharedMaterialLibrary>();
            matLibrary.SharedMaterials = mats;

            sharedMaterialComponent.MaterialLibrary = matLibrary;

            // Initialize shared material componenet
            SharedMaterials.Instance = null;
            sharedMaterialComponent.Awake();
            sharedMaterialComponent.InitializeDictionary();

            // Get the material from a key
            Assert.IsTrue(sharedMaterialComponent.GetMaterialById(mat1.MaterialId).color == mat1.Material.color);
            Assert.IsTrue(sharedMaterialComponent.GetMaterialById(mat2.MaterialId).color == mat2.Material.color);
            Assert.IsTrue(sharedMaterialComponent.GetMaterialById(mat3.MaterialId).color == mat3.Material.color);
            Assert.Throws(typeof(System.Exception), () => sharedMaterialComponent.GetMaterialById(0));

            // Get the key from a material
            Assert.IsTrue(sharedMaterialComponent.GetIdForMaterial(mat1.Material) == mat1.MaterialId);
            Assert.IsTrue(sharedMaterialComponent.GetIdForMaterial(mat2.Material) == mat2.MaterialId);
            Assert.IsTrue(sharedMaterialComponent.GetIdForMaterial(mat3.Material) == mat3.MaterialId);
            Assert.Throws(typeof(System.Exception), () => sharedMaterialComponent.GetIdForMaterial(new Material(Shader.Find("Shader Graphs/LitDOTS"))));

            // Initialize with a non null instance
            sharedMaterialComponent.Awake();
        }
    }
}