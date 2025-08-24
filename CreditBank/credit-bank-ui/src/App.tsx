import React from 'react';
import SubmitCreditRequest from './SubmitCreditRequest';
import ListCreditRequests from './ListCreditRequests';
import MakePayment from './MakePayment';

const App: React.FC = () => {
    return (
        <div>
            <h1>Credit Bank</h1>
            <hr />
            <SubmitCreditRequest />
            <hr />
            <ListCreditRequests />
            <hr />
            <MakePayment />
        </div>
    );
};

export default App;