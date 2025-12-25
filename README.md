# Miraas (Islamic Inheritance Calculator)

Available at https://miraas.azurewebsites.net

## Purpose

The purpose of this application is to provide a **simple, accurate, and user-friendly Islamic inheritance calculator**
based on the rules of *Ilm al-Fara'id* (Islamic law of inheritance).

The app will allow users to:
- Enter details about a deceased person and surviving heirs
- Automatically calculate each heir’s share based on Islamic jurisprudence rules
- Display results clearly
- Avoid manual formula mistakes
- Serve as an educational and practical tool

The system prioritizes **correctness, simplicity, and trustworthiness** over complex UI or unnecessary features.

## Target Users

- General Muslims seeking inheritance distribution
- Students learning Islamic inheritance
- Imams / teachers
- Legal assistants (non-authoritative reference)
- Families who want a quick distribution overview

The application is not a substitute for fatwa, legal ruling, or Islamic scholar verification.

## Key Product Principles

1. **Simplicity**
   - Minimal forms
   - Clear terminology
   - Straightforward workflow

2. **Accuracy First**
   - Rules reflect authentic inheritance jurisprudence
   - Always favor correctness over user convenience

3. **Transparency**
   - Show calculation logic
   - Display formulas when possible
   - Highlight assumptions

4. **Safety**
   - Warn users when scenarios are invalid or incomplete
   - Encourage validation by a scholar

5. **Maintainability**
   - Business logic in backend
   - Clean separation between UI and calculation engine

# Repository Structure

This repository contains an ASP.NET Core MVC web application intended to run on Azure App Service

## Key files & folders
- src/MiraasWeb — ASP.NET Core MVC Web app
- tests/Miraas.Tests — Unit and integration tests

## Developer workflow (summary)
1. Use Visual Studio and open the solution (Miraas.slnx)

# High-Level Architecture

## Backend
- ASP.NET Core MVC
- Routing, authentication, caching, authorization all handled on server
- All inheritance logic in C# (Domain layer)
- Controllers return Razor pages and JSON endpoints

## Frontend
- Bootstrap for styling
- Only jQuery is available for now; there is no NPM based setup available