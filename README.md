# Dashbord_design
# Dashbord_design

## Demo Mode

This copied project is a **design/demo-only** build. It does not authenticate users, read machine-wide configuration, connect to a repository or database, or call any backend service. Dashboard, archive, settings, health, and diagnostic content is generated from deterministic in-process mock data.

Demo safety is enabled by default in `src/LFPortal.Web/appsettings.json` with `Enabled`, `BypassAuthentication`, and `UseMockData` all set to `true`. The application intentionally refuses to start if any of these switches is disabled; there is no production integration path in this copy.

```bash
dotnet build LFPortal.sln --configuration Release
dotnet run --project src/LFPortal.Web/LFPortal.Web.csproj --no-build
```

Open the URL printed by ASP.NET Core. `/` and `/Login` redirect directly to `/Dashboard`; `/Archive`, `/Settings`, and `/health` are entirely local. Settings save only displays a demo confirmation and never writes credentials or machine configuration.
