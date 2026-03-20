// ============================================
// App Controller — NN Pension Planner
// ============================================

const App = {
    currentPage: 'dashboard',

    async init() {
        // Set up navigation
        document.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const page = link.dataset.page;
                App.navigate(page);
            });
        });

        // Load initial page
        await App.navigate('dashboard');
    },

    async navigate(page) {
        App.currentPage = page;

        // Update active nav
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.toggle('active', link.dataset.page === page);
        });

        const content = document.getElementById('main-content');
        content.innerHTML = UI.loading();

        try {
            switch (page) {
                case 'dashboard':
                    await App.loadDashboard();
                    break;
                case 'participants':
                    await App.loadParticipants();
                    break;
                case 'plans':
                    await App.loadPlans();
                    break;
                case 'contributions':
                    await App.loadContributions();
                    break;
                case 'projections':
                    await App.loadProjections();
                    break;
            }
        } catch (err) {
            content.innerHTML = UI.emptyState('⚠️', `Error: ${err.message}`);
        }
    },

    // --- Dashboard ---
    async loadDashboard() {
        const [participants, plans, enrollments, contributions] = await Promise.all([
            API.getParticipants(),
            API.getPlans(),
            API.getEnrollments(),
            API.get('/contributions/enrollment/' + (await API.getEnrollments())[0]?.id || '')
                .catch(() => [])
        ]);

        // Gather all contributions for balance display
        let allContributions = [];
        for (const e of enrollments) {
            try {
                const contribs = await API.getContributionsByEnrollment(e.id);
                allContributions = allContributions.concat(contribs);
            } catch { /* ignore */ }
        }

        document.getElementById('main-content').innerHTML =
            UI.renderDashboard(participants, plans, enrollments, allContributions);
    },

    // --- Participants ---
    async loadParticipants() {
        const participants = await API.getParticipants();
        document.getElementById('main-content').innerHTML = UI.renderParticipants(participants);
    },

    showAddParticipant() {
        const existing = document.querySelector('.modal-overlay');
        if (existing) existing.remove();
        document.body.insertAdjacentHTML('beforeend', UI.renderAddParticipantModal());
    },

    async saveParticipant() {
        const data = {
            firstName: document.getElementById('firstName').value,
            lastName: document.getElementById('lastName').value,
            dateOfBirth: document.getElementById('dateOfBirth').value,
            email: document.getElementById('email').value,
            employerName: document.getElementById('employerName').value,
            annualSalary: parseFloat(document.getElementById('annualSalary').value) || 0
        };

        try {
            await API.createParticipant(data);
            App.closeModal();
            await App.loadParticipants();
        } catch (err) {
            alert('Error: ' + err.message);
        }
    },

    async deleteParticipant(id) {
        if (!confirm('Are you sure you want to delete this participant?')) return;
        try {
            await API.deleteParticipant(id);
            await App.loadParticipants();
        } catch (err) {
            alert('Error: ' + err.message);
        }
    },

    async viewParticipant(id) {
        // Navigate to contributions filtered by participant (simple approach: go to contributions page)
        await App.navigate('contributions');
    },

    // --- Plans ---
    async loadPlans() {
        const plans = await API.getPlans();
        document.getElementById('main-content').innerHTML = UI.renderPlans(plans);
    },

    async viewPlan(id) {
        // Could expand to show plan details + enrollments
        const plan = await API.getPlan(id);
        alert(`${plan.name}\n\n${plan.description}\n\nType: ${plan.type}\nEmployer Match: ${plan.employerMatchPercentage}%\nMax Contribution: ${plan.maxContributionPercentage}%\nVesting: ${plan.vestingPeriodYears} years`);
    },

    // --- Contributions ---
    async loadContributions() {
        const [enrollments, participants, plans] = await Promise.all([
            API.getEnrollments(),
            API.getParticipants(),
            API.getPlans()
        ]);
        document.getElementById('main-content').innerHTML =
            UI.renderContributions(enrollments, participants, plans);
    },

    async viewContributions(enrollmentId) {
        const content = document.getElementById('main-content');
        content.innerHTML = UI.loading();

        const [contributions, balanceData] = await Promise.all([
            API.getContributionsByEnrollment(enrollmentId),
            API.getBalance(enrollmentId)
        ]);

        content.innerHTML = UI.renderContributionHistory(contributions, balanceData.balance);
    },

    showAddContribution(enrollmentId) {
        const existing = document.querySelector('.modal-overlay');
        if (existing) existing.remove();
        document.body.insertAdjacentHTML('beforeend', UI.renderAddContributionModal(enrollmentId));
    },

    async saveContribution(enrollmentId) {
        const data = {
            enrollmentId: enrollmentId,
            employeeAmount: parseFloat(document.getElementById('employeeAmount').value) || 0,
            type: document.getElementById('contributionType').value
        };

        try {
            await API.addContribution(data);
            App.closeModal();
            await App.viewContributions(enrollmentId);
        } catch (err) {
            alert('Error: ' + err.message);
        }
    },

    // --- Projections ---
    async loadProjections() {
        const [enrollments, participants, plans] = await Promise.all([
            API.getEnrollments(),
            API.getParticipants(),
            API.getPlans()
        ]);
        document.getElementById('main-content').innerHTML =
            UI.renderProjections(enrollments, participants, plans);
    },

    async calculateProjection(enrollmentId) {
        const resultDiv = document.getElementById('projection-result');
        if (resultDiv) resultDiv.innerHTML = UI.loading();

        try {
            const projection = await API.calculateProjection({
                enrollmentId: enrollmentId,
                retirementAge: 67
            });
            if (resultDiv) {
                resultDiv.innerHTML = UI.renderProjectionResult(projection);
            }
        } catch (err) {
            if (resultDiv) {
                resultDiv.innerHTML = UI.emptyState('⚠️', `Error: ${err.message}`);
            }
        }
    },

    // --- Modal helpers ---
    closeModal(event) {
        if (event && event.target !== event.currentTarget) return;
        const overlay = document.querySelector('.modal-overlay');
        if (overlay) overlay.remove();
    }
};

// Boot the app
document.addEventListener('DOMContentLoaded', () => App.init());
