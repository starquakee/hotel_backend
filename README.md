# 酒店后端管理系统 / Hotel Backend Management System

这是一个基于 .NET 6.0 的酒店管理系统后端项目，为面向对象分析与设计 (OOAD) 课程开发。

*This is a hotel management system backend project based on .NET 6.0, developed for the Object-Oriented Analysis and Design (OOAD) course.*

## 技术栈 / Technology Stack

查看完整的技术栈文档：[TECH_STACK.md](./TECH_STACK.md)

*For a complete technology stack documentation, see: [TECH_STACK.md](./TECH_STACK.md)*

### 核心技术 / Core Technologies
- **.NET 6.0** - ASP.NET Core Web API
- **MySQL** - 数据库 / Database  
- **Entity Framework Core** - ORM 框架 / ORM Framework
- **JWT** - 身份验证 / Authentication
- **支付宝 SDK / Alipay SDK** - 支付集成 / Payment Integration
- **Docker** - 容器化部署 / Containerized Deployment

## 快速开始 / Quick Start

### 环境要求 / Prerequisites
- .NET 6.0 SDK
- MySQL 8.0+
- Docker (可选 / Optional)

### 本地运行 / Local Development
```bash
# 克隆项目 / Clone the repository
git clone https://github.com/starquakee/hotel_backend.git

# 进入项目目录 / Navigate to project directory
cd hotel_backend

# 还原包依赖 / Restore packages
dotnet restore

# 运行项目 / Run the project
dotnet run --project HotelManagement
```

### Docker 部署 / Docker Deployment
```bash
# 构建镜像 / Build image
docker build -t hotel-backend -f HotelManagement/Dockerfile .

# 运行容器 / Run container
docker run -p 8080:8080 hotel-backend
```

## 项目结构 / Project Structure

- `HotelManagement/` - 主要的 Web API 项目 / Main Web API project
- `Models/` - 数据模型和数据库上下文 / Data models and database context
- `JwtUtils/` - JWT 安全工具 / JWT security utilities
- `alipay-sdk-NET20170615110549/` - 支付宝 SDK / Alipay SDK
