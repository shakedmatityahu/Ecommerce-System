import React, {Component} from "react";
import SearchResults from "../components/SearchResults";
import { initSession } from "../services/SessionService";


export const Home = () => {
    initSession();
    return (
        <SearchResults query="All" filter="category"/>
      );
}