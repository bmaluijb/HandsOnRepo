// ============================================
// API Client — NN Pension Planner
// ============================================

const API = {
    baseUrl: '/api',

    async get(path) {
        const res = await fetch(`${this.baseUrl}${path}`);
        if (!res.ok) {
            const err = await res.json().catch(() => ({ message: res.statusText }));
            throw new Error(err.message || 'Request failed');
        }
        return res.json();
    },

    async post(path, data) {
        const res = await fetch(`${this.baseUrl}${path}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!res.ok) {
            const err = await res.json().catch(() => ({ message: res.statusText }));
            throw new Error(err.message || 'Request failed');
        }
        return res.json();
    },

    async put(path, data) {
        const res = await fetch(`${this.baseUrl}${path}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!res.ok) {
            const err = await res.json().catch(() => ({ message: res.statusText }));
            throw new Error(err.message || 'Request failed');
        }
        return res.json();
    },

    async delete(path) {
        const res = await fetch(`${this.baseUrl}${path}`, { method: 'DELETE' });
        if (!res.ok && res.status !== 204) {
            throw new Error('Delete failed');
        }
        return true;
    },

    // --- Participants ---
    getParticipants: () => API.get('/participants'),
    getParticipant: (id) => API.get(`/participants/${id}`),
    createParticipant: (data) => API.post('/participants', data),
    updateParticipant: (id, data) => API.put(`/participants/${id}`, data),
    deleteParticipant: (id) => API.delete(`/participants/${id}`),

    // --- Plans ---
    getPlans: () => API.get('/plans'),
    getPlan: (id) => API.get(`/plans/${id}`),

    // --- Enrollments ---
    getEnrollments: () => API.get('/enrollments'),
    getEnrollment: (id) => API.get(`/enrollments/${id}`),
    getEnrollmentsByParticipant: (id) => API.get(`/enrollments/participant/${id}`),
    createEnrollment: (data) => API.post('/enrollments', data),

    // --- Contributions ---
    getContributionsByEnrollment: (id) => API.get(`/contributions/enrollment/${id}`),
    getBalance: (id) => API.get(`/contributions/enrollment/${id}/balance`),
    getSummary: (id) => API.get(`/contributions/enrollment/${id}/summary`),
    addContribution: (data) => API.post('/contributions', data),

    // --- Projections ---
    getProjectionsByEnrollment: (id) => API.get(`/projections/enrollment/${id}`),
    getLatestProjection: (id) => API.get(`/projections/enrollment/${id}/latest`),
    calculateProjection: (data) => API.post('/projections/calculate', data)
};
