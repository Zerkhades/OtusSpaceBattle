using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using Xunit;

namespace OtusSpaceBattle.Tests
{
    public interface ITestInterface
    {
        int GetInt();
        void SetInt(int value);
        Vector2 GetVector();
        void SetVector(Vector2 v);
        void DoSomething();
    }

    public class AdapterGeneratorTests
    {
        [Fact]
        public void GenerateAdapterCode_ShouldContainCorrectMethodSignatures_AndIoCKeys()
        {
            // Act
            var code = AdapterGenerator.GenerateAdapterCode(typeof(ITestInterface));

            // Assert
            Assert.Contains("public int GetInt()", code);
            Assert.Contains("public void SetInt(int value)", code);
            Assert.Contains("public Vector2 GetVector()", code);
            Assert.Contains("public void SetVector(Vector2 v)", code);
            Assert.Contains("public void DoSomething()", code);

            // Проверка IoC ключей с полным именем интерфейса
            Assert.Contains("OtusSpaceBattle.Tests.ITestInterface:int.get", code);
            Assert.Contains("OtusSpaceBattle.Tests.ITestInterface:int.set", code);
            Assert.Contains("OtusSpaceBattle.Tests.ITestInterface:vector.get", code);
            Assert.Contains("OtusSpaceBattle.Tests.ITestInterface:vector.set", code);
            Assert.Contains("OtusSpaceBattle.Tests.ITestInterface:dosomething", code);
        }

        [Fact]
        public void GenerateAndSaveAllAdapters_ShouldCreateFiles()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            // Act
            AdapterGenerator.GenerateAndSaveAllAdapters(tempDir, Assembly.GetExecutingAssembly());

            // Assert
            var filePath = Path.Combine(tempDir, "TestInterfaceAdapter.cs");
            Assert.True(File.Exists(filePath), $"File {filePath} should exist");
            var fileContent = File.ReadAllText(filePath);
            Assert.Contains("public class TestInterfaceAdapter", fileContent);
            Assert.Contains("public int GetInt()", fileContent);

            // Clean up
            Directory.Delete(tempDir, true);
        }
    }
}
