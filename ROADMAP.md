# Fast-Native — AI & Developer Foundation Roadmap

## 1. Project Vision

Fast-Native is an opinionated, modular, full-stack foundation designed to help developers go from an idea to a deployable service with minimal infrastructure decisions.

The goal is NOT to build a template for a specific type of product.

Fast-Native must remain product-agnostic.

It should be possible to use the same foundation to build:

- A simple static/informational website
- A service website
- A restaurant website
- A CRUD application
- An authenticated web application
- A dashboard
- A SaaS product
- An API service
- A real-time application
- A background-processing service
- An AI-powered application
- A 3D/WebGL experience (Three.js, Babylon.js, etc.)
- A complex production web application

The difference between these products should primarily be the modules implemented on top of Fast-Native, not a completely different project architecture.

The core philosophy is:

> Python backend + a web kit you choose + native Windows and Android shells.

A developer should be able to use Python for the backend, business logic, database layer, background processing, testing, and deployment workflow. The dashboard UI comes from Fast-Svelte, Fast-Next, Fast-Nuxt, or Fast-Rio, installed with `web use`. Windows (WinUI 3) and Android (Jetpack Compose) are extra clients of the same API. They are not a window around the website.

The installed web kit provides the dashboard.
WinUI 3 and Jetpack Compose provide the native shells.
FastAPI provides the backend/API.
PostgreSQL provides persistent data storage.
ARQ + Redis provide background processing infrastructure.
Docker and Traefik provide the production runtime foundation.

Fast-Native is intended to be a launchpad, not a finished product.

---

# 2. Primary Goal

The primary goal is to make Fast-Native an excellent foundation for AI-assisted development and Vibe Coding.

The developer should be able to start with:

> "I have an idea for a service."

and end with:

> "I have a working and deployable service."

without having to repeatedly make fundamental architectural decisions.

AI coding agents should not invent the architecture of every new feature.

They should work inside the existing architecture.

The project structure, conventions, documentation, examples, CLI tooling, and AI instructions should act as architectural guardrails.

The objective is NOT to prevent AI from writing code.

The objective is to make AI-generated code naturally conform to the project.

---

# 3. Core Principles

## 3.1 Product Agnostic

Do not introduce product-specific assumptions into the core project.

Do NOT turn Fast-Native into:

- An AI template
- A CMS template
- A SaaS template
- An e-commerce template
- A restaurant template
- A CRM template
- A chatbot template

All of these should be possible applications built on top of Fast-Native.

The foundation must remain generic.

---

## 3.2 Opinionated Architecture

Fast-Native intentionally makes architectural decisions.

The goal is to reduce unnecessary decisions for both developers and AI agents.

When a convention already exists:

> Reuse it.

Do not introduce a second pattern for solving the same problem.

AI agents should prefer existing project conventions over inventing new abstractions.

---

## 3.3 Modular by Default

Features should be implemented as modules.

A feature should have a clear ownership boundary.

Conceptually:

    backend/app/modules/apps/<feature>/
    frontend/web/.../modules/apps/<feature>/     # after web use
    frontend/win/.../routes/<feature>/           # when the path is shared
    frontend/android/.../routes/<feature>/

A module may contain only the files it actually needs.

Do not create unnecessary files just to satisfy a theoretical architecture.

For example:

    feature/
    ├── router.py
    ├── service.py
    ├── repository.py
    ├── models.py
    └── schemas.py

is valid when the feature requires all of them.

A simple feature may need fewer layers.

The architecture should be consistent without becoming ceremonial.

---

# 4. Backend Architecture

The preferred backend flow is:

    Request
       ↓
    Router
       ↓
    Service
       ↓
    Repository
       ↓
    Database

Responsibilities:

### Router

Responsible for:

- HTTP concerns
- Request/response handling
- Dependency injection
- Authentication/authorization integration
- Calling services

Routers should NOT contain complex business logic.

### Service

Responsible for:

