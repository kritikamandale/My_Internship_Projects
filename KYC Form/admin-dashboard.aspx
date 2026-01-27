<%@ Page Language="VB" AutoEventWireup="true" CodeFile="admin-dashboard.aspx.vb" Inherits="admin_dashboard"
    ClientIDMode="Static" %>
    <!DOCTYPE html>
    <html lang="en">

    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Admin Dashboard - KYC Submissions</title>
        <link rel="stylesheet" href="index.css">
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
        <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet">
        <style>
            .stats-card {
                background: white;
                border-radius: 10px;
                padding: 20px;
                margin-bottom: 20px;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                transition: transform 0.3s ease;
                border: 1px solid #ddd;
            }

            .stats-card:hover {
                transform: translateY(-5px);
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
            }

            .stats-number {
                font-size: 2.5rem;
                font-weight: bold;
                color: #7d98e9;
            }

            .stats-label {
                color: #6c757d;
                font-size: 0.9rem;
                text-transform: uppercase;
                font-weight: bold;
            }

            .table-container {
                background: white;
                border-radius: 5px;
                padding: 25px;
                border: 1px solid #ddd;
            }

            .action-btn {
                padding: 5px 10px;
                margin: 0 2px;
                border: none;
                border-radius: 5px;
                cursor: pointer;
                transition: all 0.3s ease;
            }

            .view-btn {
                background-color: #17a2b8;
                color: white;
            }

            .view-btn:hover {
                background-color: #138496;
            }

            .delete-btn {
                background-color: #dc3545;
                color: white;
            }

            .delete-btn:hover {
                background-color: #c82333;
            }

            .search-box {
                margin-bottom: 20px;
            }

            .no-data {
                text-align: center;
                padding: 50px;
                color: #6c757d;
            }

            .no-data i {
                font-size: 4rem;
                margin-bottom: 20px;
            }

            .status-badge {
                padding: 5px 10px;
                border-radius: 20px;
                font-size: 0.8rem;
                font-weight: bold;
            }

            .status-pending {
                background-color: #ffc107;
                color: #000;
            }

            .status-approved {
                background-color: #28a745;
                color: white;
            }

            .status-rejected {
                background-color: #dc3545;
                color: white;
            }

            .dashboard-actions {
                text-align: right;
                margin-bottom: 20px;
            }

            .table thead {
                background-color: #f8f9fa;
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <nav class="navbar bg-body-tertiary">
                <div class="container-fluid">
                    <h2><strong>
                            <a class="navbar-brand">KYC Admin Dashboard</a>
                        </strong></h2>
                    <a href="index.aspx" class="btn btn-primary">
                        <i class="fas fa-plus"></i> New KYC Form
                    </a>
                </div>
            </nav>

            <div class="container">
                <h1 class="text-center" style="text-align: center; margin-top: 20px;">Admin Dashboard</h1>
                <p class="text-center">Manage and review all KYC form submissions</p>
            </div>

            <div class="spacer">
                <div class="section-header">Statistics Overview</div>
            </div>

            <div class="container">
                <!-- Statistics Cards -->
                <div class="row mb-4">
                    <div class="col-md-3">
                        <div class="stats-card">
                            <div class="stats-number" id="totalSubmissions">0</div>
                            <div class="stats-label">Total Submissions</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="stats-card">
                            <div class="stats-number" id="todaySubmissions">0</div>
                            <div class="stats-label">Today's Submissions</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="stats-card">
                            <div class="stats-number" id="pendingSubmissions">0</div>
                            <div class="stats-label">Pending Review</div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="stats-card">
                            <div class="stats-number" id="approvedSubmissions">0</div>
                            <div class="stats-label">Approved</div>
                        </div>
                    </div>
                </div>

                <div class="spacer">
                    <div class="section-header">KYC Submissions</div>
                </div>

                <!-- Data Table -->
                <div class="table-container">
                    <div class="dashboard-actions">
                        <button class="btn btn-danger" onclick="clearAllData()">
                            <i class="fas fa-trash"></i> Clear All Data
                        </button>
                    </div>

                    <div class="search-box">
                        <input type="text" class="form-control" id="searchInput"
                            placeholder="Search by name, email, mobile, or Aadhaar..." onkeyup="filterTable()">
                    </div>

                    <div class="table-responsive">
                        <table class="table table-hover table-bordered" id="submissionsTable">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Submission Date</th>
                                    <th>Full Name</th>
                                    <th>Email</th>
                                    <th>Mobile</th>
                                    <th>Aadhaar</th>
                                    <th>Account Type</th>
                                    <th>Status</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody id="tableBody">
                                <!-- Data will be populated here -->
                            </tbody>
                        </table>
                    </div>

                    <div id="noDataMessage" class="no-data" style="display: none;">
                        <i class="fas fa-inbox"></i>
                        <h4>No Submissions Yet</h4>
                        <p>Submit a new KYC form to see data here.</p>
                    </div>

                    <!-- Hidden field to store SQL Server data -->
                    <input type="hidden" id="hiddenSubmissionsData"
                        value='<%= HttpUtility.HtmlAttributeEncode(GetSubmissionsJSON()) %>' />
                </div>
            </div>

            <!-- View Details Modal -->
            <div class="modal fade" id="detailsModal" tabindex="-1" aria-labelledby="detailsModalLabel"
                aria-hidden="true">
                <div class="modal-dialog modal-xl">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="detailsModalLabel"><i class="fas fa-user"></i> KYC Form Details
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body" id="modalContent">
                            <!-- Details will be populated here -->
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                            <button type="button" class="btn btn-success"
                                onclick="updateStatus(null, 'approved')">Approve</button>
                            <button type="button" class="btn btn-danger"
                                onclick="updateStatus(null, 'rejected')">Reject</button>
                        </div>
                    </div>
                </div>
            </div>

        </form>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

        <script>
            let currentViewingId = null;

            // Load submissions when page loads
            document.addEventListener('DOMContentLoaded', function () {
                loadSubmissions();
            });

            function loadSubmissions() {
                const hiddenField = document.getElementById('hiddenSubmissionsData');
                let submissions = [];
                try {
                    console.log("Raw Data:", hiddenField.value); // Debugging
                    submissions = hiddenField ? JSON.parse(hiddenField.value || '[]') : [];
                } catch (e) {
                    console.error("JSON Parse Error:", e);
                    alert("Error loading data. Check console for details.");
                    return;
                }

                // Check for server reported error
                if (submissions.length > 0 && submissions[0].error) {
                    alert(submissions[0].error);
                    return;
                }

                const tableBody = document.getElementById('tableBody');
                const noDataMessage = document.getElementById('noDataMessage');

                updateStatistics(submissions);

                if (submissions.length === 0) {
                    tableBody.innerHTML = '';
                    noDataMessage.style.display = 'block';
                    return;
                }

                noDataMessage.style.display = 'none';
                tableBody.innerHTML = '';

                submissions.forEach((submission, index) => {
                    const row = document.createElement('tr');
                    const statusClass = submission.status === 'Approved' ? 'status-approved' :
                        submission.status === 'Rejected' ? 'status-rejected' : 'status-pending';

                    row.innerHTML = `
                    <td>${index + 1}</td>
                    <td>${new Date(submission.submissionDate).toLocaleString()}</td>
                    <td>${submission.fullName || 'N/A'}</td>
                    <td>${submission.email || 'N/A'}</td>
                    <td>${submission.mobile || 'N/A'}</td>
                    <td>${submission.aadhaar ? '****' + submission.aadhaar.slice(-4) : 'N/A'}</td>
                    <td>${submission.accountType || 'N/A'}</td>
                    <td><span class="status-badge ${statusClass}">${submission.status || 'Pending'}</span></td>
                    <td>
                        <button class="action-btn btn-success" onclick="updateStatus(${submission.id}, 'approved')" title="Approve" style="padding: 5px 10px;">
                            <i class="fas fa-check"></i>
                        </button>
                        <button class="action-btn btn-danger" onclick="updateStatus(${submission.id}, 'rejected')" title="Reject" style="padding: 5px 10px;">
                            <i class="fas fa-times"></i>
                        </button>
                        <button class="action-btn view-btn" onclick="viewDetails(${submission.id})" title="View Details">
                            <i class="fas fa-eye"></i>
                        </button>
                        <a href="index.aspx?id=${submission.id}" class="action-btn btn-primary" style="text-decoration:none; display:inline-block; padding: 5px 10px;" title="Edit">
                            <i class="fas fa-edit"></i>
                        </a>
                        <button class="action-btn delete-btn" onclick="deleteSubmission(${submission.id})" title="Delete">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                `;
                    tableBody.appendChild(row);
                });
            }

            function updateStatistics(submissions) {
                const today = new Date().toDateString();
                const todayCount = submissions.filter(s => new Date(s.submissionDate).toDateString() === today).length;
                const pendingCount = submissions.filter(s => s.status === 'Pending' || s.status === 'pending').length;
                const approvedCount = submissions.filter(s => s.status === 'Approved').length;

                document.getElementById('totalSubmissions').textContent = submissions.length;
                document.getElementById('todaySubmissions').textContent = todayCount;
                document.getElementById('pendingSubmissions').textContent = pendingCount;
                document.getElementById('approvedSubmissions').textContent = approvedCount;
            }

            function viewDetails(id) {
                currentViewingId = id;

                console.log("Fetching details for ID:", id);

                // Call WebMethod to get details
                fetch('admin-dashboard.aspx/GetSubmissionDetails', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ id: id })
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error("Network response was not ok: " + response.statusText);
                        }
                        return response.json();
                    })
                    .then(data => {
                        console.log("Raw Server Response:", data); // Debug log

                        let details;
                        try {
                            details = JSON.parse(data.d);
                        } catch (e) {
                            console.error("JSON Parse Error:", e, "Data:", data.d);
                            alert("Error parsing server data. See console.");
                            return;
                        }

                        if (details.error) {
                            alert(details.error);
                            return;
                        }

                        // Check if details is empty
                        if (Object.keys(details).length === 0) {
                            alert("No details found for this submission.");
                            return;
                        }

                        const content = document.getElementById('modalContent');

                        // Build detailed view
                        let html = `
                        <div class="row">
                            <div class="col-md-6">
                                <h6>Personal Information</h6>
                                <table class="table table-sm">
                                    <tr><th>Full Name:</th><td>${details.FirstName || ''} ${details.MiddleName || ''} ${details.LastName || ''}</td></tr>
                                    <tr><th>Email:</th><td>${details.Email || ''}</td></tr>
                                    <tr><th>Mobile:</th><td>${details.Mobile || ''}</td></tr>
                                    <tr><th>Aadhaar:</th><td>${details.AadhaarNumber || ''}</td></tr>
                                    <tr><th>Date of Birth:</th><td>${details.AadhaarDob ? new Date(details.AadhaarDob).toLocaleDateString() : 'N/A'}</td></tr>
                                    <tr><th>Gender:</th><td>${details.GenderName || 'N/A'}</td></tr>
                                    <tr><th>Marital Status:</th><td>${details.MaritalStatus || 'N/A'}</td></tr>
                                    <tr><th>Nationality:</th><td>${details.Nationality || 'N/A'}</td></tr>
                                </table>
                            </div>
                            <div class="col-md-6">
                                <h6>Account & Address</h6>
                                <table class="table table-sm">
                                    <tr><th>Account Type:</th><td>${details.AccountType || 'N/A'}</td></tr>
                                    <tr><th>Customer Type:</th><td>${details.CustomerType || 'N/A'}</td></tr>
                                    <tr><th>Branch:</th><td>${details.BranchName || 'N/A'}</td></tr>
                                    <tr><th>Address Type:</th><td>${details.AddressType || 'N/A'}</td></tr>
                                    <tr><th>Address:</th><td>${details.Street || ''}, ${details.City || ''}, ${details.State || ''} - ${details.Pincode || ''}</td></tr>
                                </table>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-12">
                                <h6>Financial & Employment</h6>
                                <p><strong>Occupation:</strong> ${details.Occupation || 'N/A'} | <strong>Income:</strong> ${details.IncomeRange || 'N/A'} | <strong>Funds:</strong> ${details.FundSource || 'N/A'}</p>
                            </div>
                        </div>
                    `;

                        content.innerHTML = html;

                        const modal = new bootstrap.Modal(document.getElementById('detailsModal'));
                        modal.show();
                    })
                    .catch(error => {
                        console.error('Error in viewDetails:', error);
                        alert('Error loading details: ' + error.message);
                    });
            }

            function updateStatus(id, newStatus) {
                const targetId = id || currentViewingId;
                if (!targetId) return;

                // Format status consistent with DB (Title Case)
                const status = newStatus.charAt(0).toUpperCase() + newStatus.slice(1);

                fetch('admin-dashboard.aspx/UpdateStatus', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ id: targetId, status: status })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d === 'Success') {
                            // Close modal if open
                            const modalEl = document.getElementById('detailsModal');
                            if (modalEl && modalEl.classList.contains('show')) {
                                const modal = bootstrap.Modal.getInstance(modalEl);
                                modal.hide();
                            }
                            window.location.reload();
                        } else {
                            alert('Error updating status: ' + data.d);
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                        alert('Error updating status');
                    });
            }

            function deleteSubmission(id) {
                if (!confirm('Are you sure you want to delete this submission? This cannot be undone.')) return;

                fetch('admin-dashboard.aspx/DeleteSubmission', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ id: id })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d === 'Success') {
                            window.location.reload();
                        } else {
                            alert('Error deleting submission: ' + data.d);
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                        alert('Error deleting submission');
                    });
            }

            function clearAllData() {
                alert('For safety, Clear All Data is disabled in this demo. Please use individual delete.');
            }

            function filterTable() {
                const input = document.getElementById('searchInput');
                const filter = input.value.toLowerCase();
                const table = document.getElementById('submissionsTable');
                const rows = table.getElementsByTagName('tr');

                for (let i = 1; i < rows.length; i++) {
                    const row = rows[i];
                    const cells = row.getElementsByTagName('td');
                    let found = false;

                    for (let j = 0; j < cells.length; j++) {
                        const cell = cells[j];
                        if (cell.textContent.toLowerCase().indexOf(filter) > -1) {
                            found = true;
                            break;
                        }
                    }

                    row.style.display = found ? '' : 'none';
                }
            }
        </script>
    </body>

    </html>