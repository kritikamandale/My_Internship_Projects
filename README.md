# Internship Projects

This repository contains three projects built during the internship, covering full-stack form/database development, front-end web design, and a biometric authentication system.

| # | Project | Folder | Stack |
|---|---------|--------|-------|
| 1 | KYC Form (with SQL Database) | [`KYC Form/`](KYC%20Form/) | ASP.NET Web Forms (VB.NET) + SQL Server |
| 2 | Kritika's Kitchen — Recipe Website | [`bootstrap-project/`](bootstrap-project/) | HTML5 + Bootstrap 5 |
| 3 | Face Capture — Face Recognition Authentication | [`Face capture/`](Face%20capture/) | React + Node/Express + Flask + MongoDB |

> Note: [`basic KYC form/`](basic%20KYC%20form/) is an earlier static HTML/CSS prototype (no backend or database) that preceded the full ASP.NET + SQL Server implementation in `KYC Form/`. It is kept for reference but is not one of the three core deliverables.

---

## 1. KYC Form (ASP.NET + SQL Server)

A full **Know Your Customer (KYC)** onboarding form for a banking-style use case, built on **ASP.NET Web Forms (VB.NET)** with a **SQL Server** backend, file uploads, OTP-style verification UX, and an admin dashboard for reviewing submissions.

