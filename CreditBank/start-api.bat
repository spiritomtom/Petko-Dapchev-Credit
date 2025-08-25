@echo off
echo Starting the Credit Approval API...

REM Restore the .NET dependencies
echo Restoring dependencies...
dotnet restore

REM Build the project
echo Building the project...
dotnet build

REM Run the application
echo Running the application...
start "" "http://localhost:5048/swagger/index.html"
dotnet run
echo Application started successfully!

PAUSE