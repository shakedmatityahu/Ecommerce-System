import React, { useEffect, useState } from 'react';
import { Table, Dropdown, Form, Stack, Button, Card, Row, Col, Container, Modal } from 'react-bootstrap';
import { getToken } from '../services/SessionService';
import { RiSave2Fill, RiAddBoxFill} from 'react-icons/ri'; // Importing the shopping cart icon
import { CreateRule } from './CreateRule';


export const ProfileStoreRules = ({storeId} : {storeId : any}) => {
    const [rules, setRules] = useState<any[]>([]);
    const [showAddRuleModal, setShowAddRuleModal] = useState(false);

    const handleClose = () => setShowAddRuleModal(false);
    const handleSuccess = () => {
        window.location.reload();

    };

    useEffect(() => {
        const fetchRulesList = async () => {

            try {
                const response = await fetch(`https://localhost:7163/api/Market/Store/${storeId}/GetRules?identifier=${getToken()}`, {
                    method: 'GET'
                });
                const data = await response.json();
                if (response.ok) {
                    setRules(data.value);
                } else {
                    console.error('Error fetching rules list:', data.ErrorMessage);
                }
            } catch (error) {
                console.error('Error occurred while fetching rules list:', error);
            }
        };

        fetchRulesList();
    }, []); 



    return (
        <>
        <Container className="my-3">
            <h2>Store Rules</h2>
            <Table striped bordered hover className="my-3">
                <thead>
                    <tr>
                        <th>Rule ID</th>
                        <th>Rule Subject</th>
                    </tr>
                </thead>
                <tbody>
                    {rules.map((rule) => (
                        <tr>
                            <td>{rule.id}</td>
                            <td>{rule.subjectInfo}</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
            <Button variant="outline-success" onClick={() => setShowAddRuleModal(true)}> <RiAddBoxFill size={20} /></Button>

            <Modal show={showAddRuleModal} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Add Rule</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CreateRule onClose={handleClose} onSuccess={handleSuccess} storeId={storeId} rules={rules}/>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>

        </Container>
        </>
    );
};

export default ProfileStoreRules;
