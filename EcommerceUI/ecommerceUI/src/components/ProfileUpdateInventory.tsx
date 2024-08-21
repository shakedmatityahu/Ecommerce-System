import React, { useState } from 'react';
import { Table, Dropdown, Form, Stack, Button, Card, Row, Col, Container, Modal } from 'react-bootstrap';
import { Product } from './ProfileStoreNav'
import { RiSave2Fill,  RiDeleteBin2Fill, RiAddBoxFill, RiPriceTag3Fill } from 'react-icons/ri'; // Importing the shopping cart icon
import { getToken } from '../services/SessionService';
import { CreateProduct } from './CreateProduct';


interface ProfileUpdateInventoryProps {
    products: Product[];
    storeId: number | undefined;
}

export const ProfileUpdateInventory: React.FC<ProfileUpdateInventoryProps> = ({ products , storeId}) => {
    const [showAddProductModal, setShowAddProductModal] = useState(false);

    const handleClose = () => setShowAddProductModal(false);
    const handleSuccess = () => {
        window.location.reload();

    };

    return (
        <>
                <h2>Update Inventory</h2>
            <Container className="flex small-padding">
            { <Row  md={Math.min(products.length + 1, 3)} className="vertical-center">
                
                {products.map((product, index) => (
                    <Col>
                        <ProductDetails key={index} product={product} />
                    </Col>
                ))}
                <Col style={{ width: '23rem', paddingTop: '10vh'}}>
                    <Button variant="outline-success" onClick={() => setShowAddProductModal(true)}> <RiAddBoxFill size={20} /></Button>
                </Col>
            </Row> }
            </Container>

            <Modal show={showAddProductModal} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Add Product</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CreateProduct onClose={handleClose} onSuccess={handleSuccess} storeId={storeId}/>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

interface ProductDetailsProps {
    product: Product;
}

const ProductDetails: React.FC<ProductDetailsProps> = ({ product }) => {
    const [productPrice, setProductPrice] = useState(product.productPrice);
    const [productQuantity, setProductQuantity] = useState(product.productQuantity);
    const [keyword, setKeyword] = useState('');
    const [showAddKeywordModal, setShowAddKeywordModal] = useState(false);

    const handlePriceChange = (e : any) => {
        setProductPrice(e.target.value);
    }

    const handleQuantityChange = (e : any) => {
        setProductQuantity(e.target.value);
    }

    const removeProduct = async () => {
        const response= await fetch(`https://localhost:7163/api/Market/Store/${product.storeId}/Products/remove?identifier=${getToken()}`, {
           method: 'POST',
           headers: {
               'Content-Type': 'application/json',
           },
           body: JSON.stringify(
            {   storeId: product.storeId,
                id: product.productId,
                productName: product.productName,
                productDescription: product.productDescription,
                price: productPrice,
                quantity: productQuantity,
           })  
       })
       if (response.ok) {
           alert('Item deleted successfully');
       } else {
           const responseData = await response.json();
           console.error('Failed to deleted item:', responseData);
           alert('Failed to deleted item. Please try again later.');
       }
       window.location.reload();
   };

   

   const updateProduct = async () => {
    const response= await fetch(`https://localhost:7163/api/Market/Store/${product.storeId}/Products/${product.productId}?identifier=${getToken()}`, {
       method: 'PUT',
       headers: {
           'Content-Type': 'application/json',
       },
       body: JSON.stringify(
        {   storeId: product.storeId,
            id: product.productId,
            productName: product.productName,
            productDescription: product.productDescription,
            price: productPrice,
            quantity: productQuantity,
       })  
   })
   if (response.ok) {
       alert('Item updated successfully');
   } else {
       const responseData = await response.json();
       console.error('Failed to update item:', responseData);
       alert('Failed to update item. Please try again later.');
   }
};

const addKeyword = async () => {
    const response= await fetch(`https://localhost:7163/api/Market/Store/${product.storeId}/Product/${product.productId}/KeyWord?identifier=${getToken()}&keyWord=${keyword}`, {
       method: 'POST',
       headers: {
           'Content-Type': 'application/json',
       },
   })
   if (response.ok) {
       alert('Keyword successfully');
   } else {
       const responseData = await response.json();
       console.error('Failed to add keyword:', responseData);
       alert('Failed to add keyword. Please try again later.');
   }
};



    return (
        <>
            <Card style={{ width: '23rem' }}>
            <Card.Body>
            <Stack direction="horizontal" gap={3}>
                <Button variant="outline-danger" onClick={removeProduct}> <RiDeleteBin2Fill size={20} /></Button>
                <Card.Title><b>{product.productName} </b></Card.Title>
            </Stack>
            <br></br>
            <Card.Text> {product.productDescription} </Card.Text>
            <Stack direction="horizontal" gap={3}>
                <Container>
                    <Row>
                        <Col> Price </Col>
                        <Col> 
                            <Form.Control
                                type="number"
                                className="mx-2"
                                value={productPrice}
                                onChange={handlePriceChange}
                                min="0"
                                style={{ width: '7vw', textAlign: 'center' }}/> 
                        </Col>
                        <Col> 
                            <Button variant="outline-secondary" onClick={updateProduct}> <RiSave2Fill size={20} /></Button>
                        </Col>
                    </Row>
                    <Row className="small-padding">
                        <Col> Quantity </Col>
                        <Col>                     
                        <Form.Control
                            type="number"
                            className="mx-2"
                            value={productQuantity}
                            onChange={handleQuantityChange}
                            min="0"
                            style={{ width: '7vw', textAlign: 'center' }} /> 
                        </Col>
                        <Col> 
                            <Button variant="outline-secondary" onClick={() => setShowAddKeywordModal(true)}> <RiPriceTag3Fill size={20} /></Button>
                        </Col>
                    </Row>
                </Container>

            </Stack>
            <br></br>
            </Card.Body>
            </Card>
            <Modal show={showAddKeywordModal} onHide={() => setShowAddKeywordModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Add Keyword</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form.Control
                                type="text"
                                className="mx-2"
                                value={keyword}
                                placeholder="Enter Keyword"
                                onChange={(e) => setKeyword(e.target.value)}
                                min="0"
                                 /> 
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={addKeyword}>
                        Save
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};
