import React, {Component, useState} from "react";
import Button from 'react-bootstrap/Button';
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import { useNavigate } from "react-router-dom";
import { getToken, getLoggedIn, getSessionId } from "../services/SessionService";


interface FormData {
    storeId: number;
    email: string;
    phoneNumber: string;
    address: string;
    country: string;
    city: string;
    zipCode: string;
    holderId: string;
    cardHolder: string;
    cardNumber: string;
    expMonth: string;
    expYear: string;
    cvv: string;
    currency: string;
  }
  
  interface Errors {
    email?: string;
    phoneNumber?: string;
    address?: string;
    country?: string;
    city?: string;
    zipCode?: string;
    holderId?: string;
    cardHolder?: string;
    cardNumber?: string;
    expMonth?: string;
    expYear?: string;
    cvv?: string;
    currency?: string;
  }

export const Purchase = () => {
    const [formData, setFormData] = useState<FormData>({
        storeId: 0,
        email: '',
        phoneNumber: '',
        address: '',
        country: '',
        city: '',
        zipCode: '',
        holderId: '',
        cardHolder: '',
        cardNumber: '',
        expMonth: '',
        expYear: '',
        cvv: '',
        currency: ''
    });

    const [errors, setErrors] = useState<Errors>({});
    const navigate = useNavigate();
    const handleChange = (e: any) => {
        const { name, value } = e.target;
        setFormData({
        ...formData,
        [name]: value,
        });
    };

    const validate = (): Errors => {
        let formErrors: Errors = {};
        if (!formData.phoneNumber) formErrors.phoneNumber = 'Phone Number is required';
        if (!formData.email) formErrors.email = 'Email is required';
        else if (!/\S+@\S+\.\S+/.test(formData.email))
          formErrors.email = 'Email is invalid';
        if (!formData.address) formErrors.address = 'address is required';
        if (!formData.holderId) formErrors.holderId = 'id is required';
        else if (formData.holderId.length < 9)
          formErrors.holderId = 'id must be at least 9 characters';
        if (!formData.cardHolder) formErrors.cardHolder = 'Name is required';
        if (!formData.cardNumber) formErrors.cardNumber = 'Card Number is required';
        else if (formData.cardNumber.length < 16)
          formErrors.cardNumber = 'Card Number must be at least 16 characters';
        if (!formData.expMonth) formErrors.expMonth = 'Expiration Month is required';
        if (!formData.expYear) formErrors.expYear = 'Expiration Year is required';
        if (formData.cvv.length != 3)
          formErrors.cvv = 'cvv must be 3 characters';
        if (!formData.currency) formErrors.currency = 'Currency is required';
        console.log(formErrors)
        return formErrors;
      };
    
      const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        console.log("handleSubmit in purchase");
        console.log(formData);
        e.preventDefault();
        const formErrors = validate();
        if (Object.keys(formErrors).length === 0) {
            console.log("trying to fetch purchase");
          try {
            var identifier = getLoggedIn() ? getToken() : getSessionId();
            const response = await fetch(
              `https://localhost:7163/api/Market/Purchase?identifier=${identifier}`,
              {
                method: 'POST',
                headers: {
                  'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
              }
            );
    
            if (response.ok) {
              console.log('Purchase successful');
              alert('Purchase completed successfully');
              // Reset form data if needed
            //   setFormData({ username: '', email: '', password: '', age: '' });
            //   navigate('/Login');
            navigate('/');

            } else {
              // Handle error response
              const responseData = await response.json();
              console.error('Purchase failed:', responseData);
              // Optionally, you can set specific errors based on the response
              alert('Purchase failed. Please try again later.');
            }
          } catch (error) {
            console.error('Error occurred while registering:', error);
            // Handle network errors or other exceptions
            alert('An error occurred while processing your request. Please try again later.');
          }
        } else {
          setErrors(formErrors);
        }
      };
    
    return(
        <>
            <Form className="half-width small-padding" onSubmit={handleSubmit}>
                <Row> <Col> <p><b> Delivery Information </b> </p> </Col> </Row>
                <Row className="mb-3">
                    <Form.Group as={Col}>
                    <Form.Label>Email</Form.Label>
                    <Form.Control type="text" 
                        name="email"
                        placeholder="Enter email" 
                        value={formData.email} 
                        onChange={handleChange} />
                    {errors.email && <p>{errors.email}</p>}
                    </Form.Group>

                    <Form.Group as={Col}>
                    <Form.Label>Phone Number</Form.Label>
                    <Form.Control type="text" 
                        name="phoneNumber"
                        placeholder="Enter phone number" 
                        value={formData.phoneNumber} 
                        onChange={handleChange} />
                    {errors.phoneNumber && <p>{errors.phoneNumber}</p>}
                    </Form.Group>
                </Row>

                <Form.Group as={Col}>
                    <Form.Label>Address</Form.Label>
                    <Form.Control type="text" 
                        name="address"
                        placeholder="1234 Main St" 
                        value={formData.address} 
                        onChange={handleChange} />
                    {errors.address && <p>{errors.address}</p>}
                </Form.Group>

                <Row className="mb-3">
                    <Form.Group as={Col}>
                        <Form.Label>Country</Form.Label>
                        <Form.Control type="text" 
                            name="country"
                            placeholder="" 
                            value={formData.country} 
                            onChange={handleChange} />
                        {errors.country && <p>{errors.country}</p>}
                    </Form.Group>

                    <Form.Group as={Col}>
                        <Form.Label>City</Form.Label>
                        <Form.Control type="text" 
                            name="city"
                            placeholder="" 
                            value={formData.city} 
                            onChange={handleChange} />
                        {errors.city && <p>{errors.city}</p>}
                    </Form.Group>

                    

                    <Form.Group as={Col}>
                        <Form.Label>ZIP code</Form.Label>
                        <Form.Control type="text" 
                            name="zipCode"
                            placeholder="" 
                            value={formData.zipCode} 
                            onChange={handleChange} />
                        {errors.zipCode && <p>{errors.zipCode}</p>}
                    </Form.Group>
                </Row>

                <Row> <Col> <p><b> Billing Information </b></p> </Col> </Row>

                <Row className="mb-3">
                    <Form.Group as={Col}>
                        <Form.Label>ID</Form.Label>
                        <Form.Control type="text" 
                            name="holderId"
                            placeholder="" 
                            value={formData.holderId} 
                            onChange={handleChange} />
                        {errors.holderId && <p>{errors.holderId}</p>}
                    </Form.Group>

                    <Form.Group as={Col}>
                        <Form.Label>Name</Form.Label>
                        <Form.Control type="text" 
                            name="cardHolder"
                            placeholder="" 
                            value={formData.cardHolder} 
                            onChange={handleChange} />
                        {errors.cardHolder && <p>{errors.cardHolder}</p>}
                    </Form.Group>

                </Row>

                    <Form.Group as={Col}>
                        <Form.Label>Card Number</Form.Label>
                        <Form.Control type="text" 
                            name="cardNumber"
                            placeholder="XXXX-XXXX-XXXX-XXXX" 
                            value={formData.cardNumber} 
                            onChange={handleChange} />
                        {errors.cardNumber && <p>{errors.cardNumber}</p>}
                    </Form.Group>

                <Row className="mb-3">
                <Form.Group as={Col} controlId="">
                        <Form.Label>Expiration Month</Form.Label>
                        <Form.Select name="expMonth" value={formData.expMonth} onChange={handleChange}>
                        <option value="">Month...</option>
                        <option value="1">1</option>
                        <option value="2">2</option>
                        <option value="3">3</option>
                        <option value="4">4</option>
                        <option value="5">5</option>
                        <option value="6">6</option>
                        <option value="7">7</option>
                        <option value="8">8</option>
                        <option value="9">9</option>
                        <option value="10">10</option>
                        <option value="11">11</option>
                        <option value="12">12</option>
                        </Form.Select>
                        {errors.expMonth && <p>{errors.expMonth}</p>}

                    </Form.Group>

                    <Form.Group as={Col} controlId="">
                        <Form.Label>Expiration Year</Form.Label>
                        <Form.Select name="expYear" value={formData.expYear} onChange={handleChange}>
                        <option value="">Year...</option>
                        <option value="2024">2024</option>
                        <option value="2025">2025</option>
                        <option value="2026">2026</option>
                        <option value="2027">2027</option>
                        <option value="2028">2028</option>
                        <option value="2029">2029</option>
                        <option value="2030">2030</option>
                        <option value="2031">2031</option>
                        <option value="2032">2032</option>
                        <option value="2033">2033</option>
                        </Form.Select>
                        {errors.expYear && <p>{errors.expYear}</p>}

                    </Form.Group>

                    

                    <Form.Group as={Col}>
                        <Form.Label>CVV</Form.Label>
                        <Form.Control type="text" 
                            name="cvv"
                            placeholder="XXX" 
                            value={formData.cvv} 
                            onChange={handleChange} />
                        {errors.cvv && <p>{errors.cvv}</p>}
                    </Form.Group>

                    <Form.Group as={Col} controlId="">
                        <Form.Label> Currency</Form.Label>
                        <Form.Select name="currency" value={formData.currency} onChange={handleChange}>
                        <option value="">Currency...</option>
                        <option value="ILS">ILS ₪</option>
                        <option value="USD">USD $</option>
                        <option value="EURO">ERUO €</option>
                        <option value="GBP">GBP £</option>
                        </Form.Select>
                        {errors.currency && <p>{errors.currency}</p>}

                    </Form.Group>
                </Row>

                <Button variant="primary" type="submit">
                    Submit
                </Button>
            </Form>
        </>
    );
};

export default Purchase;