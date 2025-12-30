using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HostManager.Models;

namespace HostManager.Services
{
    public class EnvService
    {
        private readonly string _filePath;
        private readonly List<string> _defaultEnvs = new() { "local", "qa", "stage", "prod" };

        public EnvService()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(appDir, "envs.json");
        }

        public EnvService(string filePath)
        {
            _filePath = filePath;
        }

        public List<HostEnv> LoadEnvs()
        {
            List<string> names;

            if (!File.Exists(_filePath))
            {
                names = new List<string>(_defaultEnvs);
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    names = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>(_defaultEnvs);
                }
                catch
                {
                    names = new List<string>(_defaultEnvs);
                }
            }

            var envs = new List<HostEnv>();
            foreach (var name in names)
            {
                envs.Add(new HostEnv 
                { 
                    Name = name,
                    IsDefault = _defaultEnvs.Contains(name)
                });
            }
            return envs;
        }

        public void SaveEnvs(List<HostEnv> envs)
        {
            var names = new List<string>();
            foreach (var env in envs)
            {
                if (!string.IsNullOrWhiteSpace(env.Name))
                    names.Add(env.Name);
            }

            var json = JsonSerializer.Serialize(names, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_filePath, json);
        }

        public List<string> GetDefaultEnvs() => _defaultEnvs;
    }
}
