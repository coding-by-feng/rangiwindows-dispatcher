# Rangi Windows Dispatcher

This repository now hosts the .NET port of the legacy Java `kiwi-tools` module that is reused across Rangi Windows microservices.

## Projects

- `src/KiwiTools` – shared utilities (result objects, exception hierarchy, Snowflake IDs, password hashing, diagnostics helpers).
- `tests/KiwiTools.Tests` – xUnit tests covering the critical helpers.

Detailed migration notes and usage examples are available in [docs/kiwi-tools.md](docs/kiwi-tools.md).
