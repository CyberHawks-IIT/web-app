# web-app — CyberHawks Employee Portal

The **CyberHawks Employee Portal**: an intentionally vulnerable internal web
application for the [CyberHawks cyber range](https://github.com/CyberHawks-IIT/cyber-range).
It's the real, full-featured version of the "internal employee portal" the
range's `web` host serves — an ASP.NET Core 8 MVC site, backed by SQL Server on
`sql1`, wrapped in an (affectionately overblown) in-universe CyberHawks theme.

It is built to be attacked. Every "bug" below is deliberate.

> ⚠️ **This application is deliberately insecure.** It exists only to be
> deployed inside the isolated `cyberhawks.lab` range for training. Do not host
> it on any network you care about, and don't reuse any of its code.

> ## 📋 [Web portal briefing →](https://claude.ai/artifact/QCar6jLj7s2nAMHdkUt2H6)
> **See exactly what you're attacking.** The target host, the starter member
> account you get on day one, and the full list of findings to hunt — across
> authentication, access control, injection, client-side, file/data exposure,
> and business logic. This is also the handout for whoever attacks the portal.

## What's in it

A full employee portal — login, member directory and profiles, news with
comments, a document store, a rewards store, an admin area, search, and a small
JSON API — carrying a deliberate spread of app-layer vulnerabilities, including:

- **Broken access control** — cross-account data access (IDOR) and
  straight-to-admin authorization gaps
- **Cross-site scripting** — both reflected and stored
- **SQL injection** — blind, with a 500 on error
- **Unrestricted file upload** — HTML upload / content-type bypass
- **Sensitive file exposure** — a leftover `Web.config.bak` and `db_backup.sql`
  served straight out of `wwwroot`, leaking the `svc-web` SQL connection string
- **Weak crypto / seeded secrets** — SHA-1 password hashes, `admin` / `abc123`,
  and seeded PII

See [SITE_PLAN.md](SITE_PLAN.md) for the full design and the authoritative
vulnerability catalog.

## Stack

- ASP.NET Core 8 MVC (`net8.0`), EF Core 8 (SQL Server)
- The database is created and seeded automatically on first run
  (`DbSeeder` → `EnsureCreated()`), so there's no migration step
- Runs under IIS (in-process, via the ASP.NET Core Hosting Bundle) on the range;
  runs on Kestrel for local development

## Run it locally

Needs the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). By
default it points at `.\SQLEXPRESS` (see `appsettings.json`); override the
`Portal` connection string to point elsewhere.

```bash
dotnet run --project src/CyberHawksPortal/CyberHawksPortal.csproj --urls http://localhost:5080
```

Then browse to <http://localhost:5080>. The database is created and seeded on
startup. (The repo's `.claude/launch.json` runs exactly this.)

## Deploying into the range

You don't deploy this by hand. The range deploys it for you, as an **optional
add-on**, when you flip one toggle:

```bash
# in the cyber-range repo
ansible-playbook -i inventory/hosts.yml playbooks/vulnerable-range.yml -e range_full_portal=true
```

With the toggle on, cyber-range clones this repo on the control node, runs
`dotnet publish`, deploys the output to `web` under IIS, and points it at `sql1`
with the `svc-web` login. It replaces the lightweight placeholder portal the
core range otherwise ships. See the cyber-range
[web-portal setup guide](https://github.com/CyberHawks-IIT/cyber-range/blob/master/docs/setup/web-portal.md)
for details and prerequisites.

## Part of a bigger project

| Repo | Layer |
|---|---|
| [cyber-range](https://github.com/CyberHawks-IIT/cyber-range) | The vulnerable range (deploys this as an optional add-on) |
| **web-app** *(this repo)* | The full CyberHawks Employee Portal web application |
| [defense-tooling](https://github.com/CyberHawks-IIT/defense-tooling) | Splunk and Zeek monitoring stack |
| [splunk-detections](https://github.com/CyberHawks-IIT/splunk-detections) | Detection content for that Splunk instance |
| [vm-templates](https://github.com/CyberHawks-IIT/vm-templates) | Base VM and container builds |

## Repository layout

```
web-app/
  CyberHawksPortal.sln
  SITE_PLAN.md                 # full site design + vulnerability catalog
  src/CyberHawksPortal/        # the ASP.NET Core 8 MVC application
    Controllers/ Models/ Views/ Data/ Services/ wwwroot/
  pictures/                    # source photos used for the site theme
```
