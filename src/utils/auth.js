import { jwtDecode } from "jwt-decode";

export const isAuthenticated = () => {
  const token = localStorage.getItem("token");
  const refreshToken = localStorage.getItem("refreshToken");

  if (!token || !refreshToken) {
    return false;
  }

  try {
    jwtDecode(token);
    return true;

  } catch (error) {
    console.error("isAuthenticated: Error decoding access token:", error);
    
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    return false;
  }
};