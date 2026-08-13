# PayGo

> **An intelligent financial operations layer for modern businesses.**

PayGo is a fintech platform designed to help merchants monitor payments, maintain a reliable financial record, reconcile payment-provider data, and identify potentially fraudulent transactions. It acts as an operational wrapper around payment providers (such as Paystack), turning raw payment events into trustworthy, traceable, and actionable financial information.

---

## 📖 Table of Contents
- [The Problem We Solve](#-the-problem-we-solve)
- [Core Capabilities](#-core-capabilities)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Financial Design Principles](#-financial-design-principles)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)

---

## 🎯 The Problem We Solve
As businesses scale, payment information becomes fragmented across provider dashboards, bank statements, and internal records. PayGo focuses on the questions that follow a payment:

*   Did the payment really happen?
*   How should it be recorded?
*   Does it match our internal records?
*   Does anything look suspicious?

PayGo is **not** a bank or a payment processor. It is a financial operations platform that provides merchant visibility, robust verification, automated reconciliation, and actionable notifications.

---

## ✨ Core Capabilities
*   **Merchant & Identity:** Secure onboarding, API credentials, and role-based access control.
*   **Payments:** Payment link creation, Paystack integration, and secure webhook ingestion.
*   **Financial Ledger:** Double-entry-oriented records that guarantee immutable/append-only financial events. Balances are derived from these controlled records.
*   **Reconciliation:** Compare provider records with PayGo records to identify mismatches and track exceptions.
*   **Fraud & Risk:** Rule-based checks, velocity limits, and anomaly detection to identify suspicious activity.
*   **Notifications & Analytics:** Timely alerts for failed, refunded, or suspicious events, paired with a robust reporting dashboard.

---

## 🏗️ Architecture
PayGo is built as a **Modular Monolith** using Clean Architecture principles. This provides strong domain boundaries without the operational complexity of microservices too early in the product lifecycle.

**Workflow:**
1. Customer initiates payment.
2. Provider (e.g., Paystack) processes and sends a webhook to PayGo.
3. PayGo validates and handles the event idempotently.
4. Transaction is stored, and double-entry ledger records are created.
5. Fraud engine evaluates the transaction risk.
6. Reconciliation processes run in the background.

---

## 💻 Technology Stack

| Component | Technology | Reason |
| :--- | :--- | :--- |
| **Backend** | .NET 10 (ASP.NET Core) | Strong typing, mature ecosystem, enterprise capabilities. |
| **Architecture** | Clean Architecture + CQRS | Separation of concerns and maintainable application boundaries. |
| **Database** | PostgreSQL + EF Core | Reliable relational storage and strong transactional capabilities. |
| **Caching** | Redis | Fast access to frequently used and temporary data. |
| **Messaging** | RabbitMQ | Asynchronous processing and decoupling. |
| **Background Jobs** | Hangfire | Scheduled processing and retry mechanisms. |
| **Frontend** | React + TypeScript | Responsive, modern merchant dashboard. |

---

## ⚖️ Financial Design Principles
When contributing to PayGo, adhere to the following principles:

*   **Never trust client-side status:** Provider webhooks must be authenticated and processed idempotently.
*   **Derived Balances:** Balances are derived from controlled financial ledger records, never arbitrarily updated via UI.
*   **Double-Entry:** Refunds and reversals must be explicitly represented as ledger entries.
*   **Decimal Precision:** Monetary values use decimal representations, never floating-point arithmetic.
*   **Immutable State:** Every important financial mutation should have a strict audit trail.

---

## 🚀 Getting Started

### Prerequisites
*   .NET 10 SDK
*   PostgreSQL
*   Redis
*   RabbitMQ
*   EF Core CLI Tools (`dotnet tool install --global dotnet-ef`)

### Local Setup

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/danielseraph/paygo-api.git](https://github.com/danielseraph/paygo-api.git)
   cd paygo-api