### Key Features
- **Multi-section onboarding form** (`index.aspx` / `index.aspx.vb`) covering:
  - Account type, customer type, preferred branch, application date
  - Contact details (email, mobile, alternate mobile) with **Email OTP** (via the Resend API) and **Mobile/Aadhaar OTP** UX flows
  - Aadhaar details (number, name, DOB) and gender
  - Personal details (name, parents'/spouse name, marital status, nationality, religion, residential status, place/country of birth)
  - Address details (current + permanent address, address type, pincode)
  - Occupation & financial details (employer, nature of business, designation, income range, source of funds)
  - Identity documents (PAN, Driving License) and **document uploads**: Aadhaar, PAN, Passport/DL, address proof, and signature
  - **Edit mode**: loading an existing submission via `?id=` query string and populating the form via a JSON-serialized payload (`populateKycForm`)
- **Admin Dashboard** (`admin-dashboard.aspx` / `admin-dashboard.aspx.vb`):
  - Live stats cards (total submissions, today's submissions)
  - Submissions table populated via an AJAX call to a `GetSubmissionsJSON()` page method
  - Per-record actions exposed as ASP.NET **WebMethods** (called via AJAX): `UpdateStatus`, `DeleteSubmission`, `GetSubmissionDetails`
- **Data access layer** (`App_Code/DatabaseHelper.vb`): a shared helper class wrapping `SqlConnection`/`SqlCommand`/`SqlDataAdapter` for parameterized queries, dropdown binding, and safe value conversion — used by both the form and the dashboard to avoid SQL injection via inline string concatenation.
- **File uploads**: handled server-side in `SubmitForm`, saved under `~/Uploads` with GUID-prefixed filenames to avoid collisions/overwrites.

### Database Schema
Defined in `setup_database.sql`. The schema is normalized into lookup tables joined to one central submissions table:

- Lookup tables (seeded with sample data): `AccountTypes`, `CustomerTypes`, `Branches`, `Genders`, `MaritalStatuses`, `Nationalities`, `ResidentialStatuses`, `AddressTypes`, `OccupationTypes`, `IncomeRanges`, `FundSources`
- Main table: **`KYCSubmissions`** — foreign keys into every lookup table above, plus personal/address/financial/document columns, a `Status` column (default `Pending`), and a `SubmissionDate` timestamp.
- Additional SQL utility scripts:
  - `add_status_column.sql` — migration to add the `Status` column for dashboard review workflow
  - `verify_submission.sql` — query to inspect a submission
  - `show_dropdown_data.sql` — query to inspect lookup/dropdown table contents
  - `diag.sql` — diagnostic/troubleshooting queries

### Project Structure
```
KYC Form/
├── index.aspx / index.aspx.vb        # Main KYC submission form + code-behind
├── admin-dashboard.aspx / .vb        # Admin dashboard + code-behind (WebMethods)
├── App_Code/DatabaseHelper.vb        # Shared SQL Server data access helper
├── Uploads/                          # Uploaded KYC documents (Aadhaar, PAN, signature, etc.)
├── img/logo.jpg                      # Branding asset
├── index.css                         # Form styling
├── Web.config                        # Connection string, request limits, default docs
├── internship_kyc.sln                # Visual Studio solution
└── *.sql                             # Database setup, migration, and diagnostic scripts
```

### Setup & Run
1. **Database**: Run `setup_database.sql` against a local SQL Server / SQL Server Express instance to create the `KYC_DB` database, lookup tables, and the `KYCSubmissions` table.
2. **Connection string**: Update `Web.config` → `connectionStrings` → `KYCConnection` to point at your SQL Server instance (defaults to `Data Source=.\SQLEXPRESS;Initial Catalog=KYC_DB;Integrated Security=True`; a LocalDB alternative is included but commented out).
3. **Run**: Open `internship_kyc.sln` in Visual Studio and run via IIS Express, or deploy to IIS. The default document is `index.aspx`.
4. Visit `index.aspx` to submit a KYC application, and `admin-dashboard.aspx` to review, update status, or delete submissions.

> ⚠️ The Email OTP flow calls the Resend API directly from client-side JavaScript. For production use, this should be moved server-side so the API key isn't exposed in the browser.

---

## 2. Kritika's Kitchen — Recipe Website

A static, responsive recipe/cooking website built with **HTML5** and **Bootstrap 5**, showcasing recipes across Indian, Chinese, Italian, and Mexican cuisines plus a desserts carousel.

### Key Features
- **Sticky scroll-spy navigation** (`navbar-example2` + `data-bs-spy="scroll"`) that highlights the active cuisine section as the user scrolls
- **About/hero section** introducing the site and its theme
- **Bootstrap carousel** for a "Desserts" showcase (Gulab Jamun, Croissant, etc.) with captions
- **Cuisine sections** for Indian, Chinese, Italian, and Mexican dishes, each with imagery and descriptions (e.g. biryani, idli, manchurian, chow mein, pasta, lasagna, tacos, nachos, burritos, samosa, spring rolls)
- Fully built on **Bootstrap 5.3** components (navbar, nav-pills, carousel, grid/cards) loaded via CDN, with custom overrides in `index.css`

### Project Structure
```
bootstrap-project/
├── index.html      # Single-page site: nav, hero, carousel, cuisine sections
├── index.css        # Custom styling on top of Bootstrap
└── img/             # Dish photography (biryani, pizza, tacos, idli, etc.)
```

### Setup & Run
No build step or dependencies required — it's a static site.
```bash
# from bootstrap-project/
# simply open index.html in a browser, or serve it locally:
npx serve .
```

---

## 3. Face Capture — Face Recognition Authentication

A three-tier web application that authenticates users via their **webcam-captured face** instead of (or alongside) a password. It demonstrates a typical microservice split: a React SPA for camera capture/UI, a Node/Express API for user management, and a separate Python/Flask service dedicated to face-recognition logic.

### Architecture
```
client (React + Vite)  ──HTTP──>  server (Express + MongoDB)  ──HTTP──>  face-service (Flask)
   webcam capture              user accounts, signup/login            face embedding & comparison
   signup / login UI           orchestrates calls to face-service       (currently placeholder logic)
```

- **`client/`** — React 18 + Vite SPA
  - `App.jsx` — lays out a two-panel UI: live camera feed (left) and auth panel (right)
  - `components/CameraFeed.jsx` — accesses the webcam via `navigator.mediaDevices.getUserMedia`, renders a live `<video>` feed and a hidden `<canvas>` used to snapshot frames, and hands the video/canvas refs up to the parent for capture
  - `components/AuthPanel.jsx`, `LoginForm.jsx`, `SignupForm.jsx` — sign-up captures **5 face images** per user; login captures images for **live verification** against the stored data
  - Talks to the Express API via `axios`, base URL configured through `VITE_API_URL`

- **`server/`** — Node.js + Express REST API
  - `server.js` — bootstraps Express, connects to MongoDB, mounts `/api/auth`, raises body-size limits to 50 MB to accommodate base64-encoded images
  - `config/db.js` — Mongoose connection to MongoDB (`MONGO_URI`), with connection-event logging and graceful shutdown on `SIGINT`
  - `models/User.js` — Mongoose schema: `email` and `username` (both unique/indexed), `faceData` (array of exactly 5 base64 image strings or embeddings, schema-validated), `createdAt`
  - `routes/auth.js`:
    - `POST /api/auth/signup` — validates input, checks for duplicate email/username, optionally forwards images to the Flask service to extract embeddings (currently falls back to storing raw images if the Flask call is skipped/fails), persists the new `User`
    - `POST /api/auth/login` — looks up the user by email + username, sends the stored face data and newly captured images to the Flask `/verify` endpoint, and grants/denies login based on a match score threshold (`0.6`)

- **`face-service/`** — Python + Flask microservice (`app.py`)
  - `POST /enroll` — decodes base64 images, extracts a face embedding per image, and returns the averaged embedding
  - `POST /verify` — compares newly captured images/embeddings against stored face data and returns a `match` boolean + similarity `score`
  - **Current status: placeholder logic.** `extract_face_encoding_placeholder()` and `compare_faces_placeholder()` generate randomized embeddings/scores — the file already contains detailed inline guidance for swapping in a real implementation using either the `face_recognition` library or `DeepFace` (e.g. `Facenet`, `VGG-Face`)
  - Decodes base64 → PIL images via Pillow/NumPy, with CORS enabled for cross-origin requests from the client/server

### Setup & Run
Each of the three services has its own `.env.example` to copy to `.env`:

```bash
# 1. Face recognition microservice (Flask)
cd "Face capture/face-service"
pip install flask flask-cors pillow numpy python-dotenv
cp .env.example .env
python app.py            # runs on PORT=5001

# 2. API server (Express + MongoDB)
cd "Face capture/server"
npm install
cp .env.example .env     # MONGO_URI, PORT=5000, FLASK_SERVICE_URL
npm run dev               # node --watch server.js

# 3. Client (React + Vite)
cd "Face capture/client"
npm install
cp .env.example .env     # VITE_API_URL=http://localhost:5000
npm run dev
```

Requires a running **MongoDB** instance (local or remote) for the server, reachable via `MONGO_URI`.

> ⚠️ The face-matching logic in `face-service/app.py` is a placeholder (random scores) pending integration of a real face-recognition model — see the TODO comments in that file before relying on this for actual authentication security.

---

## Repository Layout
```
Internship/
├── KYC Form/            # Project 1: ASP.NET + SQL Server KYC onboarding system
├── bootstrap-project/   # Project 2: Bootstrap-based recipe website ("Kritika's Kitchen")
├── Face capture/         # Project 3: React + Express + Flask face-recognition auth demo
└── basic KYC form/      # Earlier static prototype of the KYC form (HTML/CSS only)
```
