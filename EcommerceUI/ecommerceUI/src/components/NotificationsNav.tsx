import React, { useEffect, useState } from 'react';
import { Container, Table } from 'react-bootstrap';
import { getToken, getUserName } from '../services/SessionService';

interface NotificationsNavProps {
    view: 'profileStoreNav' | 'purchaseHistory' | 'notificationsNav';
}

const NotificationsNav: React.FC<NotificationsNavProps> = ({ view }) => {
    const [notifications, setNotifications] = useState<any[]>([]);

    useEffect(() => {
        if (view === 'notificationsNav') {
            // Fetch purchase history data from the API
            const fetchNotifications = async () => {
                try {
                    const response = await fetch(`https://localhost:7163/api/Client/Client/Notifications?identifier=${getToken()}`, {
                        method: 'GET'
                    });
                    const data = await response.json();
                    
                    if (response.ok) {
                        setNotifications(data.value); 
                       
                    } else {
                        console.error('Error fetching Notifications', data.ErrorMessage);
                    }
                } catch (error) {
                    console.error('Error occurred while fetching Notifications:', error);
                }
            };
            
            fetchNotifications();
        }
    }, [view]); // Adding view as a dependency

    console.log(notifications);
    return (
        <Container className="my-3">
            <h2>Notification</h2>
            <Table striped bordered hover className="my-3">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Comment</th>
                    </tr>
                </thead>
                <tbody>
                    {notifications.map((notification, index) => (
                        <tr key={index}>
                            <td>{index + 1}</td>
                            <td>{notification.comment}</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </Container>
    );
};

export default NotificationsNav;