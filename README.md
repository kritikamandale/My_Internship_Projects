# Internship Projects

This repository brings together three internship projects that span database-backed form work, responsive front-end design, and face-based authentication.

| # | Project | Folder | Stack |
|---|---------|--------|-------|
| 1 | KYC Form with SQL Database | [`KYC Form/`](KYC%20Form/) | ASP.NET Web Forms (VB.NET) + SQL Server |
| 2 | Kritika's Kitchen Recipe Site | [`bootstrap-project/`](bootstrap-project/) | HTML5 + Bootstrap 5 |
| 3 | Face Capture Authentication Demo | [`Face capture/`](Face%20capture/) | React + Node/Express + Flask + MongoDB |

> Note: [`basic KYC form/`](basic%20KYC%20form/) is the earlier static HTML/CSS prototype that came before the full ASP.NET + SQL Server build in `KYC Form/`. It remains in the repo for reference, but it is not one of the three main deliverables.

---

## 1. KYC Form (ASP.NET + SQL Server)

This is a full **Know Your Customer (KYC)** onboarding flow for a banking-style scenario. It is built with **ASP.NET Web Forms (VB.NET)**, backed by **SQL Server**, and includes file uploads, OTP-style verification UX, and an admin dashboard for reviewing submissions.

### Key Features
- **Multi-section onboarding form** (`index.aspx` / `index.aspx.vb`) that collects:
  - Account type, customer type, preferred branch, and application date
  - Contact information such as email, mobile, and alternate mobile, along with **Email OTP** (through the Resend API) and **Mobile/Aadhaar OTP** UX flows
  - Aadhaar number, name, date of birth, and gender details
  - Personal information including name, parents'/spouse name, marital status, nationality, religion, residential status, and place/country of birth
  - Current and permanent address information, address type, and pincode
  - Occupation and financial details like employer, nature of business, designation, income range, and source of funds
  - Identity fields such as PAN and Driving License, plus **document uploads** for Aadhaar, PAN, Passport/DL, address proof, and signature
  - **Edit mode** support, which loads an existing submission via `?id=` and repopulates the form through a JSON payload (`populateKycForm`)
- **Admin Dashboard** (`admin-dashboard.aspx` / `admin-dashboard.aspx.vb`) with:
  - Live summary cards for total submissions and today's submissions
  - A submissions table filled through an AJAX call to the `GetSubmissionsJSON()` page method
  - Record-level actions exposed as ASP.NET **WebMethods** and called via AJAX: `UpdateStatus`, `DeleteSubmission`, and `GetSubmissionDetails`
- **Data access layer** (`App_Code/DatabaseHelper.vb`) that centralizes `SqlConnection`/`SqlCommand`/`SqlDataAdapter` usage for parameterized queries, dropdown binding, and safe value conversion, keeping both the form and dashboard away from inline SQL concatenation.
- **File uploads** handled on the server in `SubmitForm`, stored in `~/Uploads` with GUID-prefixed filenames to avoid name collisions.

### Database Schema
The database design lives in `setup_database.sql`. It uses lookup tables linked to a single submissions table so the data stays organized:

- Lookup tables seeded with sample records: `AccountTypes`, `CustomerTypes`, `Branches`, `Genders`, `MaritalStatuses`, `Nationalities`, `ResidentialStatuses`, `AddressTypes`, `OccupationTypes`, `IncomeRanges`, and `FundSources`
- Main table: **`KYCSubmissions`**, which stores foreign keys into the lookup tables along with the personal, address, financial, and document fields, plus a `Status` column that defaults to `Pending` and a `SubmissionDate` timestamp
- Supporting SQL scripts:
  - `add_status_column.sql` for adding the dashboard status field
  - `verify_submission.sql` for checking a specific submission
  - `show_dropdown_data.sql` for reviewing dropdown table data
  - `diag.sql` for diagnostics and troubleshooting

### Project Structure
```
KYC Form/
├── index.aspx / index.aspx.vb        # Main KYC submission form and code-behind
├── admin-dashboard.aspx / .vb        # Admin dashboard and code-behind (WebMethods)
├── App_Code/DatabaseHelper.vb        # Shared SQL Server data access helper
├── Uploads/                          # Uploaded KYC documents (Aadhaar, PAN, signature, etc.)
├── img/logo.jpg                      # Branding asset
├── index.css                         # Form styling
├── Web.config                        # Connection string, request limits, default docs
├── internship_kyc.sln                # Visual Studio solution
└── *.sql                             # Database setup, migration, and diagnostic scripts
```

### Setup & Run
1. **Database**: Execute `setup_database.sql` against a local SQL Server or SQL Server Express instance to create the `KYC_DB` database, lookup tables, and the `KYCSubmissions` table.
2. **Connection string**: Update `Web.config` → `connectionStrings` → `KYCConnection` so it points to your SQL Server instance. The default value is `Data Source=.\SQLEXPRESS;Initial Catalog=KYC_DB;Integrated Security=True`, and a commented LocalDB option is also included.
3. **Run**: Open `internship_kyc.sln` in Visual Studio and start it through IIS Express, or deploy it to IIS. The default document is `index.aspx`.
4. Open `index.aspx` to submit a KYC application, then use `admin-dashboard.aspx` to review records, change status, or delete submissions.

