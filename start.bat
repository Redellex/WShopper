@echo off
REM Uruchomienie backendu

echo Uruchamiam backend...
cd /d ".\WShopper"
dotnet run --urls "http://localhost:5195"