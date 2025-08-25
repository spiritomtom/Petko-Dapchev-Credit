# Credit Approval API

## Description

This project is a simplified backend API for managing credit approvals within a hypothetical company. The API allows users to submit credit requests, administrators to review and approve/reject requests, and provides listings of all submitted requests.

## Getting Started

### Prerequisites

To run this project locally, you need to have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- (Optional) Docker

### Installation

1. **Clone the repository:**
 https://github.com/spiritomtom/Petko-Dapchev-Credit.git](https://github.com/spiritomtom/Petko-Dapchev-Credit.git

2. **Go to appsettings.json and provide database connection string with the name "Credit":**
   eg. "ConnectionStrings": {
  "Credit": "Server=localhost;Database=CreditsBank;User Id={user};Password={password};TrustServerCertificate=True;"
}

3. **Run the start-api.bat file and the Swagger page of the api should be displayed along with the console:**

## Usage
### Endpoints
1. **Submit a Credit Request**
URL: POST /Credits/apply
Request body
{
    "fullName": "Pesho Peshev",
    "email": "pesho.peshev@example.com",
    "monthlyIncome": 5000,
    "creditAmount": 20000,
    "creditType": "MORTGAGE"
}


2. **Get credit requests**
URL: GET /Credits?status=0&type=0

Response:

{
    "id": "{Guid}",
    "fullName": "Pesho Peshev",
    "email": "pesho.peshev@example.com",
    "monthlyIncome": 5000,
    "creditAmount": 20000,
    "creditType": "MORTGAGE",
    "status": "PendingReview"
}

3. **Approve credit request**
URL: PATCH /Administrator/{userId}/approve/{creditRequestId}



License
This project is licensed under the MIT License - see the LICENSE file for details.
