// ============================================
// UI Rendering — NN Pension Planner
// ============================================

const UI = {
    formatCurrency(amount) {
        return new Intl.NumberFormat('nl-NL', { style: 'currency', currency: 'EUR' }).format(amount);
    },

    formatDate(dateStr) {
        if (!dateStr) return '—';
        return new Date(dateStr).toLocaleDateString('nl-NL', { year: 'numeric', month: 'short', day: 'numeric' });
    },

    statusBadge(status) {
        const cls = (status || '').toLowerCase();
        return `<span class="badge badge-${cls}">${status}</span>`;
    },

    loading() {
        return '<div class="loading">Loading...</div>';
    },

    emptyState(icon, message) {
        return `<div class="empty-state"><div class="icon">${icon}</div><p>${message}</p></div>`;
    },

    // --- Dashboard ---
    renderDashboard(participants, plans, enrollments, contributions) {
        const totalBalance = contributions.reduce((sum, c) => sum + c.employeeAmount + c.employerAmount, 0);
        const activeEnrollments = enrollments.filter(e => e.status === 'Active').length;

        return `
            <div class="section-header"><h2>Dashboard</h2></div>
            <div class="dashboard-grid">
                <div class="stat-card">
                    <h3>Participants</h3>
                    <div class="value">${participants.length}</div>
                    <div class="sub">Registered members</div>
                </div>
                <div class="stat-card">
                    <h3>Active Plans</h3>
                    <div class="value">${plans.filter(p => p.isActive).length}</div>
                    <div class="sub">Available pension plans</div>
                </div>
                <div class="stat-card">
                    <h3>Active Enrollments</h3>
                    <div class="value">${activeEnrollments}</div>
                    <div class="sub">Out of ${enrollments.length} total</div>
                </div>
                <div class="stat-card">
                    <h3>Total Contributions</h3>
                    <div class="value">${UI.formatCurrency(totalBalance)}</div>
                    <div class="sub">${contributions.length} transactions</div>
                </div>
            </div>

            <div class="section-header"><h2>Recent Participants</h2></div>
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Employer</th>
                        <th>Annual Salary</th>
                        <th>Joined</th>
                    </tr>
                </thead>
                <tbody>
                    ${participants.slice(0, 5).map(p => `
                        <tr>
                            <td>${UI.escapeHtml(p.firstName)} ${UI.escapeHtml(p.lastName)}</td>
                            <td>${UI.escapeHtml(p.employerName)}</td>
                            <td>${UI.formatCurrency(p.annualSalary)}</td>
                            <td>${UI.formatDate(p.joinDate)}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    },

    // --- Participants Page ---
    renderParticipants(participants) {
        return `
            <div class="section-header">
                <h2>Participants</h2>
                <button class="btn btn-primary" onclick="App.showAddParticipant()">+ Add Participant</button>
            </div>
            ${participants.length === 0 ? UI.emptyState('👥', 'No participants yet') : `
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Employer</th>
                        <th>Salary</th>
                        <th>Age</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${participants.map(p => `
                        <tr>
                            <td><strong>${UI.escapeHtml(p.firstName)} ${UI.escapeHtml(p.lastName)}</strong></td>
                            <td>${UI.escapeHtml(p.email)}</td>
                            <td>${UI.escapeHtml(p.employerName)}</td>
                            <td>${UI.formatCurrency(p.annualSalary)}</td>
                            <td>${p.age}</td>
                            <td>
                                <button class="btn btn-outline btn-sm" onclick="App.viewParticipant('${p.id}')">View</button>
                                <button class="btn btn-sm" style="color:#c53030" onclick="App.deleteParticipant('${p.id}')">Delete</button>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>`}
        `;
    },

    renderAddParticipantModal() {
        return `
            <div class="modal-overlay" onclick="App.closeModal(event)">
                <div class="modal" onclick="event.stopPropagation()">
                    <h3>Add New Participant</h3>
                    <div class="form-group">
                        <label>First Name</label>
                        <input type="text" id="firstName" placeholder="e.g. Jan">
                    </div>
                    <div class="form-group">
                        <label>Last Name</label>
                        <input type="text" id="lastName" placeholder="e.g. de Vries">
                    </div>
                    <div class="form-group">
                        <label>Date of Birth</label>
                        <input type="date" id="dateOfBirth">
                    </div>
                    <div class="form-group">
                        <label>Email</label>
                        <input type="email" id="email" placeholder="jan@example.nl">
                    </div>
                    <div class="form-group">
                        <label>Employer</label>
                        <input type="text" id="employerName" placeholder="e.g. TechCorp BV">
                    </div>
                    <div class="form-group">
                        <label>Annual Salary (€)</label>
                        <input type="number" id="annualSalary" placeholder="55000">
                    </div>
                    <div class="modal-actions">
                        <button class="btn btn-outline" onclick="App.closeModal()">Cancel</button>
                        <button class="btn btn-primary" onclick="App.saveParticipant()">Save</button>
                    </div>
                </div>
            </div>
        `;
    },

    // --- Plans Page ---
    renderPlans(plans) {
        return `
            <div class="section-header"><h2>Pension Plans</h2></div>
            <div class="dashboard-grid">
                ${plans.map(p => `
                    <div class="stat-card" style="cursor: pointer" onclick="App.viewPlan('${p.id}')">
                        <h3>${UI.escapeHtml(p.name)}</h3>
                        <div class="value" style="font-size: 18px">${p.type}</div>
                        <div class="sub" style="margin-top: 12px">${UI.escapeHtml(p.description)}</div>
                        <div style="margin-top: 16px; display: grid; grid-template-columns: 1fr 1fr; gap: 8px; font-size: 13px">
                            <div><span style="color: #888">Employer Match:</span> <strong>${p.employerMatchPercentage}%</strong></div>
                            <div><span style="color: #888">Max Contrib:</span> <strong>${p.maxContributionPercentage}%</strong></div>
                            <div><span style="color: #888">Vesting:</span> <strong>${p.vestingPeriodYears} yr</strong></div>
                            <div><span style="color: #888">Min Salary:</span> <strong>${UI.formatCurrency(p.minimumAnnualSalary)}</strong></div>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;
    },

    // --- Contributions Page ---
    renderContributions(enrollments, participants, plans) {
        return `
            <div class="section-header"><h2>Contributions</h2></div>
            <p style="margin-bottom: 20px; color: #888">Select an enrollment to view contribution history and add new contributions.</p>
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Participant</th>
                        <th>Plan</th>
                        <th>Status</th>
                        <th>Contribution %</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${enrollments.map(e => {
                        const p = participants.find(x => x.id === e.participantId) || {};
                        const plan = plans.find(x => x.id === e.planId) || {};
                        return `
                            <tr>
                                <td>${UI.escapeHtml(p.firstName || '')} ${UI.escapeHtml(p.lastName || '')}</td>
                                <td>${UI.escapeHtml(plan.name || '')}</td>
                                <td>${UI.statusBadge(e.status)}</td>
                                <td>${e.contributionPercentage}%</td>
                                <td>
                                    <button class="btn btn-outline btn-sm" onclick="App.viewContributions('${e.id}')">History</button>
                                    ${e.status === 'Active' ? `<button class="btn btn-primary btn-sm" onclick="App.showAddContribution('${e.id}')">+ Add</button>` : ''}
                                </td>
                            </tr>
                        `;
                    }).join('')}
                </tbody>
            </table>
        `;
    },

    renderContributionHistory(contributions, balance) {
        return `
            <div class="section-header">
                <h2>Contribution History</h2>
                <button class="btn btn-outline" onclick="App.navigate('contributions')">← Back</button>
            </div>
            <div class="detail-panel">
                <div class="detail-row">
                    <span class="detail-label">Total Balance</span>
                    <span class="detail-value" style="font-size: 24px; color: #FF6600">${UI.formatCurrency(balance)}</span>
                </div>
            </div>
            ${contributions.length === 0 ? UI.emptyState('💰', 'No contributions yet') : `
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Employee</th>
                        <th>Employer</th>
                        <th>Total</th>
                        <th>Type</th>
                    </tr>
                </thead>
                <tbody>
                    ${contributions.sort((a, b) => new Date(b.date) - new Date(a.date)).map(c => `
                        <tr>
                            <td>${UI.formatDate(c.date)}</td>
                            <td>${UI.formatCurrency(c.employeeAmount)}</td>
                            <td>${UI.formatCurrency(c.employerAmount)}</td>
                            <td><strong>${UI.formatCurrency(c.totalAmount)}</strong></td>
                            <td>${c.type}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>`}
        `;
    },

    renderAddContributionModal(enrollmentId) {
        return `
            <div class="modal-overlay" onclick="App.closeModal(event)">
                <div class="modal" onclick="event.stopPropagation()">
                    <h3>Add Contribution</h3>
                    <div class="form-group">
                        <label>Employee Amount (€)</label>
                        <input type="number" id="employeeAmount" placeholder="250.00" step="0.01">
                    </div>
                    <div class="form-group">
                        <label>Type</label>
                        <select id="contributionType">
                            <option value="Regular">Regular</option>
                            <option value="Voluntary">Voluntary</option>
                            <option value="CatchUp">Catch-Up</option>
                        </select>
                    </div>
                    <p style="font-size: 12px; color: #888; margin-top: 8px">Employer match will be calculated automatically based on the plan terms.</p>
                    <div class="modal-actions">
                        <button class="btn btn-outline" onclick="App.closeModal()">Cancel</button>
                        <button class="btn btn-primary" onclick="App.saveContribution('${enrollmentId}')">Add Contribution</button>
                    </div>
                </div>
            </div>
        `;
    },

    // --- Projections Page ---
    renderProjections(enrollments, participants, plans) {
        return `
            <div class="section-header"><h2>Retirement Projections</h2></div>
            <p style="margin-bottom: 20px; color: #888">Generate retirement projections for any active enrollment.</p>
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Participant</th>
                        <th>Plan</th>
                        <th>Age</th>
                        <th>Contribution %</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${enrollments.filter(e => e.status === 'Active').map(e => {
                        const p = participants.find(x => x.id === e.participantId) || {};
                        const plan = plans.find(x => x.id === e.planId) || {};
                        return `
                            <tr>
                                <td>${UI.escapeHtml(p.firstName || '')} ${UI.escapeHtml(p.lastName || '')}</td>
                                <td>${UI.escapeHtml(plan.name || '')}</td>
                                <td>${p.age || '—'}</td>
                                <td>${e.contributionPercentage}%</td>
                                <td>
                                    <button class="btn btn-primary btn-sm" onclick="App.calculateProjection('${e.id}')">Calculate</button>
                                </td>
                            </tr>
                        `;
                    }).join('')}
                </tbody>
            </table>
            <div id="projection-result"></div>
        `;
    },

    renderProjectionResult(projection) {
        return `
            <div style="margin-top: 32px">
                <div class="section-header"><h2>Projection Results</h2></div>
                <div class="detail-panel">
                    <div class="detail-row">
                        <span class="detail-label">Retirement Age</span>
                        <span class="detail-value">${projection.retirementAge}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Years to Retirement</span>
                        <span class="detail-value">${projection.yearsToRetirement}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Current Balance</span>
                        <span class="detail-value">${UI.formatCurrency(projection.currentBalance)}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Monthly Contribution</span>
                        <span class="detail-value">${UI.formatCurrency(projection.monthlyContribution)}</span>
                    </div>
                </div>

                <div class="scenario-cards">
                    ${projection.scenarios.map(s => {
                        const cls = s.name.toLowerCase();
                        return `
                            <div class="scenario-card ${cls}">
                                <h4>${s.name}</h4>
                                <div class="amount">${UI.formatCurrency(s.estimatedLumpSum)}</div>
                                <div class="monthly">${UI.formatCurrency(s.estimatedMonthlyPension)} / month</div>
                                <div class="rate">${(s.annualReturnRate * 100).toFixed(1)}% annual return</div>
                            </div>
                        `;
                    }).join('')}
                </div>
            </div>
        `;
    },

    // --- Helpers ---
    escapeHtml(str) {
        const div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML;
    }
};
