using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System.Numerics;
using System;
using System.IO;
using System.Reflection;

namespace OtusSpaceBattle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hemlo wormld! <3");

            // Генерация и сохранение адаптеров для всех интерфейсов
            var outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedAdapters");
            AdapterGenerator.GenerateAndSaveAllAdapters(outputDir, Assembly.GetExecutingAssembly());
            Console.WriteLine($"Adapters generated and saved to {outputDir}");
        }
    }
}
