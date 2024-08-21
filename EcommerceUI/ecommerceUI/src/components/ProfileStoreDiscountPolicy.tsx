import React, { useEffect, useState } from 'react';
import { Table, Dropdown, Form, Stack, Button, Card, Row, Col, Container, Modal } from 'react-bootstrap';
import { getToken } from '../services/SessionService';
import { RiSave2Fill,  RiDeleteBin2Fill, RiAddBoxFill} from 'react-icons/ri'; // Importing the shopping cart icon
import { CreatePolicy } from './CreactePolicy';

//sorry for the duplicated code between the policy types.
export const ProfileStoreDiscountPolicy = ({storeId} : {storeId : any}) => {
    const [policies, setPolicies] = useState<any[]>([]);
    const [showAddPolicyModal, setShowAddPolicyModal] = useState(false);

    const handleClose = () => setShowAddPolicyModal(false);
    const handleSuccess = () => {
        window.location.reload();

    };

    useEffect(() => {
        const fetchPolicies = async () => {

            try {
                const response = await fetch(`https://localhost:7163/api/Market/Store/${storeId}/Policies/Discount?identifier=${getToken()}`, {
                    method: 'GET'
                });
                const data = await response.json();
                if (response.ok) {
                    setPolicies(data.value);
                } else {
                    console.error('Error fetching purchase policies:', data.ErrorMessage);
                }
            } catch (error) {
                console.error('Error occurred while fetching purchase policies:', error);
            }
        };

        fetchPolicies();
    }, []); 

    const deletePolicy = async (policyId : any, storeId : any) => {
        const response = await fetch(`https://localhost:7163/api/Market/Store/${storeId}/Policies/Discount?identifier=${getToken()}&policyId=${policyId}&storeId=${storeId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            },
        })
        if (response.ok) {
            alert('Policy removed successfully');
        } else {
            const responseData = await response.json();
            console.error('Failed to removed Policy:', responseData);
            alert('Failed to remove Policy. Please try again later.');
        }
        window.location.reload();
    }  

    return (
        <>
        <Container className="my-3">
            <h2>Store Discount Policies</h2>
            <Table striped bordered hover className="my-3">
                <thead>
                    <tr>
                        <th>Policy ID</th>
                        <th>Percentage</th>
                        <th>Expiration Date</th>
                        <th>Rule ID</th>
                        <th>Rule Subject</th>
                        <th> Actions </th>
                    </tr>
                </thead>
                <tbody>
                    {policies.map((policy) => (
                        <tr>
                            <td>{policy.id}</td>
                            <td>{policy.precentage}%</td>
                            <td>{policy.expirationDate}</td>
                            <td>{policy.rule.id}</td>
                            <td>{policy.rule.subjectInfo}</td>
                            <td>
                                <Button variant="outline-danger" onClick={() => deletePolicy(policy.id, policy.storeId)}> <RiDeleteBin2Fill size={20} /></Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
            <Button variant="outline-success" onClick={() => setShowAddPolicyModal(true)}> <RiAddBoxFill size={20} /></Button>

            <Modal show={showAddPolicyModal} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Add Policy</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CreatePolicy onClose={handleClose} onSuccess={handleSuccess} storeId={storeId} defPolicyType="Discount"/>
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

export default ProfileStoreDiscountPolicy;
