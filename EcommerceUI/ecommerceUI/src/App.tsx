import React, { useState } from 'react';
import {BrowserRouter, Routes, Route} from 'react-router-dom'
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';
import { Home } from "./pages/Home"
import { Login } from "./pages/Login"
import { NavBar } from "./components/NavBar"
import { Register } from './pages/Register';
import { Profile } from './pages/Profile';
import { initSession } from './services/SessionService';
import { Search } from './pages/Search';
import { Cart } from './pages/Cart';
import { Purchase } from './pages/Purchase';
import { Stores } from './pages/Stores';



function App() {
  initSession();
  const [loggedIn, setLoggedIn] = useState(false)
  const [username, setUsername] = useState('')

  return (
    <div>
      <BrowserRouter>
        <NavBar loggedIn={false} setLoggedIn={setLoggedIn}/>
        <Routes>
          <Route index element = {<Home/>}/>
          <Route path="/home" element={<Home/>}/>
          <Route path="/login" element={<Login/>}  />
          <Route path="/Register" element={<Register/>}/>
          <Route path="/search" element={<Search/>}/>
          <Route path="/profile" element={<Profile/>}/>
          <Route path="/cart" element={<Cart/>}/>
          <Route path="/Purchase" element={<Purchase/>}/>
          <Route path="/stores" element={<Stores/>}/>

        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
