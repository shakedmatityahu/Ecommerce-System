import { fetchResponse } from "./GeneralService";
import ClientResponse from "./Response";
// import { serverEnterAsGuest } from "./MarketService";

interface Iuser {
  username: string;
}

export const storage = sessionStorage;
const isInitOccured = "isInitOccured";
const userName = "userName";
const isGuest = "isGuest";
const sessionId = "sessionId";
const isAdmin = "isAdmin";
const isLogged = "isLoggedIn";
const token = "token"

export async function initSession() {
  if (storage.getItem(isInitOccured) === null) {
    
    fetchResponse(serverEnterAsGuest())  // Modified to call the function
      .then((value: unknown) => {  // Added type assertion
        const sessionId = value as string;
        storage.setItem(isInitOccured, "true");
        initFields(sessionId);
      })
      .catch(() => {
        alert("Sorry, Could not enter the server");
      });
  }
}

async function serverEnterAsGuest(): Promise<ClientResponse<string>> {
  const uri = "https://localhost:7163" + "/api/Client/Guest?tokenId=123";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionID: "1", // Pass the appropriate session ID value here
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      alert(errorResponse.errorMessage);
      return Promise.reject(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();

    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function initFields(id: string) {
  storage.setItem(isInitOccured, "true");
  setSessionId(id);
  setIsGuest(true);
  setUsername("guest");
  setIsAdmin(false);
  setLoggedIn(false);
}

export function clearSession() {
  storage.clear();
}

export function getIsInitOccured(): boolean {
  const value = storage.getItem(isInitOccured);
  return value === "true";
}

export function setIsInitOccured(value: boolean): void {
  storage.setItem(isInitOccured, value.toString());
}

export function getUserName(): string | null {
  return storage.getItem(userName);
}

export function setUsername(value: string): void {
  storage.setItem(userName, value);
}

export function getIsGuest(): boolean {
  const value = storage.getItem(isGuest);
  return value === "true";
}

export function setIsGuest(value: boolean): void {
  storage.setItem(isGuest, value.toString());
}

export function getSessionId(): string | null {
  return storage.getItem(sessionId);
}

export function setSessionId(value: string): void {
  storage.setItem(sessionId, value);
}

export function getIsAdmin(): boolean {
  const value = storage.getItem(isAdmin);
  return value === "true";
}

export function setIsAdmin(value: boolean): void {
  storage.setItem(isAdmin, value.toString());
}

export function setLoggedIn(value: boolean) {
  storage.setItem(isLogged, value.toString());
}

export function getLoggedIn(): boolean {
  const value = storage.getItem(isLogged);
  return value === "true";
}

export function setToken(value: string) {
  storage.setItem(token, value.toString());
}

export function getToken(): string | null {
  return storage.getItem(token);
}
