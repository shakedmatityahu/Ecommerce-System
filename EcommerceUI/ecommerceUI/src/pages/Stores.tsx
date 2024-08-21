import React, {Component} from "react";
import { useLocation } from 'react-router-dom';
import SearchResults from "../components/SearchResults";
import StoresList from "../components/StoresList";




export const Stores = () => {    
    return(
        <StoresList/>
    );
}