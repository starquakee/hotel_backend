# 酒店后端管理系统技术栈 / Hotel Backend Management System Technology Stack

## 核心框架与语言 / Core Framework & Language

- **.NET 6.0** - 跨平台的现代 .NET 框架 / Cross-platform modern .NET framework
- **ASP.NET Core Web API** - 用于构建 RESTful API 的 Web 框架 / Web framework for building RESTful APIs
- **C#** - 主要编程语言 / Primary programming language

## 数据库与 ORM / Database & ORM

- **MySQL 8.0** - 关系型数据库管理系统 / Relational database management system
- **Entity Framework Core 6.0.10** - 对象关系映射 (ORM) 框架 / Object-Relational Mapping (ORM) framework
- **Pomelo.EntityFrameworkCore.MySql 6.0.2** - MySQL 的 EF Core 提供程序 / EF Core provider for MySQL
- **Pomelo.EntityFrameworkCore.MySql.Json.Microsoft 6.0.2** - MySQL JSON 支持 / MySQL JSON support
- **Entity Framework Migrations** - 数据库迁移支持 / Database migration support

## 身份验证与安全 / Authentication & Security

- **JWT (JSON Web Tokens)** - 基于令牌的身份验证 / Token-based authentication
- **Microsoft.AspNetCore.Authentication.JwtBearer 6.0.10** - JWT 身份验证中间件 / JWT authentication middleware
- **BCrypt.Net-Next 4.0.3** - 密码哈希和验证 / Password hashing and verification
- **自定义 SecurityUtils 项目** - 自定义的安全工具库 / Custom security utilities library

## 支付集成 / Payment Integration

- **支付宝 SDK / Alipay SDK** - 支付宝支付集成 / Alipay payment integration
  - `Alipay.AopSdk.AspnetCore 2.5.0.1` - 支付宝开放平台 SDK / Alipay Open Platform SDK
  - `Essensoft.AspNetCore.Payment.Security 3.3.2` - 支付安全组件 / Payment security components
  - `Essensoft.Paylink.Alipay 4.0.14` - 支付宝支付链接库 / Alipay payment link library

## 基础设施与部署 / Infrastructure & Deployment

- **Docker** - 容器化部署 / Containerized deployment
  - 基于 `mcr.microsoft.com/dotnet/aspnet:6.0-alpine` 的轻量级镜像 / Lightweight image based on alpine
  - 多阶段构建优化 / Multi-stage build optimization
- **Kestrel Web Server** - 高性能跨平台 Web 服务器 / High-performance cross-platform web server
- **CORS 支持** - 跨域资源共享 / Cross-Origin Resource Sharing support

## 项目架构 / Project Architecture

### 多项目解决方案结构 / Multi-Project Solution Structure

1. **HotelManagement** - 主要的 Web API 项目 / Main Web API project
   - 控制器和业务逻辑 / Controllers and business logic
   - 依赖注入配置 / Dependency injection configuration
   - 中间件配置 / Middleware configuration

2. **Models** - 数据模型项目 / Data models project
   - 实体类定义 / Entity class definitions
   - 数据库上下文 (DbContext) / Database context
   - 数据库关系配置 / Database relationship configuration

3. **SecurityUtils (JwtUtils)** - 安全工具项目 / Security utilities project
   - JWT 令牌生成和验证 / JWT token generation and validation
   - 密码加密和验证 / Password encryption and validation

4. **AopSdk** - 支付宝 SDK 项目 / Alipay SDK project
   - 支付宝 API 集成 / Alipay API integration

## 开发工具与配置 / Development Tools & Configuration

- **JSON 序列化配置** / JSON Serialization Configuration
  - 驼峰命名策略 / Camel case naming policy
  - 忽略 null 值 / Ignore null values
  - 开发环境美化输出 / Pretty printing in development

- **错误处理** / Error Handling
  - 自定义异常过滤器 / Custom exception filters
  - 全局错误处理 / Global error handling

- **配置管理** / Configuration Management
  - `appsettings.json` - 应用程序配置 / Application configuration
  - 环境特定配置 / Environment-specific configuration
  - 数据库连接字符串管理 / Database connection string management

## 主要功能模块 / Main Functional Modules

- **用户管理** / User Management
- **酒店管理** / Hotel Management
- **员工管理** / Employee Management
- **房间管理** / Room Management (客房、洗衣房、健身房、会议室等 / Guest rooms, laundry, gym, meeting rooms, etc.)
- **订单管理** / Order Management
- **预订管理** / Reservation Management
- **入住记录** / Check-in Records
- **支付处理** / Payment Processing

## 数据库表结构 / Database Table Structure

- `CompanyGroups` - 公司集团 / Company groups
- `HotelInstances` - 酒店实例 / Hotel instances
- `EmployeeInstances` - 员工实例 / Employee instances
- `Orders` - 订单 / Orders
- `ReserveOrders` - 预订订单 / Reservation orders
- `GuestRooms` - 客房 / Guest rooms
- `LaundryRooms` - 洗衣房 / Laundry rooms
- `GymRooms` - 健身房 / Gym rooms
- `MeetingRooms` - 会议室 / Meeting rooms
- `StaffRooms` - 员工房间 / Staff rooms
- `CheckInRecords` - 入住记录 / Check-in records
- `Users` - 用户 / Users
- `Administrations` - 管理 / Administrations
- `Awards` - 奖励 / Awards

## 网络与通信 / Network & Communication

- **HTTP/1.1 协议** / HTTP/1.1 protocol
- **RESTful API 设计** / RESTful API design
- **端口 8080** - 默认监听端口 / Default listening port
- **跨域请求支持** / Cross-origin request support

这个项目是一个现代化的酒店管理系统后端，采用了 .NET 6.0 技术栈，集成了支付宝支付系统，支持 Docker 容器化部署，具有完整的用户认证、酒店管理、订单处理等功能。

*This project is a modern hotel management system backend built with .NET 6.0 technology stack, integrated with Alipay payment system, supports Docker containerized deployment, and includes complete user authentication, hotel management, and order processing functionalities.*