- Business rules
- Application logic
- Coordinating repositories
- Coordinating external services
- Transaction-level application behavior

### Repository

Responsible for:

- Database access
- Queries
- Persistence operations

Do not put business rules inside repositories.

### Models

Responsible for database representation.

### Schemas

Responsible for API input/output contracts.

Do not automatically merge database models and API schemas unless there is a clear reason.

---

# 5. Frontend Architecture

The web UI is not baked into this repo. `frontend/web` stays empty until `web use` downloads Fast-Svelte, Fast-Next, Fast-Nuxt, or Fast-Rio.

That kit is a first-class client of the FastAPI backend. JS kits keep the full npm ecosystem (Three.js, Babylon.js, charting libraries). Rio stays Python. Backend conventions stay in Python either way.

Windows (`frontend/win`, WinUI 3) and Android (`frontend/android`, Jetpack Compose) are additional clients. They share `frontend/client-routes.json` and paint their own chrome. `mac`, `linux`, and `ios` are reserved slots and are not implemented yet.

After `web use`, feature UI follows that kit's module layout (`modules/base` and `modules/apps` under `frontend/web`). Do not add a project-wide `components/` folder. Native shell code stays in `modules/global`. Native pages stay under `routes/`.

Do not paste a downloaded kit's UI into the native apps. Re-implement the shared route in that shell.

The architecture should make it obvious where an AI agent should implement a new UI feature: the installed kit for the dashboard, and the native `routes/` folders when the path is shared.

---

# 6. Redis + ARQ

Redis and ARQ are part of the official Fast-Native infrastructure.

They should NOT be described as "optional technology" or as an unrelated add-on.

Fast-Native officially supports two runtime profiles:

## Full Runtime

All supported infrastructure is started.

Example:

    Dev run all

This should include the infrastructure required for:

- FastAPI
- The installed web kit on port 5000
- PostgreSQL
- Redis
- ARQ/background workers
- Reverse proxy/infrastructure where applicable

## Slim Runtime

Fast-Native also officially supports a lightweight development/runtime mode.

Example:

    Dev run all --slim

Slim mode should start only the infrastructure required for applications that do not need the additional services.

The distinction is:

> Full is the complete Fast-Native runtime.

> Slim is the lightweight Fast-Native runtime.

Slim mode is NOT a workaround.

Slim mode is NOT a fallback.

Slim mode is an official supported operating mode of Fast-Native.

The CLI should make this distinction explicit.

---

# 7. Ctrl / CLI

The `__ctrl__` directory is an important part of the developer experience.

It should act as the control layer for the project.

The goal is to allow developers to operate the entire development environment through a small and predictable CLI.

Examples:

    Dev run all

    Dev run all --slim

The CLI should provide a consistent interface for:

- Starting development infrastructure
- Stopping infrastructure
- Resetting development infrastructure
- Running services
- Selecting runtime profiles
- Passing configuration parameters
- Running tests
- Running project checks
- Deployment-related operations where applicable

The CLI should remain simple.

Do not turn `__ctrl__` into an unnecessarily complex framework.

---

# 8. AI Development Contract

Create and maintain a root-level AI instruction document such as:

    AGENTS.md

This document must explain how AI agents are expected to work inside Fast-Native.

AI agents must:

1. Read the project instructions before making architectural changes.
2. Inspect existing modules before creating new patterns.
3. Reuse existing infrastructure.
4. Follow the existing module structure.
5. Keep business logic out of routers.
6. Keep database access inside repositories.
7. Keep API schemas separate from database models where appropriate.
8. Add database migrations when database structure changes.
9. Add tests for meaningful business logic.
10. Avoid unnecessary dependencies.
11. Avoid unnecessary architectural changes.
12. Avoid modifying core infrastructure for a feature-specific requirement unless absolutely necessary.
13. Prefer the smallest change that correctly implements the requested feature.
14. Preserve existing conventions.
15. Update documentation when behavior or developer workflow changes.

