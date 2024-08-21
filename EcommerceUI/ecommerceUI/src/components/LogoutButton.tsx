import React from 'react';
import {clearSession, getToken} from '../services/SessionService';

interface LogoutButtonProps {
}

const LogoutButton: React.FC<LogoutButtonProps> = () => {

    const handleLogout = async () => {
      try {
        const response = await fetch(`https://localhost:7163/api/Client/Member/Logout?identifier=${getToken()}`, 
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
        });
  
        if (response.ok) {
          console.log('Logout successful');
          alert("Logout successfully");
          clearSession()
        } else {
          // Handle error response
          const responseData = await response.json();
          console.error('Logout failed:', responseData);
          // Optionally, you can set specific errors based on the response
          alert('Logout failed. Please try again later.');
        }
      } catch (error) {
        console.error('Error occurred while registering:', error);
        // Handle network errors or other exceptions
        alert('An error occurred while processing your request. Please try again later.');
      }
      window.location.href = '/home'; // Move this to the right place
    }; // Move this to the right place

    return (
        <button onClick={handleLogout}>
            Logout
        </button>
    );
};
export default LogoutButton;
