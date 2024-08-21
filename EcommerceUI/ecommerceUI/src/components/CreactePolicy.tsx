import React, { useState, useEffect } from 'react';
import { Form, Button, Row, Col } from 'react-bootstrap';
import { getToken } from '../services/SessionService';
import 'react-datepicker/dist/react-datepicker.css';
import DatePicker from 'react-datepicker';


export const CreatePolicy = ({ onClose, onSuccess, storeId, defPolicyType }: { onClose: any, onSuccess: any, storeId: any, defPolicyType: string }) => {
    const [rules, setRules] = useState<any[]>([]);
    const [policyType, setPolicyType] = useState(defPolicyType);
    const [operator, setOperator] = useState(0);
    const [policies, setPolicies] = useState<any[]>([]);

    
    const [ruleString, setRuleString] = useState('');
    const [rule, setRule] = useState({
        "id": 0,
        "subjectInfo": ""
      });
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);
    const [percentage, setPercentage] = useState(0);

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

        const fetchPoliciesList = async () => {
            // var purchase;
            var discount;
            // try {
            //     const response = await fetch(`https://localhost:7163/api/Market/Store/${storeId}/Policies/Purchace?identifier=${getToken()}`, {
            //         method: 'GET'
            //     });
            //     const data = await response.json();
            //     if (response.ok) {
            //         purchase = data.value;
            //     } else {
            //         console.error('Error fetching purchase policies:', data.ErrorMessage);
            //     }
            // } catch (error) {
            //     console.error('Error occurred while fetching purchase policies:', error);
            // }

            try {
                const response = await fetch(`https://localhost:7163/api/Market/Store/${storeId}/Policies/Discount?identifier=${getToken()}`, {
                    method: 'GET'
                });
                const data = await response.json();
                if (response.ok) {
                    discount = data.value;
                } else {
                    console.error('Error fetching purchase policies:', data.ErrorMessage);
                }
            } catch (error) {
                console.error('Error occurred while fetching purchase policies:', error);
            }
            // const unionArray = Array.from(new Set([...purchase, ...discount]));
            setPolicies(discount);
        };

        fetchRulesList();
        fetchPoliciesList();
    }, []); 

    const [policiesChecked, setPoliciesChecked] = useState<{[key: number]: boolean}>(
        () => {
            return policies.reduce((acc, policy) => {
                acc[policy.id] = false;
                return acc;
              }, {});
        }
    );


    const addPolicy = async () => {
        var body;
        switch (policyType) {
            case 'Purchace': {
                            body = JSON.stringify({expirationDate: selectedDate,
                                                    subject: rule.subjectInfo,
                                                    ruleId: rule.id
                                                }); 
                            break; }
            case 'Discount': {
                                body = JSON.stringify({expirationDate: selectedDate, subject: rule.subjectInfo, ruleId: rule.id, Precantage: percentage
                                                    }); 
                                break; }
            case 'Composite': {
                body = JSON.stringify({operator: operator, expirationDate: selectedDate, subject: rule.subjectInfo , policies: Object.entries(policiesChecked).filter(([key, value]) => value === true).map(([key, value]) => key)}); 

                break; }
        }

        const response = await fetch(`https://localhost:7163/api/Market/Store/${storeId}/Policies/${policyType}?identifier=${getToken()}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: body
        })
        if (response.ok) {
            alert('Policy added successfully');
        } else {
            const responseData = await response.json();
            console.error('Failed to add Policy:', responseData);
            alert('Failed to add Policy. Please try again later.');
        }
        window.location.reload();
    }        

    const onCheckClick = (checked : boolean, ruleId : number) => {
        setPoliciesChecked(prevPoliciesChecked => {
            // Create a copy of previous state
            const updatedRulesChecked = {
              ...prevPoliciesChecked,
              [ruleId]: checked,
            };
            // Log the updated state (optional for debugging)
            return updatedRulesChecked;
          });
    };


        return (
            <Form>
                <Row> 
                    <Col>
                <Form.Group as={Col} controlId="">
                    <Form.Label>Policy Type</Form.Label>
                    <Form.Select name="ruleType" value={policyType} onChange={(e) => setPolicyType(e.target.value)}>
                    <option value="Purchace">Purchase</option>
                    <option value="Discount">Discount</option>
                    <option value="Composite">Composite</option>
                    </Form.Select>
                </Form.Group>
                </Col>
                <Col>
                <Form.Group>
                        <Form.Label>Select Date</Form.Label>
                        <DatePicker
                            selected={selectedDate}
                            onChange={(date) => setSelectedDate(date)}
                            className="form-control"
                        />
                    </Form.Group>
                </Col>
                </Row>

                <Form.Group as={Col} controlId="">
                        <Form.Label>Rule</Form.Label>
                        <Form.Select name="ruleType" value={ruleString} onChange={(e) => {
                                const [id, subjectInfo] = e.target.value.split(',');
                                setRule({ id: parseInt(id), subjectInfo });
                                setRuleString(e.target.value);
                            }}>
                        <option> choose... </option>
                        {rules.map((rule) => (
                            <option value={`${rule.id},${rule.subjectInfo}`}>{rule.id}: {rule.subjectInfo}</option>
                    ))}
                        </Form.Select>
                    </Form.Group>
                    
                    <Form.Group>
                        <Form.Label>Percentage</Form.Label>
                        <Form.Control
                            type="number"
                            value={percentage}
                            onChange={(e) => { console.log(e.target.value); setPercentage(parseInt(e.target.value))}}
                            className="form-control"
                            disabled={policyType != 'Discount'}
                        />
                    </Form.Group>

                    { policyType == 'Composite' && 
                <Form.Group as={Col} controlId="">
                    <Form.Label>Operator</Form.Label>
                    <Form.Select value={operator} onChange={(e) => setOperator(parseInt(e.target.value))}>
                    <option value="0">Add</option>
                    <option value="1">Max</option>
                    </Form.Select>
                </Form.Group>
            }
            { policyType == 'Composite' && 

                <div key={`checkbox`} className="mb-3">
                    {policies.map((policy) => (
                        <Form.Check
                        key={policy.id}
                        label={`${policy.id}:  ${policy.rule.subjectInfo} ${(policy.precentage == undefined) ? '(Purchase)' : '(Discount)' }`}
                        type="checkbox"
                        checked={ policiesChecked[policy.id] === true} 
                        onChange={(ev) => onCheckClick(ev.target.checked, policy.id)}      
                    /> 
                    ))}
                </div> }


                <Button variant="primary" type="button" onClick={addPolicy}>
                    Add Policy
                </Button>
            </Form>
        );

};