The AI should not redesign Fast-Native simply because it personally prefers another architecture.

---

# 9. Canonical Example Module

Fast-Native should contain one intentionally simple but complete example module.

The example should demonstrate the complete lifecycle of a feature:

    Module
       ↓
    Database Model
       ↓
    Migration
       ↓
    Repository
       ↓
    Service
       ↓
    Router
       ↓
    Web kit UI
    ↓
    Native pages for shared routes
    ↓
    Tests

The example should be product-neutral.

Do not use an AI-specific, e-commerce-specific, or other product-specific example.

The example exists to teach both developers and AI agents:

> "This is how a feature is implemented in Fast-Native."

AI agents should be instructed to inspect this module before implementing a new module.

---

# 10. Module Scaffolding

Evaluate and, if appropriate, implement a CLI command for creating modules.

Example:

    fast-svelte app create <name>

or the equivalent command through `__ctrl__`.

The purpose is to remove repetitive structural decisions from both developers and AI agents.

A module generator should create only the necessary structure and should follow the project's current conventions.

Do not generate unnecessary boilerplate.

---

# 11. Background Jobs

Background processing should have a clear first-class extension point.

ARQ should be the preferred background job mechanism.

The foundation should support common use cases such as:

- Long-running operations
- File processing
- External API operations
- Scheduled/queued work
- AI inference/generation jobs
- Email processing
- Cleanup tasks

The core should remain generic.

Do not add product-specific workers.

The objective is to make background processing predictable when a future application requires it.

---

# 12. Storage

Evaluate adding a generic storage abstraction for application files.

Potential use cases include:

- User uploads
- Images
- Documents
- Generated files
- Application assets

The abstraction should allow different implementations without forcing every project to use the same storage provider.

Potential implementations may include:

- Local storage
- S3-compatible storage

Do not hard-code a specific cloud provider into the core architecture.

The important part is defining a clean extension point.

---

# 13. Logging and Error Handling

Establish clear project-wide conventions for:

- Application logging
- Error handling
- API errors
- Unexpected exceptions
- Background job failures

AI agents should not invent a new error-handling pattern for every module.

There should be a recognizable project-wide approach.

The implementation should remain lightweight.

Do not introduce a large observability stack simply for the sake of completeness.

---

# 14. Configuration

Keep configuration centralized and predictable.

AI agents should know:

- Where environment variables belong
- Where application settings belong
- How development settings differ from production settings
- How runtime profiles such as Slim/Full are selected

Avoid scattering environment access throughout the application.

---

# 15. Documentation Structure

Documentation should serve two audiences:

## Developer

A developer should be able to understand:

- What Fast-Native is
- Why the architecture exists
- How to start it
- How modules work
- How to add a feature
- How to work with the database
- How to run tests
- How to use the CLI
- How to deploy

## AI Agent

An AI agent should be able to understand:

- Project architecture
- File ownership
- Module conventions
- Coding rules
- Dependency rules
- Testing expectations
- Migration expectations
- Deployment workflow
- CLI usage
- Runtime profiles
- What it must NOT change

Documentation should be written so that an AI can reliably follow it.

---

# 16. Suggested Documentation Files

Evaluate creating/maintaining:

    README.md
    AGENTS.md

Potential additional documentation:

    docs/
    ├── architecture.md
    ├── modules.md
    ├── development.md
    ├── testing.md
    ├── database.md
    ├── deployment.md
    ├── cli.md
    └── conventions.md

Do not create documentation files merely for the sake of having more files.

Each document should answer a real question.

---

# 17. AI/Vibe Coding Workflow

The intended workflow should be approximately:

    1. Clone Fast-Native
             ↓
    2. Start the environment
             ↓
    3. Give AI the project idea
             ↓
    4. AI reads AGENTS.md
             ↓
    5. AI inspects existing modules
             ↓
    6. AI creates/extends modules
             ↓
    7. AI creates migrations
             ↓
    8. AI implements backend
             ↓
    9. AI implements the web kit UI and native pages for shared routes
             ↓
    10. AI writes/updates tests
             ↓
    11. Developer reviews
             ↓
    12. Run project checks
             ↓
    13. Deploy
             ↓
    14. Working service

