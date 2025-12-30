using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HostManager.Models;

namespace HostManager.Services
{
    public class HostsFileService
    {
        private readonly string _hostsFilePath;
        private const string DefaultHostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        
        // IP 주소 정규식 (IPv4)
        private static readonly Regex IpRegex = new Regex(
            @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            RegexOptions.Compiled);

        // 호스트 라인 파싱 정규식
        // 형식: [#]IP주소 호스트명 # [Env:xxx] [Group:xxx] [Desc:xxx]
        private static readonly Regex HostLineRegex = new Regex(
            @"^(?<disabled>#)?\s*(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\s+(?<host>\S+)(?:\s*#\s*(?:\[Env:(?<env>[^\]]*)\])?\s*(?:\[Group:(?<group>[^\]]*)\])?\s*(?:\[Desc:(?<desc>[^\]]*)\])?)?",
            RegexOptions.Compiled);

        public HostsFileService() : this(DefaultHostsFilePath)
        {
        }

        public HostsFileService(string hostsFilePath)
        {
            _hostsFilePath = hostsFilePath;
        }

        public List<HostEntry> LoadHosts()
        {
            var entries = new List<HostEntry>();

            if (!File.Exists(_hostsFilePath))
                return entries;

            var lines = File.ReadAllLines(_hostsFilePath, Encoding.UTF8);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = HostLineRegex.Match(line);
                if (match.Success)
                {
                    var entry = new HostEntry
                    {
                        IpAddress = match.Groups["ip"].Value,
                        HostName = match.Groups["host"].Value,
                        Env = match.Groups["env"].Success ? match.Groups["env"].Value : string.Empty,
                        Group = match.Groups["group"].Success ? match.Groups["group"].Value : string.Empty,
                        Description = match.Groups["desc"].Success ? match.Groups["desc"].Value : string.Empty,
                        IsEnabled = !match.Groups["disabled"].Success,
                        OriginalLineNumber = i
                    };
                    entries.Add(entry);
                }
            }

            return entries;
        }

        public void SaveHosts(List<HostEntry> entries)
        {
            var lines = new List<string>();

            // 환경별 > 그룹별로 정렬
            var sortedEntries = entries
                .OrderBy(e => string.IsNullOrEmpty(e.Env) ? "zzz" : e.Env)
                .ThenBy(e => string.IsNullOrEmpty(e.Group) ? "zzz" : e.Group)
                .ThenBy(e => e.HostName)
                .ToList();

            string? currentEnv = null;
            string? currentGroup = null;

            // 정렬된 엔트리들 추가
            foreach (var entry in sortedEntries)
            {
                // 환경이 바뀌면 구분 주석 추가
                if (entry.Env != currentEnv)
                {
                    if (currentEnv != null)
                        lines.Add("");  // 빈 줄 추가
                    
                    var envName = string.IsNullOrEmpty(entry.Env) ? "미지정" : entry.Env;
                    lines.Add($"# ==================== [{envName}] ====================");
                    currentEnv = entry.Env;
                    currentGroup = null;  // 환경이 바뀌면 그룹도 리셋
                }

                // 그룹이 바뀌면 구분 주석 추가
                if (entry.Group != currentGroup)
                {
                    var groupName = string.IsNullOrEmpty(entry.Group) ? "미지정" : entry.Group;
                    lines.Add($"# --- {groupName} ---");
                    currentGroup = entry.Group;
                }

                var sb = new StringBuilder();

                // disabled인 경우 # 추가
                if (!entry.IsEnabled)
                    sb.Append("# ");

                sb.Append(entry.IpAddress);
                sb.Append(' ');
                sb.Append(entry.HostName);

                // 메타데이터가 있으면 추가
                if (!string.IsNullOrEmpty(entry.Env) || 
                    !string.IsNullOrEmpty(entry.Group) || 
                    !string.IsNullOrEmpty(entry.Description))
                {
                    sb.Append(" #");
                    
                    if (!string.IsNullOrEmpty(entry.Env))
                        sb.Append($" [Env:{entry.Env}]");
                    
                    if (!string.IsNullOrEmpty(entry.Group))
                        sb.Append($" [Group:{entry.Group}]");
                    
                    if (!string.IsNullOrEmpty(entry.Description))
                        sb.Append($" [Desc:{entry.Description}]");
                }

                lines.Add(sb.ToString());
            }

            File.WriteAllLines(_hostsFilePath, lines, Encoding.UTF8);
        }

        public static bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;
            return IpRegex.IsMatch(ip.Trim());
        }

        public static bool IsValidHostName(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                return false;
            
            // 간단한 호스트명 검증 (영문, 숫자, 하이픈, 점 허용)
            return Regex.IsMatch(hostName.Trim(), @"^[a-zA-Z0-9][a-zA-Z0-9\-\.]*[a-zA-Z0-9]$|^[a-zA-Z0-9]$");
        }
    }
}
