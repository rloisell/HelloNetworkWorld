#!/usr/bin/env bash
# AI/COMMANDS.sh — HelloNetworkWorld
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# February 2026
#
# Append-only audit trail of every significant shell command run during AI sessions.
# One commented block per session.

# ── Session 1 — 2026-02-27 — Project scaffold ────────────────────────────────

# Initialize git repo (run after scaffold files are created)
# cd /Users/rloisell/Documents/developer/HelloNetworkWorld
# git init
# git remote add origin git@github.com:bcgov-c/HelloNetworkWorld.git
# git add -A
# git commit -m "chore: initialize HelloNetworkWorld from rl-project-template"
# git push -u origin main

# Initialize spec-kitty (run in HelloNetworkWorld directory)
# pip install spec-kitty
# spec-kitty init --here --ai copilot --non-interactive --no-git --force
# spec-kitty validate-tasks --all

# Create .NET solution and projects
# dotnet new sln -n HNW
# dotnet new webapi -n HNW.Api -o src/HNW.Api --no-https
# dotnet new classlib -n HNW.Data -o src/HNW.Data
# dotnet sln HNW.sln add src/HNW.Api/HNW.Api.csproj src/HNW.Data/HNW.Data.csproj
# dotnet add src/HNW.Api reference src/HNW.Data

# Add NuGet packages — HNW.Data
# dotnet add src/HNW.Data package Pomelo.EntityFrameworkCore.MySql --version 8.*
# dotnet add src/HNW.Data package Microsoft.EntityFrameworkCore.Design --version 9.*
# dotnet add src/HNW.Data package Microsoft.EntityFrameworkCore.Relational --version 9.*

# Add NuGet packages — HNW.Api
# dotnet add src/HNW.Api package Quartz --version 3.*
# dotnet add src/HNW.Api package Quartz.AspNetCore --version 3.*
# dotnet add src/HNW.Api package Swashbuckle.AspNetCore --version 6.*
# dotnet add src/HNW.Api package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.*

# Create frontend
# cd src/HNW.WebClient
# npm create vite@latest . -- --template react
# npm install
# npm install @bcgov/design-tokens @tanstack/react-query axios react-router-dom recharts
# cd ../..

# EF Core migrations (run after DB is up)
# dotnet ef migrations add InitialCreate --project src/HNW.Data --startup-project src/HNW.Api
# dotnet ef database update --project src/HNW.Data --startup-project src/HNW.Api

# Build and test
# dotnet build
# dotnet test
# cd src/HNW.WebClient && npm run build
