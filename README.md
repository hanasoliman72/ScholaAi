# 🎓 ScholaAi — Backend API

> The core .NET 8 Web API powering the ScholaAi tutoring platform — handling authentication, session management, payments, focus reporting, and real-time notifications.

---

## 📋 Overview

This is the **central REST API** for the ScholaAi platform. It connects the React frontend and the Python AI services to a **SQL Server** database via **Entity Framework Core**, exposes a **SignalR** hub for real-time events, and integrates **Stripe** for payment processing.

---

## 🏗️ Architecture

```
React Frontend          Python AI Services
      │                       │
      │  REST / SignalR        │  REST (focus score, summaries)
      ▼                       ▼
┌──────────────────────────────────────────┐
│           ScholaAi .NET 8 API            │
│                                          │
│  Controllers ─── Services ─── Repos      │
│       │                         │        │
│    SignalR Hub            EF Core        │
│    (real-time)          SQL Server DB    │
└──────────────────────────────────────────┘
         │                   │
         │                   └── Supabase (file storage)
         └── Stripe (payments)
```

---

## ⚙️ Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 8.0 | Runtime & framework |
| ASP.NET Core Web API | 8.0 | REST endpoints |
| Entity Framework Core | 8.0.22 | ORM / database access |
| SQL Server | — | Primary database |
| ASP.NET Core Identity | 8.0 | User management & password hashing |
| JWT Bearer Auth | 8.0.22 | Stateless authentication |
| SignalR | 1.2.9 | Real-time push (focus alerts, chat) |
| Stripe.net | 51.1 | Payment processing |
| Swashbuckle / Swagger | 6.6.2 | API documentation |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (local or remote)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code with C# extension

### Configuration

Update `appsettings.Development.json` with your local values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ScholaAiDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<your-secret-key>",
    "Issuer": "ScholaAi",
    "Audience": "ScholaAiUsers"
  },
  "Stripe": {
    "SecretKey": "sk_test_..."
  }
}
```

### Database Setup

```bash
# Apply all EF Core migrations
dotnet ef database update
```

### Running

```bash
dotnet run
```

The API will start on **`http://localhost:5254`** (or the port configured in `launchSettings.json`).  
Swagger UI is available at: `http://localhost:5254/swagger`

---

## 📁 Project Structure

```
ScholaAi/
├── Controllers/
│   ├── accountController.cs           # Register, login, delete account
│   ├── AdminController.cs             # User management, subjects, platform stats
│   ├── CalendarController.cs          # Session scheduling
│   ├── ChatController.cs              # In-session messaging
│   ├── NotificationsController.cs     # Push notifications
│   ├── PaymentController.cs           # Stripe payment intents
│   ├── ratingController.cs            # Teacher ratings & reviews
│   ├── SessionStreamController.cs     # WebRTC session join/leave
│   ├── StudentDashboardController.cs  # Student home data
│   ├── TeacherDashboardController.cs  # Teacher home data
│   ├── studentProfileController.cs    # Student profile CRUD
│   ├── teacherProfileController.cs    # Teacher profile, students list
│   ├── studentSessionsController.cs   # Session lifecycle (student side)
│   ├── teacherSessionsController.cs   # Session lifecycle, recording upload
│   └── UploadController.cs            # File upload to Supabase
│
├── DTOs/                              # Request/Response transfer objects
├── Hubs/                              # SignalR hub definitions
├── Models/                            # EF Core entity models
├── Repositories/                      # Data access layer
├── Services/                          # Business logic services
├── SignalR/                           # Real-time event helpers
├── Migrations/                        # EF Core database migrations
├── Program.cs                         # App bootstrapping & DI registration
├── appsettings.json
└── appsettings.Development.json       # Local overrides (not committed)
```

---

## 🔌 Key API Endpoints

### Authentication
| Method | Route | Description |
|---|---|---|
| `POST` | `/api/account/register` | Register a new user (teacher or student) |
| `POST` | `/api/account/login` | Login and receive a JWT token |
| `DELETE` | `/api/account/delete` | Delete account (cascades all related data) |

### Sessions (Teacher)
| Method | Route | Description |
|---|---|---|
| `POST` | `/api/teacherSessions/create` | Create a new session |
| `POST` | `/api/teacherSessions/end` | End a session, trigger recording upload & summarization |
| `POST` | `/api/teacherSessions/uploadRecording` | Upload session recording to Supabase |

### Sessions (Student)
| Method | Route | Description |
|---|---|---|
| `GET` | `/api/studentSessions/upcoming` | List upcoming sessions |
| `POST` | `/api/studentSessions/{id}/report-focus` | Receive focus score from AI agent |
| `POST` | `/api/studentSessions/{id}/notify-distraction` | Trigger distraction alert via SignalR |
| `POST` | `/api/studentSessions/{id}/rate` | Submit a post-session rating |

### Admin
| Method | Route | Description |
|---|---|---|
| `GET` | `/api/Admin/subjects` | List all available subjects |
| `GET` | `/api/Admin/users` | List all platform users |
| `GET` | `/api/Admin/revenue` | Monthly platform revenue (5 EGP fee per session) |

### Profiles & Ratings
| Method | Route | Description |
|---|---|---|
| `GET` | `/api/teacherProfile/{id}` | Get teacher profile |
| `GET` | `/api/teacherProfile/myStudents` | Get teacher's student list |
| `GET` | `/api/rating/{teacherId}` | Get all ratings for a teacher |
| `POST` | `/api/rating` | Submit a teacher rating |

---

## 🔔 SignalR Hub

The real-time hub (`/hubs/session`) pushes the following events to connected clients:

| Event | Direction | Description |
|---|---|---|
| `StudentDistracted` | Server → Teacher | Student focus dropped below 50% |
| `NewMessage` | Server → Client | New chat message in session |
| `SessionEnded` | Server → Client | Teacher ended the session |

---

## 🔗 Related Services

| Service | Repo | Description |
|---|---|---|
| Frontend | `ScholaAi-Front-End` | React application |
| Session Server | `ScholaAi-mediasoup-server` | WebRTC media routing |
| AI Model Hub | `ScholaAi-model-hub` | Focus detection & summarization |

---

## 📄 License

MIT
