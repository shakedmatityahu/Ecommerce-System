import React, {Component, useState} from "react";
import { useNavigate } from "react-router-dom";
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { getToken, setToken, setUsername } from "../services/SessionService";
import { setLoggedIn } from "../services/SessionService";
import { initWebSocket } from "../services/NotificationsService";





export const Login = () => {
  const [username, setusername] = useState('')
  const [password, setpassword] = useState('')
  const [usernameError, setUsernameError] = useState('')
  const [passwordError, setPasswordError] = useState('')

  const navigate = useNavigate()

  const onButtonClick = () => {
    // Set initial error values to empty
  setUsernameError('')
  setPasswordError('')
  
  // Check if the user has entered both fields correctly
  if ('' === username ) {
    setUsernameError('Please enter your username')
    return
  }


  if ('' === password) {
    setPasswordError('Please enter a password')
    return
  }

  if (password.length < 7) {
    setPasswordError('The password must be 8 characters or longer')
    return
  }
  
  logIn()
   
  }
  
  const logIn = () => {
    fetch(`https://localhost:7163/api/Client/Guest/Login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({username, password }),
    }).then((r) => {
      if (r.ok) {
        const address = `ws://127.0.0.1:4570/${username}-alerts`;
        initWebSocket(address);
        setLoggedIn(true)
        return r.json(); 
      } else {
        throw new Error('Wrong username or password');
      }
    })
    .then((data) => {
      // `data` is the parsed JSON object
      setToken(data.value)
      setUsername(username);
      navigate('/');
    })
    .catch((error) => {
      window.alert(error.message);
    });
};


  return (
    
    <div className="login-form" >
    <Form.Group className="mb-3">
      <Form.Label>Username</Form.Label>
      <Form.Control type="text"
        name="username"
        value={username}
        onChange={(ev) => setusername(ev.target.value)} 
        placeholder="username" />
    </Form.Group>
    <label className="errorLabel">{usernameError}</label>

    <Form.Group className="mb-3" >
      <Form.Label>Password</Form.Label>
      <Form.Control type="password" placeholder="Password" 
                    name="password"
                    value={password}
                    onChange={(ev) => setpassword(ev.target.value)}
                    />
    </Form.Group>
    <label className="errorLabel">{passwordError}</label>
    <Button variant="primary" type="submit" onClick={onButtonClick}>
      Login 
    </Button>
    </div>
    );
}


