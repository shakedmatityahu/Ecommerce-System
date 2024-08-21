import React, { useState, useEffect } from 'react';
import { Table, Dropdown, Form, Button, Container, Modal } from 'react-bootstrap';
import { Role } from './ProfileStoreNav';
import { RiLockLine } from 'react-icons/ri'; // Importing lock icon
import { getToken } from '../services/SessionService';
import { permission } from 'process';


// Enum for possible permissions
enum Permission {
    addProduct = 'Add Product',
    removeProduct = 'Remove Product',
    updateProductPrice = 'Update Product Price',
    updateProductDiscount = 'Update Product Discount',
    updateProductQuantity = 'Update Product Quantity',
    editPermissions = 'Edit Permissions',
}

interface TableRowProps {
    role: Role;
    index: number;
    storeId: number; // Add storeId if needed
    onRemoveAppointee: (username: string, roleName: string) => void; // Callback function to handle remove appointee
}

const TableRow: React.FC<TableRowProps> = ({ role, index, storeId, onRemoveAppointee }) => {
    const isOwner = role.role === 'Owner';
    const isFounder = role.role === 'Founder';
    const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);

    useEffect(() => {
        // Initialize selectedPermissions with current role.permissions
        setSelectedPermissions(role.permissions);
    }, [role.permissions]);

    const handlePermissionChange = (permission: string, isChecked: boolean) => {
        setSelectedPermissions(prevPermissions => {
            if (isChecked) {
                // Add permission to selectedPermissions if checked
                return [...prevPermissions, permission];
            } else {
                // Remove permission from selectedPermissions if unchecked
                return prevPermissions.filter(perm => perm !== permission);
            }
        });
    };

    const handleSavePermissions = () => {
        // Determine permissions to add (POST) and permissions to remove (DELETE)
        const permissionsToAdd = selectedPermissions.filter(perm => !role.permissions.includes(perm));
        const permissionsToRemove = role.permissions.filter(perm => !selectedPermissions.includes(perm));

        if (permissionsToAdd.length > 0) {
            fetch(`https://localhost:7163/api/Market/Store/${storeId}/Permisions?identifier=${getToken()}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ memberUserName: role.username, permission: permissionsToAdd, roleName: role.role }),
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Failed to add permission');
                    }
                    else {
                        console.log('Permission added successfully');
                        alert('Permission added successfully');
                    }
                })
                .catch(error => {
                    console.error('Error adding permission:', error);
                });
        }

        if (permissionsToRemove.length > 0) {
            fetch(`https://localhost:7163/api/Market/Store/${storeId}/Permisions?identifier=${getToken()}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ memberUserName: role.username, permission: permissionsToRemove, roleName: role.role }),
            })
                .then(response => {
                    if (!response.ok) {
                        alert('Failed to remove permission');
                        throw new Error('Failed to remove permission');
                    }
                    else {
                        console.log('Permission removed successfully');
                        alert('Permission removed successfully');
                    }
                })
                .catch(error => {
                    alert('Failed to remove permission');
                    console.error('Error removing permission:', error);
                });
        }

        updatedPermissions(permissionsToAdd, permissionsToRemove);
    };

    const updatedPermissions = (permissionsToAdd: string[], permissionsToRemove: string[]) => {
        const updatedPermissions = role.permissions.slice();
        permissionsToAdd.forEach(permission => {
            if (!updatedPermissions.includes(permission)) {
                updatedPermissions.push(permission);
            }
        });
        permissionsToRemove.forEach(permission => {
            const index = updatedPermissions.indexOf(permission);
            if (index !== -1) {
                updatedPermissions.splice(index, 1);
            }
        });
        role.permissions = updatedPermissions;
        console.log('Permissions updated:', role.permissions);

    };

    return (
        <tr>
            <td>{index + 1}</td>
            <td>{role.username}</td>
            <td>{role.role}</td>
            <td>{ role.appointer}</td> {/* Possible to add the option {isFounder? '' : role.appointer} */}
            <td>
                {isOwner && (
                    <div className="text-success">
                        <RiLockLine size={30} /> {/* RiLockLine icon */}
                        <i className="fas fa-lock ml-2"></i> {/* Lock icon */}
                        Full permission (except Close Store)
                    </div>
                )}
                {isFounder && (
                    <div className="text-success">
                        <RiLockLine size={30} /> {/* RiLockLine icon */}
                        <i className="fas fa-lock ml-2"></i> {/* Lock icon */}
                        Full permission
                    </div>
                )}
                {!isOwner && !isFounder && (
                    <>
                       <Form>
                            {Object.keys(Permission).map((permission: string) => (
                                <div key={`permission-${permission}`} className="mb-3">
                                    <Form.Check
                                        type="checkbox"
                                        id={`permission-checkbox-${permission}`}
                                        label={(Permission as Record<string, string>)[permission]}
                                        checked={selectedPermissions.includes(permission)}
                                        onChange={(e) => handlePermissionChange(permission, e.target.checked)}
                                    />
                                    {role.permissions.includes(permission) && (
                                        <span style={{ marginLeft: '5px', color: 'green' }}>✓</span>
                                    )}
                                </div>
                            ))}
                        </Form>
                        <Button variant="primary" onClick={handleSavePermissions}>Save Permissions</Button>
                    </>
                )}
            </td>
            <td>
                {!isFounder && (
                <Button variant="danger" onClick={() => onRemoveAppointee(role.username, role.role)}>
                    Remove Appointee
                </Button> )}
            </td>
        </tr>
    );
};


