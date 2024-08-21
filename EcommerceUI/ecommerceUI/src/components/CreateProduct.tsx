import React, { useState } from 'react';
import { Form, Button, Row, Col } from 'react-bootstrap';
import { getToken } from '../services/SessionService';


export const CreateProduct = ({ onClose, onSuccess, storeId }: { onClose: any, onSuccess: any, storeId: any }) => {
    const [productName, setProductName] = useState('');
    const [sellMethod, setSellMethod] = useState('RegularSell');
    const [productDescription, setProductDescription] = useState('');
    const [price, setPrice] = useState('');
    const [category, setCategory] = useState('All');
    const [quantity, setQuantity] = useState('');
    const [ageLimit, setAgeLimit] = useState(true);

    const [productNameError, setProductNameError] = useState('');
    const [productDescriptionError, setProductDescriptionError] = useState('');
    const [priceError, setPriceError] = useState('');
    const [categoryError, setCategoryError] = useState('');
    const [quantityError, setQuantityError] = useState('');
    
    const addProduct = async () => {
        const response= await fetch(`https://localhost:7163/api/Market/Store/${storeId}/Products/Add?identifier=${getToken()}`, {
           method: 'POST',
           headers: {
               'Content-Type': 'application/json',
           },
           body: JSON.stringify(
            {   
                storeId: storeId,
                productName: productName,
                sellMethod: sellMethod,
                productDescription: productDescription,
                price: price,
                category: category,
                quantity: quantity,
                ageLimit: ageLimit
           })  
       })
       if (response.ok) {
           alert('Item added successfully');
       } else {
           const responseData = await response.json();
           console.error('Failed to added item:', responseData);
           alert('Failed to added item. Please try again later.');
       }
       window.location.reload();
    };

    const validate = () => {
        // Validate the inputs
        let valid = true;

        if (productName.trim() === '') {
            setProductNameError('Please enter the product name');
            valid = false;
        }

        if (productDescription.trim() === '') {
            setProductDescriptionError('Please enter a description');
            valid = false;
        }

        if (price.trim() === '') {
            setPriceError('Please enter a price');
            valid = false;
        }

        if (category.trim() === '') {
            setCategoryError('Please enter a price');
            valid = false;
        }

        if (quantity.trim() === '') {
            setQuantityError('Please enter a quantity');
            valid = false;
        }

        if (valid) {
            addProduct();
        }
    }
    return (
        <Form>
            <Form.Group className="mb-3">
                <Form.Label>Name</Form.Label>
                <Form.Control 
                    type="text"
                    value={productName}
                    onChange={(ev) => setProductName(ev.target.value)} 
                    placeholder="Enter product name" 
                />
                <label className="errorLabel">{productNameError}</label>
            </Form.Group>
            <Form.Group className="mb-3">
                <Form.Label>Description</Form.Label>
                <Form.Control 
                    type="text"
                    value={productDescription}
                    onChange={(ev) => setProductDescription(ev.target.value)} 
                    placeholder="Enter product description" 
                />
                <label className="errorLabel">{productDescriptionError}</label>
            </Form.Group>
            <Row>
            <Col>
            <Form.Group className="mb-3">
                <Form.Label>Category</Form.Label>
                <Form.Control 
                    type="text"
                    value={category}
                    onChange={(ev) => setCategory(ev.target.value)} 
                    placeholder="Category" 
                />
                <label className="errorLabel">{categoryError}</label>
            </Form.Group>
            </Col>
            <Col>
            <Form.Group as={Col} controlId="">
                        <Form.Label>Sell Method</Form.Label>
                        <Form.Select name="sellMethod" value={sellMethod} onChange={(e) => setSellMethod(e.target.value)}>
                        <option value="RegularSell">Regular</option>
                        <option value="BidSell">Bid</option>
                        <option value="AuctionSell">Auction</option>
                        <option value="LotterySell">Lottery</option>
                        </Form.Select>
            </Form.Group>
            </Col>
            </Row>
            <Row>
                <Col>
            <Form.Group className="mb-3">
                <Form.Label>Price</Form.Label>
                <Form.Control 
                    type="text"
                    value={price}
                    onChange={(ev) => setPrice(ev.target.value)} 
                    placeholder="Enter product price" 
                />
                <label className="errorLabel">{priceError}</label>
            </Form.Group>
            </Col>
            <Col>
            <Form.Group className="mb-3">
                <Form.Label>Quantity</Form.Label>
                <Form.Control 
                    type="text"
                    value={quantity}
                    onChange={(ev) => setQuantity(ev.target.value)} 
                    placeholder="Enter product quantity" 
                />
                <label className="errorLabel">{quantityError}</label>
            </Form.Group>
            </Col>
            <Col>
            <Form.Group className="mb-3" id="" style={{paddingTop:'3vh'}}>
                <Form.Check type="checkbox" label="Age Limit" checked={ageLimit} onChange={(ev) => setAgeLimit(ev.target.checked)}/>
            </Form.Group>
            </Col>
            </Row>
            
            <Button variant="primary" type="button" onClick={validate}>
                Add Product
            </Button>
        </Form>
    );
};

