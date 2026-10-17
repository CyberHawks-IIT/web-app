# CyberHawks Member Portal — Site Plan

Intentionally-vulnerable internal portal for Illinois Tech's **CyberHawks** cybersecurity
club, themed around founder **David Arnold**, to replace the existing web app on the cyber
range's `web` host (`10.0.2.5`). Practiced by David himself. This is the build spec.

> **STATUS (2026-10-17): BUILT.** ASP.NET Core 8 MVC app under `src/CyberHawksPortal/`. All
> 31 vulnerabilities in §5 plus the delete-confirm XSS (§5a) are implemented and the headline
> ones verified end-to-end. Runs locally via `.claude/launch.json` (`cyberhawks`, :5080)
> against SQL Server Express. Each vuln is commented in code with its §5 number. Remaining:
> deploy to `web` (IIS + ASP.NET Core Hosting Bundle).

> Context lives in session memory: theme (`web-app-theme-david-arnold`), lore/easter eggs
> (`web-app-cyberhawks-lore`), photos (`web-app-photos`), the range-dictated findings
> (`web-app-vuln-spec`), the mandatory four (`web-app-app-vulns`), and the full catalog
> (`web-app-vuln-catalog`). Authoritative range docs: `C:\Tools\cyber-range\CLAUDE.md`.

---

## 1. Stack & environment

- **ASP.NET Core MVC (.NET 8)**, hosted on **IIS** (Windows Server 2022, `web` / 10.0.2.5)
  via the ASP.NET Core Hosting Bundle. (Chosen over .NET Framework MVC 5 because the control
  host has no SDK/MSBuild/VS/IIS Express — Core builds, runs via Kestrel, and previews
  locally after a winget SDK install.) The `Web.config.bak` leak is preserved: Core apps on
  IIS ship a real **`web.config`**, and the app reads the `svc-web` connection string from it
  (via `System.Configuration.ConfigurationManager`), so the planted `.bak` copy is genuine.
- **Data:** SQL Server on **sql1** (10.0.2.6). The app connects as the SQL login **`svc-web`**
  (SQL auth, connection string in `Web.config`). Separately, the DB is readable by **all
  domain users** (range/SQL config) — so any domain user can read the same tables directly.
- **Served over HTTP** (port 80), no TLS → cleartext credential transmission is natural.
- **Razor views**; **Bootstrap**-style CSS for a believable corporate/club intranet look
  (IIT scarlet/grey nod, padlock-with-hawk logo motif, triceratops favicon easter egg).
