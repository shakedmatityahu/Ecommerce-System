import React, { useState } from 'react';
import { Form, Button } from 'react-bootstrap';
import { getToken } from '../services/SessionService';

export const CreateStore = ({ onClose, onSuccess }: { onClose: any, onSuccess: any }) => {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [phoneNum, setPhoneNum] = useState('');
    const [nameError, setNameError] = useState('');
    const [emailError, setEmailError] = useState('');
    const [phoneNumError, setPhoneNumError] = useState('');

    const handleCreateStore = () => {
        // Validate the inputs
        let valid = true;

        if (name.trim() === '') {
            setNameError('Please enter the store name');
            valid = false;
        }

        if (email.trim() === '') {
            setEmailError('Please enter an email');
            valid = false;
        } else if (!/\S+@\S+\.\S+/.test(email)) {
            setEmailError('Please enter a valid email');
            valid = false;
        }

        if (phoneNum.trim() === '') {
            setPhoneNumError('Please enter a phone number');
            valid = false;
        }

        if (valid) {
            createStore();
        }
    };

    const createStore = async () => {
         const response= await fetch(`https://localhost:7163/api/Client/Client/CreateStore?identifier=${getToken()}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ name, email, phoneNum }),
        })
        if (response.ok) {
            window.alert('Store created successfully');
            onClose();
            onSuccess();
        } else {
            const responseData = await response.json();
            console.error('Failed to create store:', responseData);
            // Optionally, you can set specific errors based on the response
            alert('Failed to create store. Please try again later.');
        }
    };

    return (
        <Form>
            <Form.Group className="mb-3">
                <Form.Label>Store Name</Form.Label>
                <Form.Control 
                    type="text"
                    value={name}
                    onChange={(ev) => setName(ev.target.value)} 
                    placeholder="Enter store name" 
                />
                <label className="errorLabel">{nameError}</label>
            </Form.Group>
            <Form.Group className="mb-3">
                <Form.Label>Email</Form.Label>
                <Form.Control 
                    type="email"
                    value={email}
                    onChange={(ev) => setEmail(ev.target.value)} 
                    placeholder="Enter email" 
                />
                <label className="errorLabel">{emailError}</label>
            </Form.Group>
            <Form.Group className="mb-3">
                <Form.Label>Phone Number</Form.Label>
                <Form.Control 
                    type="text"
                    value={phoneNum}
                    onChange={(ev) => setPhoneNum(ev.target.value)} 
                    placeholder="Enter phone number" 
                />
                <label className="errorLabel">{phoneNumError}</label>
            </Form.Group>
            <Button variant="primary" type="button" onClick={handleCreateStore}>
                Create Store
            </Button>
        </Form>
    );
};
