import React, {Component} from "react";
import { useLocation } from 'react-router-dom';
import SearchResults from "../components/SearchResults";

export const Search = () => {
    const location = useLocation();
    const query = new URLSearchParams(location.search).get('query');
    const filter = new URLSearchParams(location.search).get('filter');
    
    return(
        <SearchResults query={query} filter={filter}/>
    );
}