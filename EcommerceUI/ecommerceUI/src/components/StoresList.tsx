import React, {Component, useState, useEffect} from "react";
import { Table, Dropdown, Form, Stack, Button, Card, Row, Col, Container, Modal } from 'react-bootstrap';
import ItemCard from "./ItemCard";
import { getToken } from "../services/SessionService";

export const StoresList = () => {
    const [dataValue, setDataValue] = useState<any[]>([]); 
    const [storeView, setStoreView] = useState<{}>({}); 
    const [showStoreInfoModal, setShowStoreInfoModal] = useState(false);

    const handleClose = () => setShowStoreInfoModal(false);
    const choseStore = (storeData) => {setShowStoreInfoModal(true); setStoreView(storeData);}

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(`https://localhost:7163/api/Market/Stores`, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    throw new Error('Error occurred in search');
                }

                const data = await response.json();
                setDataValue(data.value);
            } catch (error) {
                console.error('Error:', error);
            }
        };

        fetchData();
    }, []);

    return(
        <>
            <Container className="flex small-padding">
                { <Row xs={1} md={Math.min(dataValue.length, 4)} className="g-4">
                {dataValue.map((store) => (
                    <Col key={store.storeName}>
                        <Card style={{ width: '18rem' }}>
                        <Card.Body>
                            <Card.Title>{store.storeName}</Card.Title>
                            <Card.Text> Store ID: {store.storeId} </Card.Text>
                            <Card.Text> Contact Number: {store.storePhoneNum} </Card.Text>
                            <Card.Text> Contact Email: {store.storeEmailAdd} </Card.Text>
                            {!store.active && <Card.Text style={{color: 'red'}}> store is not active </Card.Text>}
                            <Button variant="primary" onClick={() => choseStore(store)}>View</Button>
                        </Card.Body>
                        </Card>
                    </Col>
                ))}
              </Row> }
            </Container>
            <Modal show={showStoreInfoModal} onHide={handleClose} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>{storeView.storeName} Information </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <StoreView onClose={handleClose} storeInfo={storeView}/>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
    
}

export const StoreView = ({onClose, storeInfo}) => {
    const [dataValue, setDataValue] = useState<any[]>([]); 

    return(
        <>
            <h4> Inventory </h4>
            <Container className="flex small-padding">
                { <Row xs={2} md={Math.min(dataValue.length, 2)} className="g-4">
                {(storeInfo.products).map((product) => (
                    <Col key={product.name}>
                        <Card style={{ width: '20rem' }}>
                        <Card.Body>
                            <Card.Title>{product.name}</Card.Title>
                            <Card.Text> Description: {product.description} </Card.Text>
                            <Card.Text> Price: {product.price}₪ </Card.Text>
                            <Card.Text> Quantity: {product.quantity} </Card.Text>
                        </Card.Body>
                        </Card>
                    </Col>                    
                ))}
              </Row> }
            </Container>
        </>
    );
    
}


export default StoresList;
