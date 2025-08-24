import React, { useState } from 'react';
import api from './api';
import axios from 'axios';

const SubmitCreditRequest: React.FC = () => {
    const [fullName, setFullName] = useState('');
    const [email, setEmail] = useState('');
    const [monthlyIncome, setMonthlyIncome] = useState<number | ''>('');
    const [creditAmount, setCreditAmount] = useState<number | ''>('');
    const [typeEnum, setTypeEnum] = useState<number | ''>(0);
    const [message, setMessage] = useState('');

    const handleSubmit = async () => {
        try {
            const response = await api.post('/credits/apply', {
                fullName,
                email,
                monthlyIncome,
                creditAmount,
                typeEnum
            });
            setMessage('Credit request submitted successfully');
        } catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                setMessage(error.response.data.Message);
            } else {
                setMessage('Error submitting credit request');
            }
        }
    };

    return (
        <div>
            <h2>Submit Credit Request</h2>
            <form onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
                <input type="text" placeholder="Full Name" value={fullName} onChange={(e) => setFullName(e.target.value)} required />
                <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                <input type="number" placeholder="Monthly Income" value={monthlyIncome} onChange={(e) => setMonthlyIncome(Number(e.target.value))} required />
                <input type="number" placeholder="Credit Amount" value={creditAmount} onChange={(e) => setCreditAmount(Number(e.target.value))} required />
                <select value={typeEnum} onChange={(e) => setTypeEnum(Number(e.target.value))} required>
                    <option value={0}>Mortgage</option>
                    <option value={1}>Auto</option>
                    <option value={2}>Personal</option>
                </select>
                <button type="submit">Submit</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
};

export default SubmitCreditRequest;