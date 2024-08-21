import React, { useEffect, useState } from 'react';
import { Container, Table } from 'react-bootstrap';
import { getToken, getUserName } from '../services/SessionService';

interface PurchaseHistoryProps {
    view: 'profileStoreNav' | 'purchaseHistory';
}

const PurchaseHistory: React.FC<PurchaseHistoryProps> = ({ view }) => {
    const [purchases, setPurchases] = useState<any[]>([]);

    useEffect(() => {
        if (view === 'purchaseHistory') {
            // Fetch purchase history data from the API
            const fetchPurchaseHistory = async () => {

                try {
                    //TODO change the url token id
                    const response = await fetch(`https://localhost:7163/api/Client/Member/PurchaseHistory?username=${getUserName()}`, {
                        method: 'GET'
                    });
                    const data = await response.json();
                    if (response.ok) {
                        console.log(data.value[0].baskets);
                        const newPurchases = data.value[0].baskets.flatMap((basket: any) => basket.products);
                console.log(newPurchases);
                setPurchases(newPurchases);
                    } else {
                        console.error('Error fetching purchase history:', data.ErrorMessage);
                    }
                } catch (error) {
                    console.error('Error occurred while fetching purchase history:', error);
                }
            };

            fetchPurchaseHistory();
        }
    }, [view]); // Adding view as a dependency

    console.log(purchases);
    return (
        <Container className="my-3">
            <h2>Purchase History</h2>
            <Table striped bordered hover className="my-3">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Item Name</th>
                        <th>Description</th>
                        <th>Price</th>
                        {/* <th>Date Purchased</th> */}
                        <th>Quantity</th>
                    </tr>
                </thead>
                <tbody>
                    {purchases.map((purchase, index) => (
                        <tr key={index}>
                            <td>{index + 1}</td>
                            <td>{purchase.name}</td>
                            <td>{purchase.description}</td>
                            <td>{purchase.price}</td>
                            {/* <td>{new Date(purchase.datePurchased).toLocaleDateString()}</td> */}
                            <td>{purchase.quantity}</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </Container>
    );
};

export default PurchaseHistory;
