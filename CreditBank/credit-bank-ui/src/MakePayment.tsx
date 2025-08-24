import React, { useState } from 'react';
import api from './api';
import axios from 'axios';

const MakePayment: React.FC = () => {
    const [creditId, setCreditId] = useState('');
    const [userId, setUserId] = useState('');
    const [amount, setAmount] = useState<number | ''>('');
    const [message, setMessage] = useState('');

    const handlePayment = async () => {
        try {
            const response = await api.patch(`/credits/${creditId}/payment`, {
                userId,
                creditId,
                amount
            });
            setMessage('Payment processed successfully');
        } catch (error) {
            if (axios.isAxiosError(error) && error.response) {
                setMessage(error.response.data.Message);
            } else {
                setMessage('Error processing payment');
            }
        }
    };

    return (
        <div>
            <h2>Make Payment</h2>
            <form onSubmit={(e) => { e.preventDefault(); handlePayment(); }}>
                <input type="text" placeholder="Credit ID" value={creditId} onChange={(e) => setCreditId(e.target.value)} required />
                <input type="text" placeholder="User ID" value={userId} onChange={(e) => setUserId(e.target.value)} required />
                <input type="number" placeholder="Amount" value={amount} onChange={(e) => setAmount(Number(e.target.value))} required />
                <button type="submit">Make Payment</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
};

export default MakePayment;