- Deployment: Ansible role or manual publish to IIS on `web` — decide at build time
  (see range repo convention). Photos served from `/Content/img/` (source:
  `C:\Tools\web-app\pictures\`).

### Mapping the plan onto Core
- **Mass assignment** → model-binding over-posting (bind the EF entity directly, no view model).
- **CSRF** → omit `[ValidateAntiForgeryToken]` / global filter on state-changing POSTs.
- **Admin area** → `/Admin` controllers with intentionally-missing/role-less `[Authorize]`.
- **JSON findings** → API controllers (excessive data exposure, CORS, BOLA).
- **Insecure deserialization** → Json.NET `TypeNameHandling.All` on a signed-but-weak
  preferences cookie (classic .NET gadget RCE; no BinaryFormatter needed).
- **Config leak** → ship `web.config` with `<connectionStrings>` the app actually reads;
  plant `Web.config.bak` beside it.

---

## 2. Audiences & auth model

Three tiers, so findings chain realistically but each is individually reachable:

1. **Unauthenticated** (network access only) — home, login, password reset, public
   News/Competitions/Sponsors, static/sensitive files.
2. **Member** (self-registered or seeded) — dashboard, roster, own profile, documents,
   store/events, preferences.
3. **Officer/Admin** (`/Admin` area) — user management, import tools, range/diagnostic
   tools, sponsor management.

**Starter access:** open **self-registration** (weak, client-side-only policy) gives any
student an authenticated foothold; plus seeded accounts (`admin`/`abc123`, member accounts
for the real roster, a couple with weak web passwords). Privilege escalation (mass
assignment / forgeable session / client-controlled auth / missing function-level checks)
takes a member to admin. Many findings are fully unauthenticated.

**Session:** custom cookie **`CHAUTH`** = Base64(`{userId}|{role}|{issuedUnix}`) with a weak,
guessable keyed hash (or none) → predictable & forgeable (§5 #6).

---

## 3. Sitemap

### Public
- **`/` Home** — hero **feature1** (David + John building the server rack), club tagline,
  "Est. 2018 · Founded by David Arnold", championship badge *"First team ever to break 5,000
  points"*, latest-news teaser, photo strip.
- **`/Account/Login`** — cleartext creds (HTTP), **username enumeration** (distinct
  "no such user" vs "wrong password"), weak admin cred `abc123`, issues the forgeable
  `CHAUTH`, **open redirect** via `returnUrl`. Cameo photo: **20231103** ("Log In" laptop).
- **`/Account/Register`** — weak password policy (client-side only); **mass assignment**
  (over-post `Role`/`IsAdmin`).
- **`/Account/ForgotPassword` + `/ResetPassword`** — **insecure password reset**
  (predictable token = Base64(userId+date), no expiry/rate-limit, delivered in URL) +
  username enumeration.
- **`/News` + `/News/{id}`** — club blog (author **Jamie Crosby**). **Comments** →
  **blind/second-order SQLi** (payload stored, surfaced by a *second* query on an admin
  "recent comments" view; **not reflected**; malformed query → **HTTP 500**) and
  **stored XSS**. Optional classic in-band SQLi on `?category=`/`?sort=`.
- **`/Competitions`** — CCDC / CyberForce / Hivestorm / NCAE / CPTC teams & results, the
  5,000-point story. Gallery: **CyberHawks.jpg, IIT CyberHawks, image.jpg, 20191116,
  Resized_IMG_2547 (scoreboard w/ "David Arnold – Team18"), 20230422, IMG20231104194438,
  IMG_3445, dwe**.
- **`/Sponsors`** — industry sponsors (coordinator **Abdul Hadi Khan**); click-through
  **open redirect** (`/go?url=`).
- **`/About`** — founder story + quotes; photos **arnold_stryker_2024, david_arnold_-_450x730,
  main_new, feature2 (triceratops)**. Faculty advisors **Jafar Saniie**, **Won-Jae Yi**;
  ECE 222 mention; "CyberHawks Cyber Challenge".
- **Static / sensitive files** — `/Web.config.bak` (svc-web connection string, #R4),
  plus planted `/backup/db_backup.sql`, `/uploads/`, `/robots.txt` disclosing paths →
  **unauthenticated access to sensitive files**. `/Scripts/portal.js` with **cleartext
  credentials in client-side code** (hardcoded internal API key / basic-auth, distinct from
  svc-web and abc123).
- **404 page** — hidden **david-jamesbond** (Skyfall) easter egg.

### Member (authenticated)
- **`/Dashboard`** — member landing.
- **`/Members` + `/Members/{id}`** — roster (real people). **IDOR** (`/Members/{id}` /
  `/api/members/{id}` exposes others' private fields). Avatars: David's headshots;
  `redefiningreality` = John Ford's handle, `darnold` = David.
- **`/Profile/Edit`** — **mass assignment** (over-post `Role`/`IsAdmin`/`MembershipLevel`);
  **stored XSS** in bio/display name; **CSRF** (no anti-forgery token on save); the
  **delete-confirmation stored XSS** (deleting a saved item pops a modal that injects the
  stored name unescaped).
- **`/Search`** — **reflected XSS** (query echoed) + **DOM-based XSS** (client JS reads
  `location.hash`/query → `innerHTML`).
- **`/Documents`** — meeting minutes, seminar slides (his real talks), competition writeups,
  "projects" named after his papers (ICS data historian, ultrasonic comms, IoT botnet,
  remote patient health monitoring). **Path traversal** (`?file=../../Web.config`);
  **file upload** blocking `.html` client-side only → upload HTML → stored XSS from
  `/uploads/`.
- **`/Store` or `/Events`** (treasurer **Mohithaa Ekambaram** context) — **business logic
  flaw** (negative qty / price tamper / reuse one-time code) and **race condition** (TOCTOU
  on a one-time welcome-coupon / limited swag / event-seat / reimbursement-budget overrun).
- **`/Preferences`** — view settings stored as a **Json.NET (`TypeNameHandling.All`)**
  serialized Base64 cookie → **insecure deserialization** (ysoserial.net `.NET` gadget → RCE).
- **API** `/api/*` (Web API 2) — **excessive data exposure** (`/api/members/{id}` returns
  hash/SSN/stipend); **CORS misconfiguration** (reflects `Origin` + `Allow-Credentials:true`);
  **BOLA/IDOR** mirrors the MVC IDOR.

### Admin / Officer — `/Admin` area (President **Lucas Ferguson**, VP **Benjamin De Pater**)
- **Unauthenticated access to the admin interface** (`/Admin` has no `[Authorize]`) **and**
  **missing function-level access control** (individual `/Admin/*` POST actions don't
  re-check role) — two distinct findings.
- **`/Admin/Users`** — list users, view hashes, reset passwords. **Client-controlled
  authorization parameter** (an approval action trusts a hidden `role`/`accessLevel` field
  or a separate `role` cookie).
- **`/Admin/Tools/Ping`** — "check a CyberForce/ICS device" → **command injection**
  (`cmd /c ping <host>`).
- **`/Admin/Import`** — import roster / competition results from **XML** → **XXE**
  (DtdProcessing + XmlResolver enabled → local file read / SSRF).
- **`/Admin/Sponsors`** — "fetch sponsor logo / preview site by URL" → **SSRF**
  (reach `169.254.169.254`, internal `10.0.2.x`).
- **`/Admin/Files`** — file browser → path traversal / sensitive-file disclosure.

---

## 4. Data model (SQL Server on sql1)

- **Users**(UserId PK, Username, PasswordHash *(unsalted MD5/SHA1 → weak password storage)*,
  Email, Role, IsAdmin, MembershipLevel, FirstName, LastName, **SSN**, Phone, Address,
  **StipendAmount**, Bio, AvatarPath, CreatedAt) — SSN/stipend **stored unencrypted**; seeded
  `admin`/`abc123`; real roster; a few weak-password members.
- **NewsPosts**(PostId, Title, Body, AuthorId, PublishedAt)
- **Comments**(CommentId, PostId, AuthorName, Body, CreatedAt) — blind SQLi + stored XSS
- **Competitions**(Id, Name, Year, Result, Notes)
- **Documents**(DocId, Title, FilePath, UploadedBy, Visibility)
- **Sponsors**(Id, Name, LogoUrl, Website)
- **StoreItems / Coupons / Reimbursements** — business-logic & race-condition targets
- **PasswordResets**(Token, UserId, Expiry) — predictable tokens

App login (#R1) validates against **Users**; admin hash cracks to `abc123` (#R2); the whole
DB is readable by all domain users (#R3).

---

## 5. Vulnerability coverage matrix

| # | Finding | Location |
|---|---|---|
| R1 | Login validated vs sql1 DB | `/Account/Login` → Users |
| R2 | App-admin `abc123` (hashed, crackable) | Users.admin |
| R3 | DB readable by all domain users | sql1 config + schema |
| R4 | `Web.config.bak` svc-web leak | `/Web.config.bak` |
| 1 | Unauthenticated username enumeration | Login + ForgotPassword |
| 2 | Weak password: login portal | `abc123` + weak member creds |
| 3 | Weak password policy | Register/Reset (client-side only) |
| 4 | Insecure password reset | predictable token, no expiry |
| 5 | Cleartext transmission of creds | HTTP login |
| 6 | Predictable/forgeable session token | `CHAUTH` cookie |
| 7 | Missing function-level access control | `/Admin/*` actions |
| 8 | IDOR | `/Members/{id}`, `/api/members/{id}` |
| 9 | Client-controlled authorization param | `/Admin/Users` approval field/cookie |
| 10 | Mass assignment | Register + Profile/Edit over-post |
| 11 | Excessive data exposure (API) | `/api/members/{id}` |
| 12 | Unauth access: admin interface | `/Admin` (no `[Authorize]`) |
| 13 | Business logic flaw | Store/Events (qty/price/one-time) |
| 14 | Race condition | coupon/seat/budget TOCTOU |
| 15 | SQL injection | Comments (blind) + optional classic |
| 16 | Command injection | `/Admin/Tools/Ping` |
| 17 | Insecure deserialization | `/Preferences` Json.NET TypeNameHandling cookie |
| 18 | XXE | `/Admin/Import` (XML) |
| 19 | SSRF | `/Admin/Sponsors` fetch-logo |
| 20 | File upload (HTML bypass) | `/Documents` upload |
| 21 | Path traversal | `/Documents/Download`, `/Admin/Files` |
| 22 | Reflected XSS | `/Search` |
| 23 | Stored XSS | comments + profile fields |
| 24 | DOM-based XSS | `/Search` client JS |
| 25 | CSRF | Profile/email change (no token) |
| 26 | CORS misconfiguration | `/api/*` reflects Origin |
| 27 | Open redirect | `returnUrl`, `/go?url=` |
| 28 | Unauth access: sensitive files | Web.config.bak, backups, uploads |
| 29 | Cleartext creds in client-side code | `/Scripts/portal.js` |
| 30 | Weak password storage | unsalted MD5/SHA1 |
| 31 | Sensitive data stored unencrypted | Users.SSN / StipendAmount |
| M4 | Delete-confirm stored XSS (echoed name) | Profile/items delete modal |

**Dropped (per decision, ASP.NET-contrived):** SSTI, Server-Side Code Injection, File
Inclusion.

### 5a. Mandatory four — honored exactly as specified (hard acceptance criteria)
These come from the user's "Plan to FOR SURE include each of these" message and must match its
wording precisely — each genuinely exploitable end-to-end:
1. **AuthZ — cross-account (horizontal):** a user can reach another account's data →
   `/Members/{id}` & `/api/members/{id}` IDOR (#8). **AuthZ — admin functionality (vertical):**
   a non-admin reaches admin functionality → `/Admin` with no server-side role check (#12 + #7).
2. **Reflected XSS in a search bar:** `/Search` reflects the query (#22). **Stored XSS incl.
   the delete-confirmation variant:** deleting a saved item/profile pops a confirmation dialog
   that injects the stored (attacker-controlled) **name** unescaped (M4), plus stored XSS in
   comments/profile (#23).
3. **File upload — HTML blocked client-side only:** JS/accept rejects `.html`, but the server
   applies **no** equivalent check, so HTML uploads succeed and are served (#20).
4. **SQLi on blog/comments — blind/second-order:** the injected payload is **not reflected** in
   the response (it's surfaced by a *separate* query elsewhere, e.g. an admin "recent comments"
   view), and a **malformed query returns HTTP 500** (#15). On News comments.

**Design hygiene (mirrors the range's rules):** each finding is independently reachable from
network / self-registered member / crackable admin; targets don't collapse onto one another;
planted creds (`abc123` in DB, `svc-web` in Web.config.bak, the portal.js API key) are three
distinct secrets at three distinct spots.

---

## 6. Photo placement

| Photo | Placement |
|---|---|
| **feature1** | Home hero (MUST) |
| **feature2** | About "life at CyberHawks" + triceratops favicon/mascot (MUST) |
| **david-jamesbond** | 404 page + hidden on David's profile (MUST) |
| david_arnold_-_450x730 | David's profile avatar / About |
| david_arnold_450x730 | About alt / alumni card |
| arnold_stryker_2024 | About — Stryker award / founder |
| main_new | About — PhD commencement w/ Saniie |
| Resized_IMG_2547 | Competitions — Midwest scoreboard ("David Arnold – Team18") |
| CyberHawks.jpg | Competitions — 2023 NCAE @ USF |
| IIT CyberHawks 1280x850 | Competitions — CyberForce table |
| image.jpg | Competitions — CyberForce team + placard |
| 20191116 | Competitions — 2019 history (John Ford) |
| 20230422 | Competitions — nationals travel candid |
| IMG20231104194438 | Competitions — ballroom crowd |
| IMG_3445 | Competitions — practice/lab |
| dwe_190917 | Documents/Seminars — hands-on lab |

---

## 7. Easter eggs

- Footer: "CyberHawks — Est. 2018 · Founded by David Arnold".
- "First team ever to break 5,000 points" badge; Midwest scoreboard screenshot (real).
- Triceratops mascot/favicon (from feature2); James Bond / Skyfall hidden photo.
- Handles: `redefiningreality` (John Ford), `darnold` (David).
- "Projects" = his real papers; "Seminars" = his real talks (Nmap/Wireshark/JtR/Splunk/Burp).
- Faculty: Jafar Saniie, Won-Jae Yi; ECE 222; "CyberHawks Cyber Challenge".

---

## 8. Open items (confirm at build)
1. Starter access: open self-registration **plus** seeded accounts (recommended) — OK?
2. Add one classic in-band SQLi alongside the mandatory blind one? (recommended for variety)
3. `svc-web` password + DB name — TBD per range provisioning.
4. Deployment path: Ansible role vs manual IIS publish.
