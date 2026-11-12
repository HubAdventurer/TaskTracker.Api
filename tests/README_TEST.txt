# Task Tracker API tests

These three xUnit tests use RestSharp to send real HTTP requests to the local API.

- A PATCH that sends only `isCompleted: false` saves that value and preserves the other fields.
- A PATCH with a 101-character title and a valid completion change returns 400 and saves neither change.
- A GET with an unsupported `sortBy` returns 400 with the expected error message.

## Run locally

Use the .NET 10 SDK and EF Core 10 CLI (`dotnet ef`). Run the commands from the repository root. The example below uses Bash.

In the first terminal, create and migrate a fresh SQLite test database, then start the API:

---
dotnet ef database update
dotnet run --project TaskTracker.Api.csproj --launch-profile http
---

Wait for the API to listen on `http://localhost:5118`. Keep it running and open a second terminal:

---
dotnet test tests/TaskTracker.Api.Tests/TaskTracker.Api.Tests.csproj
---

The two PATCH tests create their own tasks and use the IDs returned by the API. Each reads the task back with a separate GET to verify the saved state. Stop the API when finished.