> ⚠️ The Email OTP flow currently calls the Resend API from client-side JavaScript. For production use, that logic should move server-side so the API key is not exposed in the browser.

---

## 2. Kritika's Kitchen — Recipe Website

This is a static, responsive recipe and cooking site built with **HTML5** and **Bootstrap 5**. It showcases Indian, Chinese, Italian, and Mexican recipes, along with a desserts carousel.

### Key Features
- **Sticky scroll-spy navigation** (`navbar-example2` + `data-bs-spy="scroll"`) that keeps the active cuisine section highlighted while scrolling
- **About/hero section** that introduces the website and its theme
- **Bootstrap carousel** that presents a desserts showcase such as Gulab Jamun and Croissant with captions
- **Cuisine sections** for Indian, Chinese, Italian, and Mexican dishes, each paired with images and short descriptions such as biryani, idli, manchurian, chow mein, pasta, lasagna, tacos, nachos, burritos, samosa, and spring rolls
- Built entirely with **Bootstrap 5.3** components loaded from CDN, with custom styling in `index.css`

### Project Structure
```
bootstrap-project/
├── index.html      # Single-page site: nav, hero, carousel, cuisine sections
├── index.css        # Custom styling on top of Bootstrap
└── img/             # Dish photography (biryani, pizza, tacos, idli, etc.)
```

### Setup & Run
No build step or extra dependencies are needed because this is a static site.
```bash
# from bootstrap-project/
# simply open index.html in a browser, or serve it locally:
npx serve .
```

---

## 3. Face Capture — Face Recognition Authentication

This three-tier web application authenticates users with a **webcam-captured face** instead of, or in addition to, a password. It follows a common microservice layout: a React SPA for camera capture and UI, a Node/Express API for user management, and a separate Python/Flask service for face-recognition logic.

### Architecture
```
client (React + Vite)  ──HTTP──>  server (Express + MongoDB)  ──HTTP──>  face-service (Flask)
   webcam capture              user accounts, signup/login            face embedding & comparison
   signup / login UI           orchestrates calls to face-service       (currently placeholder logic)
```

- **`client/`** — React 18 + Vite SPA
  - `App.jsx` — builds a two-panel layout with the live camera feed on the left and the auth panel on the right
  - `components/CameraFeed.jsx` — uses `navigator.mediaDevices.getUserMedia` to access the webcam, renders a live `<video>` element and a hidden `<canvas>` for snapshots, and passes the refs back up for capture
  - `components/AuthPanel.jsx`, `LoginForm.jsx`, `SignupForm.jsx` — signup records **5 face images** per user, while login captures images for **live verification** against stored data
  - Communicates with the Express API through `axios`, with the base URL set by `VITE_API_URL`

- **`server/`** — Node.js + Express REST API
  - `server.js` — starts Express, connects to MongoDB, mounts `/api/auth`, and raises the body-size limit to 50 MB so base64-encoded images can be handled
  - `config/db.js` — manages the Mongoose connection to MongoDB (`MONGO_URI`) with connection-event logging and graceful shutdown on `SIGINT`
  - `models/User.js` — defines the Mongoose schema with unique/indexed `email` and `username` fields, a `faceData` array containing exactly 5 base64 image strings or embeddings, and `createdAt`
  - `routes/auth.js`:
    - `POST /api/auth/signup` — validates the request, checks for duplicate email or username, optionally sends images to the Flask service for embedding extraction, and falls back to storing raw images if that call is skipped or fails before saving the new `User`
    - `POST /api/auth/login` — finds the user by email and username, sends the stored face data and newly captured images to the Flask `/verify` endpoint, and allows or rejects login based on a match-score threshold of `0.6`

- **`face-service/`** — Python + Flask microservice (`app.py`)
  - `POST /enroll` — decodes base64 images, extracts one face embedding per image, and returns the averaged embedding
  - `POST /verify` — compares newly captured images or embeddings with stored face data and returns a `match` boolean plus a similarity `score`
  - **Current status: placeholder logic.** `extract_face_encoding_placeholder()` and `compare_faces_placeholder()` generate random embeddings and scores. The file already includes inline guidance for replacing that logic with a real implementation based on either `face_recognition` or `DeepFace` (for example `Facenet` or `VGG-Face`)
  - Converts base64 data to PIL images through Pillow and NumPy, with CORS enabled for cross-origin requests from the client and server

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

The server requires a running **MongoDB** instance, either local or remote, reachable through `MONGO_URI`.

> ⚠️ The face-matching logic in `face-service/app.py` is still a placeholder that returns random scores until a real face-recognition model is integrated. Check the TODO comments in that file before relying on it for actual authentication security.

---

## Repository Layout
```
Internship/
├── KYC Form/            # Project 1: ASP.NET + SQL Server KYC onboarding system
├── bootstrap-project/   # Project 2: Bootstrap-based recipe website ("Kritika's Kitchen")
├── Face capture/         # Project 3: React + Express + Flask face-recognition auth demo
└── basic KYC form/      # Earlier static prototype of the KYC form (HTML/CSS only)
```
