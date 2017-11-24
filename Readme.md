# Arragro
The following library is used to provide a common base library for arragro.com.

## Getting Started
Please feel free to pull down the code, then go through the Arragro.Tests folder to see unit tests and use cases for the libraries provided.

The goal is to provide abstractions and force the use of patterns to enable better communication and standards.

If you have any feedback, suggestions, or criticisms please get in touch at mike@arragro.com.

Cheers

## Project Management
The project is managed in visualstudio.com using Team Foundation Service.  Here we use a Build Definition to increment the version, build the solution, run tests, package into nupkg then finally push up to nuget.org.

This is done via build controllers we house in our internal infrastructure and via the build controllers hosted by visualstudio.com.

## Arragro.Core.Common
This library is the foundation of our projects and the core for other libraries under this repository.  It provides the following functionality:

1. Repository Pattern
2. Business Rules
3. Service Base classes
4. Logging Engine (based on log4net, but adaptable)
5. Memory Caching

## Arragro.Core.EntityFrameworkCore
This is a implementation of the IRepository interface defined in Arragro.Core.Common
for Entity Framework 6.

