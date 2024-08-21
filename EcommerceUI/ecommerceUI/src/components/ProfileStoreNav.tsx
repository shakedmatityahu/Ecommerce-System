import React, { useState, useEffect } from "react";
import { Container, Modal, Nav, Dropdown, Button, Col, Stack } from "react-bootstrap";
import { useNavigate } from 'react-router-dom';
import { CreateStore } from './CreateStore';
import ProfileStoreStuff from "./ProfileStoreStuff";
import ProfileStoreInfo from "./ProfileStoreInfo";
import ProfileStoreRules from "./ProfileStoreRules";
import ProfileStorePurchasePolicy from "./ProfileStorePurchasePolicy";
import ProfileStoreDiscountPolicy from "./ProfileStoreDiscountPolicy";
import {ProfileUpdateInventory} from "./ProfileUpdateInventory";
import { getToken } from "../services/SessionService";

export interface Role {
    role: string;
    username: string;
    appointer: string;
    appointees: string[];
    permissions: string[];
}

export interface Product {
    productId: number;             
    storeId: number;               
    productName: string;           
    productPrice: number;          
    productQuantity: number;       
    // productCategory: string;       
    productDescription: string;    
    // productKeywords: string[];     
    // productRating: number;         
    // ageLimit: boolean;             
    // sellMethod: string;           
}


interface Store {
    storeName: string | undefined;
    storeId: number | undefined;
    storeActive: boolean | undefined;
    storeEmailAdd: string | undefined;
    storePhoneNum: string | undefined;
    storeRaiting: number | undefined;
    roles: Role[]; // Add the roles property here
    products: Product[];
}


export const ProfileStoreNav = () => {
    const [stores, setStores] = useState<number[]>([]);
    const [num, setNum] = useState<number>(-1); // Initialize num to -1
    const [showCreateStoreModal, setShowCreateStoreModal] = useState(false);
    const [view, setView] = useState<'ProfileStoreStuff' | 'ProfileStoreInfo'| 'ProfileUpdateInventory' | 'Rules' | 'PPolicy' | 'DPolicy'>('ProfileStoreInfo');
    const [storeInfo, setStoreInfo] = useState<Store | null>(null);
    const mappedProducts: Product[] = [];

    const handleClose = () => setShowCreateStoreModal(false);
    const handleSuccess = () => {
        window.location.reload();

    };
    const handleViewChange = (newView: 'ProfileStoreStuff' | 'ProfileStoreInfo'| 'ProfileUpdateInventory' | 'Rules' | 'PPolicy' | 'DPolicy') => {
         setView(newView);
        // Fetch store info whenever the view changes
        if (num !== -1) {
            fetchStoreInfo(num);
        }
    };

    useEffect(() => {
        // Fetch store information when the component mounts or when the store ID changes
        fetchStores();
    }, [num]);

    const fetchStoreInfo = async (num : number) => {
        try {
            const response = await fetch(`https://localhost:7163/api/Client/Client/Stores/${num}/?identifier=${getToken()}`);
            if (response.ok) {
                const { value } = await response.json(); // Destructure value from the response
                const { storeName, storeId, active: storeActive, storeEmailAdd, storePhoneNum, rating: storeRaiting, roles, products } = value;
                const mappedProducts: Product[] = products.map((product: any) => ({
                    productId: product.id,
                    storeId: product.storeId,
                    productName: product.name,
                    productPrice: product.price,
                    productDescription: product.description,
                    productQuantity: product.quantity,
                }));
                setStoreInfo({ storeName, storeId, storeActive, storeEmailAdd, storePhoneNum, storeRaiting, roles, products: mappedProducts});
            } else {
                throw new Error('Failed to fetch store information');
            }
        } catch (error) {
            console.error('Error fetching store information:', error);
        }
    };

    const fetchStores = async () => {
        try {
            const response = await fetch(`https://localhost:7163/api/Client/Client/Stores/?identifier=${getToken()}`);
            if (response.ok) {
                const { value } = await response.json();
                const storeIds = value.map((store : Store) => store.storeId); // Extracting store IDs from the response
                setStores(storeIds);
            } else {
                throw new Error('Failed to fetch store information');
            }
        } catch (error) {
            console.error('Error fetching store information:', error);
        }
    };
    
    return (
        <>
            <Container className="d-flex justify-content-between align-items-center my-3">
            <Nav variant="tabs" defaultActiveKey="/home">
                {stores.map(storeId => (
                    <Nav.Item key={storeId}>
                    <Nav.Link onClick={() => { setNum(storeId); fetchStoreInfo(storeId); }}>Store {storeId}</Nav.Link> 
    </Nav.Item>
                ))}
            </Nav>
                <Stack direction="horizontal" gap={3}>
                    <Button variant="primary" onClick={() => setShowCreateStoreModal(true)}>Create Store</Button>
                    <Dropdown className="ml-2">
                        <Dropdown.Toggle variant="secondary" id="dropdown-basic">
                            Options
                        </Dropdown.Toggle>
                        <Dropdown.Menu>
                            <Dropdown.Item onClick={() => handleViewChange('ProfileStoreInfo')}>Store Info</Dropdown.Item>
                            <Dropdown.Item onClick={() => handleViewChange('ProfileStoreStuff')}>Store Permission</Dropdown.Item>
                            <Dropdown.Item onClick={() => handleViewChange('ProfileUpdateInventory')}>Update Inventory</Dropdown.Item>
                            <Dropdown.Item onClick={() => handleViewChange('Rules')}>Rules</Dropdown.Item>
                            <Dropdown.Item onClick={() => handleViewChange('PPolicy')}>Purchase Policies</Dropdown.Item>
                            <Dropdown.Item onClick={() => handleViewChange('DPolicy')}>Discount Policies</Dropdown.Item>
                        </Dropdown.Menu>
                    </Dropdown>
                </Stack>
                
            </Container>
            {/* {num !== -1 && ( // Render the following only if num is not -1
                <Container className="d-flex justify-content-between align-items-center my-3">
                    <p className="mb-0">Store {num}</p>
                </Container>
            )} */}
            <Col className="profile-right">
            {storeInfo && (
            <>
                {view === 'ProfileStoreInfo' && (
                    <ProfileStoreInfo 
                        storeName={storeInfo.storeName}
                        storeId={storeInfo.storeId}
                        storeActive={storeInfo.storeActive}
                        storeEmailAdd={storeInfo.storeEmailAdd}
                        storePhoneNum={storeInfo.storePhoneNum}
                        storeRaiting={storeInfo.storeRaiting}
                    />
                )} 
                {view === 'ProfileStoreStuff' && (
                    <ProfileStoreStuff roles={storeInfo.roles} storeId={num} />
                )}
                 {view === 'ProfileUpdateInventory' && (
                    <ProfileUpdateInventory storeId={storeInfo.storeId} products={storeInfo.products} />
                )}
                {view === 'Rules' && (
                    <ProfileStoreRules storeId={storeInfo.storeId} />
                )}
                {view === 'PPolicy' && (
                    <ProfileStorePurchasePolicy storeId={storeInfo.storeId} />
                )}
                {view === 'DPolicy' && (
                    <ProfileStoreDiscountPolicy storeId={storeInfo.storeId} />
                )}
            </>
        )}
            </Col>
            <Modal show={showCreateStoreModal} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Add Store</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CreateStore onClose={handleClose} onSuccess={handleSuccess} />
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

export default ProfileStoreNav;
function setStores(storeIds: any) {
    throw new Error("Function not implemented.");
}