interface MyTableProps {
    roles: Role[];
    storeId: number;
}

const MyTable: React.FC<MyTableProps> = ({ roles, storeId }) => {
    const [showAppointeeModal, setShowAppointeeModal] = useState(false);
    const [memberUserName, setName] = useState('');
    const [roleName, setRole] = useState('Owner');
    const [appointer, setAppionter] = useState('');
    const [permission, setPermissions] = useState<string[]>([]);

    const handleAddAppointeeClick = () => {
        setShowAppointeeModal(true);
    };

    const handleCloseAppointeeModal = () => {
        setShowAppointeeModal(false);
        window.location.reload();
    };

    const handleAddAppointeeSubmit = () => {
        console.log('Adding appointee:',  roleName);
        fetch(`https://localhost:7163/api/Market/Store/${storeId}/Staff?identifier=${getToken()}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ memberUserName, permission, roleName }),
        })
            .then((r) => {
                if (r.ok) {
                    return r.json();
                } else {
                    throw new Error('Failed to add appointee');
                }
            })
            .then((data) => {
                console.log('Appointee added successfully:', data);
                handleCloseAppointeeModal();
            })
            .catch((error) => {
                alert(error.message);
                console.error('Error adding appointee:', error);
            });
    };

    const handleRemoveAppointee = (memberUserName: string, roleName: string) => {
        fetch(`https://localhost:7163/api/Market/Store/${storeId}/Staff?identifier=${getToken()}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ memberUserName,permission, roleName }), //TOOO : delete permission and check its working
        })
            .then((response) => {
                if (response.ok) {
                    console.log(`Successfully removed appointee ${memberUserName}`);
                    window.location.reload(); // TODO need to think about another way to update the table

                } else {
                    alert('Failed to remove appointee');
                    throw new Error(`Failed to remove appointee ${memberUserName}`);
                }
            })
            .catch((error) => {
                alert('Error removing appointee');
                console.error('Error removing appointee:', error);
            });
    };

    return (
        <>
            <Container className="d-flex justify-content-end my-3">
                <Button variant="primary" onClick={handleAddAppointeeClick}>
                    Add Appointee
                </Button>
            </Container>
            <Table striped bordered hover className="my-3">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Name</th>
                        <th>Role</th>
                        <th>Appointer</th>
                        <th>Permissions</th>
                        <th>Actions</th> {/* New column for Actions */}
                    </tr>
                </thead>
                <tbody>
                    {roles.map((role, index) => (
                        <TableRow
                            key={`role-${index}`}
                            role={role}
                            index={index}
                            storeId={storeId}
                            onRemoveAppointee={handleRemoveAppointee} // Pass the removal function as prop
                        />
                    ))}
                </tbody>
            </Table>
            <Modal show={showAppointeeModal} onHide={handleCloseAppointeeModal}>
                <Modal.Header closeButton>
                    <Modal.Title>Add Appointee</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group controlId="formUsername">
                            <Form.Label>Username</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="Enter username"
                                value={memberUserName}
                                onChange={(e) => setName(e.target.value)}
                            />
                        </Form.Group>
                        <Form.Group controlId="formRole">
                            <Form.Label>Role</Form.Label>
                            <Form.Select value={roleName} onChange={(e) => setRole(e.target.value)}>
                                <option value="Owner">Owner</option>
                                <option value="Manager">Manager</option>
                            </Form.Select>
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleCloseAppointeeModal}>
                        Close
                    </Button>
                    <Button variant="primary" type="submit" onClick={handleAddAppointeeSubmit}>
                        Submit
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default MyTable;


