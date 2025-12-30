using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HostManager.Models;

namespace HostManager.Services
{
    public class GroupService
    {
        private readonly string _filePath;

        public GroupService()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(appDir, "groups.json");
        }

        public GroupService(string filePath)
        {
            _filePath = filePath;
        }

        public List<HostGroup> LoadGroups()
        {
            if (!File.Exists(_filePath))
                return new List<HostGroup>();

            try
            {
                var json = File.ReadAllText(_filePath);
                var names = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                var groups = new List<HostGroup>();
                foreach (var name in names)
                {
                    groups.Add(new HostGroup { Name = name });
                }
                return groups;
            }
            catch
            {
                return new List<HostGroup>();
            }
        }

        public void SaveGroups(List<HostGroup> groups)
        {
            var names = new List<string>();
            foreach (var group in groups)
            {
                if (!string.IsNullOrWhiteSpace(group.Name))
                    names.Add(group.Name);
            }

            var json = JsonSerializer.Serialize(names, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_filePath, json);
        }
    }
}