The developer should primarily provide:

- Product requirements
- Business rules
- UI requirements
- Integration requirements

The developer should NOT need to repeatedly explain basic project architecture to the AI.

---

# 18. Definition of Done

A feature is not considered complete when the code merely works locally.

A meaningful feature should satisfy the relevant parts of:

- Correct module location
- Correct architectural layers
- Database migration
- API schemas
- Authentication/authorization where required
- Web kit UI, and a native page when the path is shared
- Tests
- Error handling
- Logging
- Documentation
- CLI/development compatibility
- Production compatibility

Not every feature requires every layer.

The rule is:

> Use the smallest appropriate architecture for the feature while respecting the project's conventions.

---

# 19. Deployment Philosophy

Fast-Native should remain usable from the first local development command through production deployment.

The project should provide a clear path:

    Development
        ↓
    Testing
        ↓
    Production Build
        ↓
    Deployment
        ↓
    Running Service

Deployment documentation should be understandable to a developer who has never deployed a FastAPI application before.

Avoid requiring users to understand the entire infrastructure stack before they can deploy.

The goal is:

> Simple enough for a beginner, structured enough for production.

---

# 20. Do Not Overengineer

This is a critical rule.

Fast-Native must remain simple.

Do not add technologies merely because they are common in large systems.

Do not turn the project into:

- Microservice architecture
- Kubernetes platform
- Enterprise framework
- Complex DDD implementation
- Massive DevOps framework

unless there is a direct architectural requirement.

Every addition should answer:

> Does this make Fast-Native a better general-purpose foundation for developers and AI agents?

If not, do not add it.

---

# 21. Generality Test

Before adding a feature to the core, ask:

> Could this reasonably be useful for multiple fundamentally different applications?

Examples:

### Good core candidates

- Authentication
- Database
- Migrations
- Testing
- Modular architecture
- Background jobs
- Redis
- File storage abstraction
- Logging
- Configuration
- Deployment
- CLI
- AI development rules

### Product-specific candidates

- ChatGPT-like conversation model
- Restaurant ordering system
- E-commerce cart
- AI prompt library
- CRM pipeline
- Game inventory

Product-specific functionality belongs in application modules, not the Fast-Native core.

---

# 22. Acceptance Criteria for Fast-Native

Fast-Native should eventually satisfy the following scenario:

A developer with basic Python knowledge can:

    1. Clone the repository.
    2. Start it with the documented CLI.
    3. Understand the basic structure.
    4. Ask an AI coding agent to build a new feature.
    5. Have the AI implement the feature inside the existing architecture.
    6. Run the application.
    7. Run tests.
    8. Make configuration changes.
    9. Deploy the resulting application.

The developer should not need to become an expert in:

- FastAPI architecture
- The installed web kit
- Docker
- Reverse proxies
- SSL
- Database migrations
- Background workers
- Project structure

before being able to build something useful.

Fast-Native provides the runway.

The developer provides the idea.

AI helps build the product.

---

# 23. Final Philosophy

Fast-Native should feel like a launchpad.

Not a framework that dictates what you build.

Not a collection of random boilerplate.

Not an AI product template.

Not a dashboard template.

Not a SaaS template.

It is a structured foundation that answers the infrastructure and architecture questions before development begins.

The intended experience is:

    Idea
      ↓
    Fast-Native
      ↓
    AI-assisted development
      ↓
    Modular implementation
      ↓
    Tests
      ↓
    Deployment
      ↓
    Working Service

The long-term statement of the project is:

> **Python backend + a chosen web kit + native shells = the product surface.**
>
> **Fast-Native provides the runway.**
>
> **You provide the idea.**
>
> **AI helps build the service.**
