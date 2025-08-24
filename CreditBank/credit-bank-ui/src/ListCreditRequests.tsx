import React, { useEffect, useState } from 'react';
import api from './api';

const ListCreditRequests: React.FC = () => {
    const [creditRequests, setCreditRequests] = useState<any[]>([]);
    const [status, setStatus] = useState<number | ''>('');
    const [typeEnum, setTypeEnum] = useState<number | ''>('');

    useEffect(() => {
        fetchCreditRequests();
    }, [status, typeEnum]);

    const fetchCreditRequests = async () => {
        try {
            const response = await api.get('/credits', {
                params: { status, typeEnum }
            });
            setCreditRequests(response.data);
        } catch (error) {
            console.error('Error fetching credit requests', error);
        }
    };

    return (
        <div>
            <h2>Credit Requests</h2>
            <select value={status} onChange={(e) => setStatus(Number(e.target.value))}>
                <option value="">All</option>
                <option value={0}>PendingReview</option>
                <option value={1}>Ongoing</option>
                <option value={2}>Canceled</option>
                <option value={3}>Finished</option>
            </select>
            <select value={typeEnum} onChange={(e) => setTypeEnum(Number(e.target.value))}>
                <option value="">All</option>
                <option value={0}>Mortgage</option>
                <option value={1}>Auto</option>
                <option value={2}>Personal</option>
            </select>
            <button onClick={fetchCreditRequests}>Filter</button>
            <ul>
                {creditRequests.map((request) => (
                    <li key={request.id}>
                        {request.fullName} - {request.email} - {request.creditAmount} - {request.typeEnum}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default ListCreditRequests;