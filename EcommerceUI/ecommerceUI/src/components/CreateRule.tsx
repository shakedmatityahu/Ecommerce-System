import React, {useEffect, useState } from 'react';
import { Form, Button, Row, Col, ListGroup } from 'react-bootstrap';
import { getToken } from '../services/SessionService';

export const CreateRule = ({ onClose, onSuccess, storeId, rules }: { onClose: any, onSuccess: any, storeId: any, rules : any }) => {
    const [ruleType, setRuleType] = useState('SimpleRule');
    const [subject, setSubject] = useState('');
    const [minQuantity, setMinQuantity] = useState('');
    const [maxQuantity, setMaxQuantity] = useState('');
    const [targetPrice, setTargetPrice] = useState('');
    const [operator, setOperator] = useState(0);
    const [rulesChecked, setRulesChecked] = useState<{[key: number]: boolean}>(
        () => {
            return rules.reduce((acc, rule) => {
                acc[rule.id] = false;
                return acc;
              }, {});
        }
    );

    const [ruleTypeError, setRuleTypeError] = useState('');
    const [subjectError, setSubjectError] = useState('');
    const [minQuantityError, setMinQuantityError] = useState('');
    const [maxQuantityError, setMaxQuantityError] = useState('');
    const [targetPriceError, setTargetPriceError] = useState('');


    const onCheckClick = (checked : boolean, ruleId : number) => {
        console.log(`im in on check click with id = ${ruleId} and e.target.value = ${checked}`);
        console.log(rulesChecked)
        setRulesChecked(prevRulesChecked => {
            // Create a copy of previous state
            const updatedRulesChecked = {
              ...prevRulesChecked,
              [ruleId]: checked,
            };
            // Log the updated state (optional for debugging)
            console.log(updatedRulesChecked);
            return updatedRulesChecked;
          });
    };

    const addRule = async () => {
        var ruleTypeAddress = "";
        var body;
        switch (ruleType) {
            case 'Quantity': {ruleTypeAddress = `/Quantity`; 
                            body = JSON.stringify({subject: subject,
                                                    minQuantity: minQuantity,
                                                    maxQuantity: maxQuantity,
                                                }); 
                            break; }
            case 'TotalPrice': {ruleTypeAddress = `/TotalPrice`; 
                                body = JSON.stringify({subject: subject,
                                                        targetPrice: targetPrice,
                                                    }); 
                                break; }
            case 'SimpleRule': {ruleTypeAddress = ``; 
                                body = JSON.stringify({subject: subject}); 
                                break; }
            case 'Composite': {ruleTypeAddress = `/CompositeRule`; 
                body = JSON.stringify({operator: operator, rules: Object.entries(rulesChecked).filter(([key, value]) => value === true).map(([key, value]) => key)}); 
                break; }
        }

        const response = await fetch(`https://localhost:7163/api/Market/Store/${storeId}/AddRule${ruleTypeAddress}?identifier=${getToken()}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: body
        })
        if (response.ok) {
            alert('Rule added successfully');
        } else {
            const responseData = await response.json();
            console.error('Failed to add rule:', responseData);
            alert('Failed to add rule. Please try again later.');
        }
        window.location.reload();
        };

        const validate = () => {
            // Validate the inputs
            let valid = true;
            
            if (ruleType != 'Composite' && subject.trim() === '') {
                setSubjectError('Please enter the subject');
                valid = false;
            }

            switch (ruleType) {
                case 'Quantity': { 
                                    if (minQuantity.trim() === '') {
                                        setMinQuantity('Please enter a minimum quantity');
                                        valid = false;
                                    }
                            
                                    if (maxQuantity.trim() === '') {
                                        setMaxQuantity('Please enter a maximum quantity');
                                        valid = false;
                                    }
                                break; }
                case 'TotalPrice': { 
                                    if (targetPrice.trim() === '') {
                                        setTargetPrice('Please enter a desired total price');
                                        valid = false;
                                    }
                                break; }
            }

            if(valid)
                addRule();
        }
    
        return (
            <Form>
                <Row>
                <Col>
                <Form.Group as={Col} controlId="">
                        <Form.Label>Rule Type</Form.Label>
                        <Form.Select name="ruleType" value={ruleType} onChange={(e) => setRuleType(e.target.value)}>
                        <option value="SimpleRule">Simple Rule</option>
                        <option value="Quantity">Quantity</option>
                        <option value="TotalPrice">Total Price</option>
                        <option value="Composite">Composite Rule</option>
                        </Form.Select>
                    </Form.Group>
                </Col>
                <Col>
                <Form.Group className="mb-3">
                    <Form.Label>Subject</Form.Label>
                    <Form.Control 
                        type="text"
                        value={subject}
                        onChange={(ev) => setSubject(ev.target.value)} 
                        placeholder="Enter Subject" 
                        disabled={ruleType == 'Composite'}
                    />
                    <label className="errorLabel">{subjectError}</label>
                </Form.Group>
                </Col>
                </Row>

                <Row>
                    <Col>
                <Form.Group className="mb-3">
                    <Form.Label>Min Quantity</Form.Label>
                    <Form.Control 
                        type="text"
                        value={minQuantity}
                        onChange={(ev) => setMinQuantity(ev.target.value)} 
                        placeholder="" 
                        disabled={ruleType != 'Quantity'}
                    />
                    { ruleType == 'Quantity' && <label className="errorLabel">{minQuantityError}</label>}
                </Form.Group>
                </Col>
                <Col>
                <Form.Group className="mb-3">
                    <Form.Label>Max Quantity</Form.Label>
                    <Form.Control 
                        type="text"
                        value={maxQuantity}
                        onChange={(ev) => setMaxQuantity(ev.target.value)} 
                        placeholder="" 
                        disabled={ruleType != 'Quantity'}
                    />
                    { ruleType == 'Quantity' && <label className="errorLabel">{maxQuantityError}</label> }
                </Form.Group>
                </Col>
                <Col>
                <Form.Group className="mb-3">
                    <Form.Label>Total Price</Form.Label>
                    <Form.Control 
                        type="text"
                        value={targetPrice}
                        onChange={(ev) => setTargetPrice(ev.target.value)} 
                        placeholder="" 
                        disabled={ruleType != 'TotalPrice'}
                    />
                    {ruleType == 'TotalPrice' && <label className="errorLabel">{targetPriceError}</label> }
                </Form.Group>
                </Col>
                </Row>

                { ruleType == 'Composite' && 
                <Form.Group as={Col} controlId="">
                    <Form.Label>Operator</Form.Label>
                    <Form.Select value={operator} onChange={(e) => setOperator(parseInt(e.target.value))}>
                    <option value="0">Or</option>
                    <option value="1">Xor</option>
                    <option value="2">And</option>
                    </Form.Select>
                </Form.Group>
            }
        { ruleType == 'Composite' && 

                <div key={`checkbox`} className="mb-3">
                    {rules.map((rule) => (
                        <Form.Check
                        key={rule.id}
                        label={`${rule.id}:  ${rule.subjectInfo}`}
                        type="checkbox"
                        checked={ rulesChecked[rule.id] === true} 
                        onChange={(ev) => onCheckClick(ev.target.checked, rule.id)}      
                    /> 
                    ))}
                </div> }
                <Button variant="primary" type="button" onClick={validate}>
                    Add Rule
                </Button>
            </Form>
        );

};
