# Host Manager

WPF (.NET 8)로 개발된 모던 Windows hosts 파일 관리 도구입니다.

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-MIT-green)

## 📋 개요

Host Manager는 Windows hosts 파일(`C:\Windows\System32\drivers\etc\hosts`)을 쉽게 관리할 수 있는 GUI 애플리케이션입니다. 환경/그룹별 분류, 일괄 활성화/비활성화, 모던 UI 디자인을 지원합니다.

## ✨ 주요 기능

- **호스트 관리**: 호스트 항목 추가, 수정, 삭제
- **환경별 분류**: 환경별로 호스트 정리 (local, dev, staging, prod 등)
- **그룹별 분류**: 그룹별로 호스트 정리 (API, DB, Web 등)
- **활성화/비활성화 토글**: 삭제하지 않고 빠르게 호스트 활성화/비활성화
- **검색 및 필터**: 환경, 그룹, 검색어로 호스트 필터링
- **변경 표시**: 수정된 항목(노란색), 새 항목(초록색) 시각적 표시
- **자동 백업**: 최초 실행 시 원본 hosts 파일 자동 백업 (`hosts_prev_backup`)
- **수동 백업**: 언제든지 수동 백업 생성 가능 (`hosts_backup`)
- **시스템 트레이**: 백그라운드 실행을 위한 시스템 트레이 최소화
- **다국어 지원**: 시스템 언어 설정에 따라 영어/한국어 자동 지원

## 🚀 설치 방법

### 방법 1: 릴리즈 다운로드
1. [Releases](../../releases) 페이지에서 `HostManager.exe` 다운로드
2. 관리자 권한으로 실행 (hosts 파일 수정에 필요)

### 방법 2: 소스에서 빌드
```bash
# 저장소 클론
git clone https://github.com/yourusername/HostManager.git
cd HostManager

# 빌드
dotnet build

# 실행
dotnet run

# 배포용 빌드 (단일 파일)
dotnet publish -c Release -o publish
```

## 📖 사용 방법

### 키보드 단축키

| 단축키 | 동작 |
|--------|------|
| `Ctrl + H` | 가이드 열기 |
| `Ctrl + R` / `F5` | 새로고침 |
| `Ctrl + S` | 저장 |
| `Ctrl + N` | 새 호스트 추가 |
| `Delete` | 선택 항목 삭제 |
| `Ctrl + A` | 전체 선택/해제 |
| `Ctrl + F` | 검색창 포커스 |
| `Ctrl + 1` | 환경 드롭다운 토글 |
| `Ctrl + 2` | 그룹 드롭다운 토글 |
| `ESC` | 팝업 닫기 |

### 기본 사용법

1. **호스트 추가**: "추가" 버튼 클릭 또는 `Ctrl+N`
2. **호스트 수정**: 테이블의 필드를 클릭하여 직접 수정
3. **변경 저장**: "저장" 버튼 클릭 또는 `Ctrl+S`
4. **활성화/비활성화**: 체크박스로 호스트 선택 후 "활성화" 또는 "비활성화" 클릭
5. **필터링**: 환경/그룹 드롭다운 또는 검색창으로 호스트 필터링

### Hosts 파일 형식

Host Manager는 메타데이터 저장을 위해 특정 형식을 사용합니다:

```
# ==================== [local] ====================
# --- API ---
127.0.0.1 local.api.com # [Env:local] [Group:API] [Desc:로컬 API 서버]
#127.0.0.1 local.db.com # [Env:local] [Group:DB] [Desc:비활성화됨]

# ==================== [dev] ====================
# --- Web ---
192.168.1.100 dev.myapp.com # [Env:dev] [Group:Web] [Desc:개발 서버]
```

### 백업 파일

| 파일 | 설명 |
|------|------|
| `hosts_prev_backup` | 최초 실행 시 자동 생성 (원본 형식 보존) |
| `hosts_backup` | "백업" 버튼으로 수동 생성 |

위치: `C:\Windows\System32\drivers\etc\`

## ⚠️ 주의 사항

- **관리자 권한 필요**: hosts 파일 수정을 위해 관리자 권한으로 실행해야 합니다
- **백업 권장**: 중요한 변경 전에는 항상 백업을 권장합니다
- **형식 변환**: Host Manager는 메타데이터 지원을 위해 hosts 파일을 자체 형식으로 변환합니다

## 🛠️ 개발

### 요구 사항
- .NET 8.0 SDK
- Windows 10/11
- Visual Studio 2022 또는 VS Code

### 프로젝트 구조
```
HostManager/
├── Models/          # 데이터 모델
├── Views/           # WPF 뷰 (XAML)
├── ViewModels/      # MVVM 뷰모델
├── Services/        # 비즈니스 로직 서비스
├── Converters/      # WPF 값 변환기
├── Resources/       # 다국어 리소스
├── Styles/          # WPF 스타일
└── Tests/           # 단위 테스트
```

### 테스트 실행
```bash
cd Tests
dotnet test
```

## 📄 라이선스

이 프로젝트는 MIT 라이선스를 따릅니다. 자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

## 🤝 기여하기

1. 저장소 Fork
2. 기능 브랜치 생성 (`git checkout -b feature/AmazingFeature`)
3. 변경사항 커밋 (`git commit -m 'Add some AmazingFeature'`)
4. 브랜치에 Push (`git push origin feature/AmazingFeature`)
5. Pull Request 생성

---

[English README](README.md)
