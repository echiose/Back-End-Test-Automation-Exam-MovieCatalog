# Movie Catalog API Test Automation

This project was developed as part of the SoftUni Back-End Test Automation course and is being used for API testing as part of the exam assignment. The application itself was created by SoftUni for training and assessment purposes, and this repository focuses on automated backend testing of its Movie Catalog REST API.

This repository contains an NUnit-based backend test automation project for validating a Movie Catalog REST API. The tests cover creating, editing, retrieving, and deleting movie records, as well as validation scenarios for invalid requests.

## Project Overview

The project is built with:

- .NET 8
- NUnit
- RestSharp
- JSON serialization for API payloads

The test suite interacts with the deployed API at:

- http://144.91.123.158:5000

It authenticates using a JWT token and exercises the movie catalog endpoints for both success and failure scenarios.

## Solution Structure

- `MovieCatalogExam.sln` – Visual Studio solution file
- `MovieCatalogExam/` – main test project
  - `MovieCatalogExam.csproj` – project configuration and dependencies
  - `UnitTest1.cs` – automated API tests
  - `Models/` – DTO models used for request/response serialization

## Included Test Coverage

The project verifies the following behaviors:

1. Create movie with valid required data
2. Edit an existing movie
3. Get all movies
4. Delete a created movie
5. Create movie with missing required data -> returns 400 Bad Request
6. Edit a non-existing movie -> returns 400 Bad Request
7. Delete a non-existing movie -> returns 400 Bad Request

## Authentication

The tests use a static JWT token defined in the project, and a fallback login flow is also available if the token is empty:

- Email: `examqa26@softuni.com`
- Password: `examqa2026`

This allows the API client to authenticate before sending requests.

## Prerequisites

Before running the tests, make sure you have:

- .NET SDK 8.0 or later installed
- A working internet connection to reach the test API
- A supported IDE such as Visual Studio or VS Code

## Run the Tests

From the project root, run:

```bash
dotnet test
```

This will execute the NUnit test suite and report the pass/fail results for each API scenario.

## Notes

- The project is focused on backend/API automation rather than UI automation.
- Tests rely on real API responses and validate the actual application behavior.
- The tests include both positive and negative scenarios to ensure request validation and error handling are correctly implemented.

## Author / Context

This project was created as part of the SoftUni Back-End Test Automation course exam exercise. The API application itself is provided by SoftUni, and the automated tests in this repository are used to validate its behavior during API testing.
