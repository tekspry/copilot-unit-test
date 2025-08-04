# E-Commerce Application Architecture

This document provides a visual representation of our microservices-based e-commerce application architecture using DAPR (Distributed Application Runtime).

## System Overview

```mermaid
graph TB
    subgraph Frontend[Frontend Layer]
        SPA[React SPA<br>Port:3000]
    end

    subgraph Services[Microservices Layer]
        subgraph DAPR[DAPR Runtime Sidecars]
            PS[Product Service<br>Port:5016]
            OS[Order Service<br>Port:5206]
            PMS[Payment Service<br>Port:5293]
            NS[Notification Service<br>Port:5294]
            CS[Customer Service<br>Port:5295]
        end
    end

    SPA --> PS
    SPA --> OS
    SPA --> CS

    OS --> PS
    OS --> CS
    
    PMS --> OS
    
    NS --> OS
    NS --> CS
    NS --> PS

    classDef frontend fill:#2ecc71,stroke:#fff,stroke-width:2px,color:#fff
    classDef service fill:#3498db,stroke:#fff,stroke-width:2px,color:#fff
    class SPA frontend
    class PS,OS,PMS,NS,CS service
```

## Service Communication Flow

```mermaid
sequenceDiagram
    participant SPA as React SPA
    participant PS as Product Service
    participant OS as Order Service
    participant PMS as Payment Service
    participant NS as Notification Service
    
    SPA->>PS: 1. Browse/Manage Products
    SPA->>OS: 2. Place Order
    OS->>PS: 3. Check & Update Inventory
    OS->>PMS: 4. Process Payment
    PMS-->>OS: 5. Payment Status
    OS-->>NS: 6. Order Status Updated
    NS-->>SPA: 7. Email Notification
```

## Clean Architecture Implementation

```mermaid
graph TB
    subgraph "Clean Architecture Layers"
        API[API Controllers]
        App[Application Layer]
        Domain[Domain Layer]
        Infra[Infrastructure Layer]
        DB[(Database)]
        
        API --> App
        App --> Domain
        App --> Infra
        Infra --> Domain
        Infra --> DB
        
        classDef layer fill:#9b59b6,stroke:#fff,stroke-width:2px,color:#fff
        class API,App,Domain,Infra layer
    end
```

## DAPR Building Blocks Usage

```mermaid
graph LR
    subgraph "DAPR Building Blocks"
        SI[Service Invocation] --> SM[State Management]
        PS[Pub/Sub] --> SM
        B[Bindings]
    end
    
    subgraph "Services"
        Product[Product Service]
        Order[Order Service]
        Payment[Payment Service]
        Notification[Notification Service]
    end
    
    Product --> SI
    Order --> SI
    Order --> PS
    Payment --> PS
    Notification --> PS
    
    classDef dapr fill:#e74c3c,stroke:#fff,stroke-width:2px,color:#fff
    classDef service fill:#3498db,stroke:#fff,stroke-width:2px,color:#fff
    class SI,PS,SM,B dapr
    class Product,Order,Payment,Notification service
```

## Event-Driven Architecture

```mermaid
graph LR
    subgraph "Order Events Flow"
        Order[Order Service] --> |Publishes| Events[Order Events]
        Events --> |Subscribes| Payment[Payment Service]
        Events --> |Subscribes| Notification[Notification Service]
    end
    
    subgraph "Event Types"
        OP[Order Placed]
        OC[Order Confirmed]
        PP[Payment Pending]
        PC[Payment Confirmed]
    end
    
    Events --> OP
    Events --> OC
    Events --> PP
    Events --> PC
    
    classDef service fill:#3498db,stroke:#fff,stroke-width:2px,color:#fff
    classDef event fill:#f1c40f,stroke:#fff,stroke-width:2px,color:#fff
    class Order,Payment,Notification service
    class OP,OC,PP,PC event
```

## Service Details

### Ports & Endpoints
- Frontend (React SPA): `http://localhost:3000`
- Product Service: `http://localhost:5016`
- Order Service: `http://localhost:5206`
- Payment Service: `http://localhost:5293`
- Notification Service: `http://localhost:5294`
- Customer Service: `http://localhost:5295`

### DAPR Sidecar Ports
- Product Service: 3501
- Order Service: 3502
- Payment Service: 3503
- Notification Service: 3504
- Customer Service: 3505

## Key Architecture Characteristics

1. **Microservices Architecture**
   - Independent deployable services
   - Service-specific databases
   - DAPR-based communication

2. **Clean Architecture**
   - Separation of concerns
   - Domain-driven design
   - Layered architecture

3. **Event-Driven Design**
   - Asynchronous communication
   - Pub/sub messaging
   - Event-based workflows

4. **Cloud-Native Features**
   - Container support
   - Service discovery
   - Distributed tracing
   - State management

5. **Frontend Architecture**
   - React Single Page Application
   - Component-based design
   - React Router for navigation
   - RESTful API communication
