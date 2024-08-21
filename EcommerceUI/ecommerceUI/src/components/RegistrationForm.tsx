import React, { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useNavigate } from 'react-router-dom';

interface FormData {
  username: string;
  email: string;
  password: string;
  age: string;
}

interface Errors {
  username?: string;
  email?: string;
  password?: string;
  age?: string;
}

const RegistrationForm: React.FC = () => {
  const [formData, setFormData] = useState<FormData>({
    username: '',
    email: '',
    password: '',
    age: '',
  });

  const [errors, setErrors] = useState<Errors>({});
  const navigate = useNavigate();
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const validate = (): Errors => {
    let formErrors: Errors = {};
    if (!formData.username) formErrors.username = 'Username is required';
    if (!formData.email) formErrors.email = 'Email is required';
    else if (!/\S+@\S+\.\S+/.test(formData.email))
      formErrors.email = 'Email is invalid';
    if (!formData.password) formErrors.password = 'Password is required';
    else if (formData.password.length < 6)
      formErrors.password = 'Password must be at least 6 characters';
    if (!formData.age) formErrors.age = 'Age is required';
    else if (isNaN(parseInt(formData.age, 10)))
      formErrors.age = 'Age must be a number';
    return formErrors;
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formErrors = validate();
    if (Object.keys(formErrors).length === 0) {
      try {
        const response = await fetch(
          `https://localhost:7163/api/Client/Guest/Register`,
          {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData),
          }
        );

        if (response.ok) {
          console.log('Registration successful');
          alert('Registered successfully');
          // Reset form data if needed
          setFormData({ username: '', email: '', password: '', age: '' });
          navigate('/Login');
        } else {
          // Handle error response
          const responseData = await response.json();
          console.error('Registration failed:', responseData);
          // Optionally, you can set specific errors based on the response
          alert('Registration failed. Please try again later.');
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

  return (
    <div className="login-form">
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Username</Form.Label>
          <Form.Control
            type="text"
            name="username"
            value={formData.username}
            onChange={handleChange}
            placeholder="username"
          />
          {errors.username && <p>{errors.username}</p>}
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Email</Form.Label>
          <Form.Control
            type="text"
            name="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="email"
          />
          {errors.email && <p>{errors.email}</p>}
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            placeholder="Password"
            name="password"
            value={formData.password}
            onChange={handleChange}
          />
          {errors.password && <p>{errors.password}</p>}
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Age</Form.Label>
          <Form.Control
            type="text"
            name="age"
            value={formData.age}
            onChange={handleChange}
            placeholder="age"
          />
          {errors.age && <p>{errors.age}</p>}
        </Form.Group>

        <Button variant="primary" type="submit">
          Register
        </Button>
      </Form>
    </div>
  );
};

export default RegistrationForm;
