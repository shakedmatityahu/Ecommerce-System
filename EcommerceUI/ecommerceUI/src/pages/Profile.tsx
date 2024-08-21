import React, { useEffect, useState } from "react";
import { Container, Image, Col, Row, Button, Stack } from "react-bootstrap";
import avatar from '../assets/avatar.jpg';
import PurchaseHistory from "../components/PurchaseHistory";
import ProfileStoreNav from "../components/ProfileStoreNav";
import NotificationsNav from "../components/NotificationsNav";
import LogoutButton from "../components/LogoutButton";
import { getLoggedIn, getUserName } from '../services/SessionService';
import { useNavigate } from "react-router-dom";

interface ProfileProps {}

export const Profile: React.FC<ProfileProps> = () => {
    const navigate = useNavigate();
    const userDataString = getUserName();
    const [view, setView] = useState<'profileStoreNav' | 'purchaseHistory' | 'notificationsNav' >('purchaseHistory');

    const handleViewChange = (newView: 'profileStoreNav' | 'purchaseHistory' | 'notificationsNav') => {
        setView(newView);
    };

    useEffect(() => {
        const loggedIn = getLoggedIn();
        if(!loggedIn){
            navigate("/Login");
        }
    }, []);


    return (
        <>
            <Row className="full-height-row">
                <Col md={2} className="profile-left">
                    <Stack gap={2}>
                        <Image src={avatar} roundedCircle className="w-25 mx-auto" />
                        <p>{userDataString}</p>
                        <Button variant="outline-secondary" onClick={() => handleViewChange('purchaseHistory')}> Purchase History </Button>
                        <Button variant="outline-secondary" onClick={() => handleViewChange('profileStoreNav')}> Store details</Button>
                        <Button variant="outline-secondary" onClick={() => handleViewChange('notificationsNav')}> Notifications </Button>
                        <LogoutButton />
                    </Stack>
                </Col>

                <Col className="profile-right">
                    {view === 'profileStoreNav' && <ProfileStoreNav />}
                    {view === 'purchaseHistory' && <PurchaseHistory view={view} />}
                    {view === 'notificationsNav' && <NotificationsNav view={view} /> }
                </Col>
            </Row>
        </>
    );
};

export default Profile;
