import React, {Component, useState} from "react";
import Button from 'react-bootstrap/Button';
import Container from 'react-bootstrap/Container';
import Form from 'react-bootstrap/Form';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { RiShoppingCart2Line, RiUserLine, RiStore2Fill} from 'react-icons/ri'; // Importing the shopping cart icon
import { useNavigate } from 'react-router-dom';
import SplitButton from 'react-bootstrap/SplitButton';
import Dropdown from 'react-bootstrap/Dropdown';
import {getLoggedIn} from '../services/SessionService';


interface NavBarProps {
  loggedIn: boolean;
  setLoggedIn: (loggedIn: boolean) => void;
}

export const NavBar: React.FC<NavBarProps> = (props:{loggedIn : boolean, setLoggedIn: (arg0: boolean) => void; }) => {
  const [query, setQuery] = useState('');
    const [selectedFilter, setSelectedFilter] = useState('name');
    const navigate = useNavigate();

    // Function to handle the event when an option is clicked
    const handleFilterClick = (option: string) => {
      // Update the selected option
      setSelectedFilter(option);
    };

    const onSearchClick = (event: React.FormEvent<HTMLFormElement>) => {
      console.log("onSearchClick");
      event.preventDefault();
      if (query.trim()) {
        navigate(`/search?query=${encodeURIComponent(query)}&filter=${encodeURIComponent(selectedFilter)}`);
      }
    };

    return (
        <Navbar expand="lg" className="bg-body-tertiary" fixed="top">
          <Container fluid>
          <RiShoppingCart2Line size={30} />
            <Navbar.Brand href="/">Ecommerce</Navbar.Brand>
            <Navbar.Toggle aria-controls="navbarScroll" />
            <Navbar.Collapse id="navbarScroll">

              {/* ---- Navigation Linkes ---- */}
              <Nav
                className="me-auto my-2 my-lg-0"
                style={{ maxHeight: '100px' }}
                navbarScroll
              >
                <Nav.Link href="/home">Home</Nav.Link>
              </Nav>

              {/* ---- Search ---- */}
              <Form className="d-flex mx-auto" onSubmit={onSearchClick}>
                <Button variant="outline-success" className="me-2" onClick={() => navigate('/stores')}> <RiStore2Fill size={20} /> </Button>
                <Form.Control
                  type="search"
                  placeholder={`Search by ${selectedFilter}`}
                  className="me-2"
                  aria-label="Search"
                  value={query}
                  onChange={(ev) => setQuery(ev.target.value)} />
                    <SplitButton
                      key="search"
                      variant="outline-success"
                      type="submit"
                      title="search">
                      <Dropdown.Item eventKey="1" active={selectedFilter == "name"} onClick={() => handleFilterClick("name")}> name</Dropdown.Item>
                      <Dropdown.Item eventKey="2" active={selectedFilter == "category"} onClick={() => handleFilterClick("category")}> category </Dropdown.Item>
                      <Dropdown.Item eventKey="3" active={selectedFilter == "keywords"} onClick={() => handleFilterClick("keywords")}> keyword </Dropdown.Item>
                    </SplitButton>

              </Form>

              {/* ---- client related buttons ----- */}

              {/* show all the time */}
              <Button variant="outline-info" onClick={() => navigate('/cart')}> <RiShoppingCart2Line size={20} /> </Button>
              {/* show if not logged in */}
              {!getLoggedIn() && (
              <Button variant="outline-info" onClick={() => navigate('/login')}>Login</Button> )}
              {!getLoggedIn() && (
              <Button variant="outline-info" onClick={() => navigate('/register')}>Register</Button> )}
              {/* show if logged in */}
              {getLoggedIn() && (
              <Button variant="outline-info" onClick={() => navigate('/profile')}><RiUserLine size={20} /></Button>)}
            </Navbar.Collapse>
          </Container>
        </Navbar>
      );
    
}





export default NavBar;